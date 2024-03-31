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
	public class UserPermitController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserPermitController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets all user permit for user, or gets all user permits for all users if user is admin
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
		public async Task<ActionResult<List<UserPermitDto>>> GetUserPermits([FromQuery]UserPermitParams userPermitParams)
		{
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userPermits = (await _unitOfWork.UserPermits.FindAllAsync(
                criteria: p => roles.Any(r => r == "Admin") ? true : p.UserId == User.GetUserId(),
                orderBy: p => userPermitParams.OrderByMember(p),
                orderByDirection: userPermitParams.OrderByDirection,
                skip: (userPermitParams.PageNumber - 1) * userPermitParams.PageSize,
                take: userPermitParams.PageSize)).AsQueryable();

            return Ok(userPermits.ProjectTo<UserPermitDto>(_mapper.ConfigurationProvider).ToList());
        }

        /// <summary>
        /// Gets user permit
        /// </summary>
        /// <param name="id">Permit Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPermitDto>> GetUserPermit(int id)
		{
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userpermit = await _unitOfWork.UserPermits.FindAsync(a => (a.UserId == User.GetUserId() || roles.Any(r => r == "Admin")) && (a.PermitId == id), includes: new[] { "Vehicle", "Permit" });


            // If user is not admin and user is trying to get permit of another user
            if (userpermit == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PermitApplicationDto>(userpermit));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Permit Id</param>
        /// <param name="transferRequestDto"></param>
        /// <returns></returns>
		[HttpPost("transfer/{id}")]
		public async Task<ActionResult<ResponseDto>> SubmitTransferRequest(int id, TransferRequestDto transferRequestDto)
		{
            try
            {
                // Get user permit
                var userpermit = await _unitOfWork.UserPermits.FindAsync(ur => ur.PermitId == id && ur.UserId == User.GetUserId());

                if (userpermit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find user permit"
                    });
                }

                var user = userpermit.User;
                var toUser = await _unitOfWork.AppUsers.GetByIdAsync(transferRequestDto.StudentId);

                if (user == null || toUser == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find user"
                    });
                }

                var permit = userpermit.Permit;

                if (permit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find permit"
                    });
                }

                var transferRequest = new TransferRequest
                {
                    FromUser = user,
                    ToUser = toUser,
                    Permit = permit
                };

                await _unitOfWork.TransferRequests.AddAsync(transferRequest);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Success,
                        Message = "Created transfer request successfully"
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
        /// Submits a details update request for user permit
        /// </summary>
        /// <param name="id">Permit Id</param>
        /// <param name="transferRequestDto"></param>
        /// <returns></returns>
		[HttpPost("details/{id}")]
        public async Task<ActionResult<ResponseDto>> SubmitDetailsUpdateRequest(int id, UpdateRequestDto updateRequestDto)
		{
            try
            {
                var userpermit = await _unitOfWork.UserPermits.FindAsync(ur => ur.PermitId == id && ur.UserId == User.GetUserId());

                if (userpermit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find user permit"
                    });
                }

                var detailsUpdateRequest = new UpdateDetailsRequest
                {
                    User = userpermit.User,
                    Permit = userpermit.Permit,
                    LicenseImgPath = updateRequestDto.LicenseImgPath,
                    Vehicle = _mapper.Map<Vehicle>(updateRequestDto.Vehicle)
                };

                await _unitOfWork.UpdateDetailsRequests.AddAsync(detailsUpdateRequest);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Success,
                        Message = "Created update details request successfully"
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
    }
}

