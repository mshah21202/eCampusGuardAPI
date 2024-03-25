using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using eCampusGuard.API.Extensions;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
    [Authorize]
	public class PermitApplicationController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PermitApplicationController(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
		}

        /// <summary>
        /// Gets all permit applications for user, or gets all permit applications for all users if request is made by admin
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IEnumerable<PermitApplicationInfoDto>> GetPermitApplications()
        {
            var roles = await User.GetUserRolesAsync(_unitOfWork);

            var applications = (await _unitOfWork.PermitApplications.FindAllAsync(p => roles.Any(r => r.Name == "Admin") ? true : p.UserId == User.GetUserId())).AsQueryable();

            return applications.ProjectTo<PermitApplicationInfoDto>(_mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Gets permit application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PermitApplicationDto>> GetPermitApplication(int id)
        {
            var application = await _unitOfWork.PermitApplications.GetByIdAsync(id);

            var roles = await User.GetUserRolesAsync(_unitOfWork);

            // If user is not admin and user is trying to get application of another user
            if (!roles.Any(r => r.Name == "Admin") && application.UserId != User.GetUserId())
            {
                return Unauthorized();
            }

            return _mapper.Map<PermitApplicationDto>(application);
        }

        /// <summary>
        /// Submits permit application for user
        /// </summary>
        /// <param name="permitApplicationDto"></param>
        /// <returns></returns>
        [HttpPost("apply")]
        public async Task<ActionResult<ResponseDto>> Apply(PermitApplicationDto permitApplicationDto)
        {
            try
            {
                PermitApplication application = _mapper.Map<PermitApplication>(permitApplicationDto);

                application.UserId = User.GetUserId();

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
                    Message = e.ToString()
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

