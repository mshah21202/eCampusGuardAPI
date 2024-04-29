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
        public async Task<ActionResult<List<UserPermitDto>>> GetUserPermits([FromQuery] UserPermitParams userPermitParams)
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Any(r => r == "Admin");
            var userPermits = (await _unitOfWork.UserPermits.FindAllAsync(
                userPermitParams.Criteria(isAdmin, user),
                null,
                userPermitParams.OrderByMember(),
                userPermitParams.OrderByDirection,
                userPermitParams.PageSize,
                userPermitParams.PageNumber * userPermitParams.PageSize)).AsQueryable();

            return Ok(userPermits.ProjectTo<UserPermitDto>(_mapper.ConfigurationProvider).ToList());
        }

        /// <summary>
        /// Gets the relevant user permit, mainly used for the home screen.
        /// </summary>
        /// <returns></returns>
        [HttpGet("relevant")]
        public async Task<ActionResult<UserPermitDto>> GetRelevantUserPermit()
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            var userPermit = await _unitOfWork.UserPermits.FirstOrDefaultAsync(
                up => (up.UserId == User.GetUserId()) &&
                (up.Status == UserPermitStatus.Valid || up.Status == UserPermitStatus.Expired));

            if (user == null || userPermit == null)
            {
                return NotFound();
            }
            

            return Ok(_mapper.Map<UserPermitDto>(userPermit));
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
        /// Submits an update request for user permit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateRequestDto"></param>
        /// <returns></returns>
        [HttpPost("{id}/update")]
        public async Task<ActionResult<ResponseDto>> SubmitUpdateRequest(int id, CreateUpdateRequestDto updateRequestDto)
        {
            try
            {
                var userPermit = await _unitOfWork.UserPermits.FindAsync(up => up.PermitId == id && up.UserId == User.GetUserId());

                if (userPermit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find user permit"
                    });
                }

                var newPermit = await _unitOfWork.Permits.GetByIdAsync(updateRequestDto.PermitId);

                if (newPermit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find permit"
                    });
                }

                var vehicle = _mapper.Map<Vehicle>(updateRequestDto.Vehicle);

                var updateRequest = new UpdateRequest
                {
                    UserPermit = userPermit,
                    NewPermit = newPermit,
                    UpdatedVehicle = vehicle,
                };

                await _unitOfWork.UpdateRequests.AddAsync(updateRequest);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully created update request"
                    });
                }

                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Something went wrong"
                });

            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.Message
                });
            }

        }

        /// <summary>
        /// Get all update requests
        /// </summary>
        /// <returns></returns>
        [HttpGet("update-requests")]
        public async Task<ActionResult<IEnumerable<UpdateRequestDto>>> GetUpdateRequests(int id, [FromQuery]UpdateRequestsParams requestsParams)
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Any(r => r == "Admin");

            var updateRequests = (await _unitOfWork.UpdateRequests.FindAllAsync(
                requestsParams.Criteria(isAdmin, user),
                null,
                null,
                "ASC",
                requestsParams.PageSize,
                requestsParams.PageSize * requestsParams.PageNumber)).AsQueryable();

            var count = await _unitOfWork.UpdateRequests.CountAsync(requestsParams.Criteria(isAdmin, user));


            Response.AddPaginationHeader(requestsParams.PageNumber, requestsParams.PageSize, count, (int)Math.Ceiling(count / (double)requestsParams.PageSize));


            return Ok(updateRequests.ProjectTo<UpdateRequestDto>(_mapper.ConfigurationProvider).ToList());
        }

        /// <summary>
        /// Get update request
        /// </summary>
        /// <returns></returns>
        [HttpGet("update-requests/{id}")]
        public async Task<ActionResult<UpdateRequestDto>> GetUpdateRequest(int id)
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Any(r => r == "Admin");
            var updateRequest = await _unitOfWork.UpdateRequests.FindAllAsync(
                ur => (ur.Id == id) && (!isAdmin ? ur.UserPermit.UserId == User.GetUserId() : true)
                );

            if (updateRequest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UpdateRequestDto>(updateRequest));
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("update-requests/{id}/response")]
        public async Task<ActionResult<ResponseDto>> SubmitUpdateRequestResponse(int id, [FromQuery] bool accept)
        {
            try
            {
                var updateRequest = await _unitOfWork.UpdateRequests.GetByIdAsync(id);

                if (updateRequest == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Couldn't find update request"
                    });
                }

                if (accept)
                {
                    updateRequest.Status = UpdateRequestStatus.Accepted;

                    var userPermit = updateRequest.UserPermit;

                    userPermit.Vehicle = updateRequest.UpdatedVehicle;

                    userPermit.Permit = updateRequest.NewPermit;

                    _unitOfWork.UserPermits.Update(userPermit);

                }
                else
                {
                    updateRequest.Status = UpdateRequestStatus.Denied;
                }

                _unitOfWork.UpdateRequests.Update(updateRequest);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully accepted update request"
                    });
                }

                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Something went wrong"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.Message
                });
            }
        }
    }
}

