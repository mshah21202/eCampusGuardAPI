using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using eCampusGuard.API.Extensions;
using eCampusGuard.API.Helpers;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	public class SearchController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public SearchController(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize(Policy = "RequireGateStaffRole")]
        [HttpGet]
		public async Task<ActionResult<IEnumerable<UserPermitDto>>> Search([FromQuery]UserPermitParams searchParams)
		{
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            var userPermits = (await _unitOfWork.UserPermits.FindAllAsync(
                searchParams.Criteria(true, user),
                null,
                searchParams.OrderByMember(),
                searchParams.OrderByDirection,
                searchParams.PageSize,
                searchParams.PageNumber * searchParams.PageSize)).AsQueryable();

            var count = await _unitOfWork.UserPermits.CountAsync(searchParams.Criteria(true, user));


            Response.AddPaginationHeader(searchParams.PageNumber,
                searchParams.PageSize,
                count, (int)Math.Ceiling(count / (double)searchParams.PageSize));

            return Ok(userPermits.ProjectTo<UserPermitDto>(_mapper.ConfigurationProvider).ToList());
		}
	}
}

