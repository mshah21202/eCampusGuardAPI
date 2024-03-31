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
	public class PermitApplicationController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public PermitApplicationController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
		}

        /// <summary>
        /// Gets all permit applications for user, or gets all permit applications for all users if request is made by admin
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PermitApplicationInfoDto>>> GetPermitApplications([FromQuery]PermitApplicationParams applicationParams)
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var applications = (await _unitOfWork.PermitApplications.FindAllAsync(
                criteria: p => applicationParams.Criteria(p, roles.Any(r => r == "Admin"), user),
                orderBy: p => applicationParams.OrderByMember(p),
                orderByDirection: applicationParams.OrderByDirection,
                skip: (applicationParams.PageNumber - 1) * applicationParams.PageSize,
                take: applicationParams.PageSize)).AsQueryable();

            return Ok(applications.ProjectTo<PermitApplicationInfoDto>(_mapper.ConfigurationProvider).ToList());
        }

        /// <summary>
        /// Gets permit application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PermitApplicationDto>> GetPermitApplication(int id)
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var application = await _unitOfWork.PermitApplications.FindAsync(a => (a.UserId == User.GetUserId() || roles.Any(r => r == "Admin")) && (a.Id == id), includes: new[] { "Vehicle", "Permit" });


            // If user is not admin and user is trying to get application of another user
            if (application == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PermitApplicationDto>(application));
        }

        /// <summary>
        /// Submits permit application for user
        /// </summary>
        /// <param name="permitApplicationDto"></param>
        /// <returns></returns>
        [HttpPost("apply")]
        public async Task<ActionResult<ResponseDto>> Apply(CreatePermitApplicationDto permitApplicationDto)
        {
            try
            {
                PermitApplication application = _mapper.Map<PermitApplication>(permitApplicationDto);

                var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

                if (user == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find user"
                    });
                }

                application.User = user;
                application.Vehicle.User = user;

                var permit = await _unitOfWork.Permits.GetByIdAsync(application.PermitId);

                if (permit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find permit"
                    });
                }

                application.Permit = permit;

                await _unitOfWork.PermitApplications.AddAsync(application);
                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Success
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCodeEnum.Failed,
                    Message = e
                });
            }
            

            return BadRequest(new ResponseDto
            {
                ResponseCode = ResponseCodeEnum.Failed,
                Message = "Something went wrong"
            });
        }

        /// <summary>
        /// Submits application response. For admin only
        /// </summary>
        /// <param name="permitApplicationDto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("response/{id}")]
        public async Task<ActionResult<ResponseDto>> SubmitApplicationResponse(PermitApplicationDto permitApplicationDto, int id)
        {
            try
            {
                var application = await _unitOfWork.PermitApplications.GetByIdAsync(id);

                if (application == null)
                {
                    return BadRequest(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find application with id " + id
                    });
                }

                _unitOfWork.PermitApplications.Update(_mapper.Map<PermitApplication>(permitApplicationDto));
                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Success,
                        Message = "Successfully updated permit application"
                    });
                }

                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCodeEnum.Failed,
                    Message = "Could not save permit application"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCodeEnum.Failed,
                    Message = e.ToString()
                });
            }

        }
    }
}

