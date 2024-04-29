using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using eCampusGuard.API.Extensions;
using eCampusGuard.API.Helpers;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
    [Authorize]
	public class AreaController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AreaController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

		[HttpGet()]
		public async Task<ActionResult<IEnumerable<AreaDto>>> GetAreas([FromQuery]PaginationParams paginationParams)
		{
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var areas = (await _unitOfWork.Areas.FindAllAsync(
                a => true,
                null,
                null,
                "ASC",
                paginationParams.PageSize,
                paginationParams.PageSize * paginationParams.PageNumber)).AsQueryable();

            var totalItems = await _unitOfWork.Areas.CountAsync();

            Response.AddPaginationHeader(paginationParams.PageNumber, paginationParams.PageSize, totalItems, (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize));

            return Ok(areas.ProjectTo<AreaDto>(_mapper.ConfigurationProvider).ToList());
        }

        [Authorize(Policy = "RequireAdminRole")]
		[HttpPost()]
		public async Task<ActionResult<ResponseDto>> CreateArea(AreaDto areaDto)
		{
            try
            {
                var area = _mapper.Map<Area>(areaDto);

                await _unitOfWork.Areas.AddAsync(area);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Area created successfully"
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.ToString()
                });
            }

            return BadRequest(new ResponseDto
            {
                ResponseCode = ResponseCode.Failed,
                Message = "Something went wrong"
            });
		}

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("{id}")]
        public async Task<ActionResult<ResponseDto>> UpdateArea(int id, AreaDto areaDto)
        {
            try
            {
                var area = _mapper.Map<Area>(areaDto);

                area.Id = id;

                _unitOfWork.Areas.Update(area);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Area updated successfully"
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.ToString()
                });
            }

            return BadRequest(new ResponseDto
            {
                ResponseCode = ResponseCode.Failed,
                Message = "Something went wrong"
            });
        }

        [HttpGet("{id}")]
		public async Task<ActionResult<AreaDto>> GetArea(int id)
		{
            var area = await _unitOfWork.Areas.GetByIdAsync(id);

            if (area == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AreaDto>(area));
		}


        [Authorize(Policy = "RequireGateStaffRole")]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<AreaScreenDto>> GetAreaDetails(int id)
		{
            var area = await _unitOfWork.Areas.GetByIdAsync(id);

            var accessLogs = (await _unitOfWork.AccessLogs.FindAllAsync(al => al.UserPermit.Permit.AreaId == id)).AsQueryable();

            return Ok(new AreaScreenDto
            {
                AccessLogs = accessLogs.ProjectTo<AccessLogDto>(_mapper.ConfigurationProvider)
            });
		}

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto>> DeleteArea(int id)
        {
            try
            {
                var area = await _unitOfWork.Areas.GetByIdAsync(id);

                if (area == null)
                {
                    return NotFound();
                }

                _unitOfWork.Areas.Delete(area);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Area deleted successfully"
                    });
                }

            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.ToString()
                });
            }

            return BadRequest(new ResponseDto
            {
                ResponseCode = ResponseCode.Failed,
                Message = "Something went wrong"
            });
        }
    }
}

