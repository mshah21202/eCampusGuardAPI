using AutoMapper;
using AutoMapper.QueryableExtensions;
using eCampusGuard.API.Extensions;
using eCampusGuard.API.Helpers;
using eCampusGuard.Core.Consts;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid;

namespace eCampusGuard.API.Controllers
{
    [Authorize]
    public class UserPermitsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly INotificationService<Response> _notificationService;

        public UserPermitsController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, INotificationService<Response> notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _notificationService = notificationService;
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
            var isAdmin = roles.Any(r => r == "Admin");
            var userPermits = (await _unitOfWork.UserPermits.FindAllAsync(
                userPermitParams.Criteria(isAdmin, user),
                null,
                userPermitParams.OrderByMember(),
                userPermitParams.OrderByDirection,
                userPermitParams.PageSize,
                userPermitParams.PageNumber * userPermitParams.PageSize)).AsQueryable();

            var count = await _unitOfWork.UserPermits.CountAsync(userPermitParams.Criteria(isAdmin, user));


            Response.AddPaginationHeader(userPermitParams.PageNumber,
                userPermitParams.PageSize,
                count, (int)Math.Ceiling(count / (double)userPermitParams.PageSize));


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

            var userPermits = await _unitOfWork.UserPermits.FindAllAsync(
                up => (up.UserId == User.GetUserId()) &&
                (up.Status == UserPermitStatus.Valid ||
                up.Status == UserPermitStatus.Expired ||
                up.Status == UserPermitStatus.Withdrawn), null, up => up.Id, OrderBy.Descending);


            if (user == null || userPermits == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<UserPermitDto>(userPermits.First()));
        }

        /// <summary>
        /// Gets user permit
        /// </summary>
        /// <param name="id">UserPermit Id</param>
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
            var userpermit = await _unitOfWork.UserPermits.FindAsync(a => (a.UserId == User.GetUserId() || roles.Any(r => r == "Admin")) && (a.Id == id), includes: new[] { "Vehicle", "Permit" });


            // If user is not admin and user is trying to get permit of another user
            if (userpermit == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserPermitDto>(userpermit));
        }

        /// <summary>
        /// Updates user permit details. Admin only
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userPermitDto"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("{id}")]
        public async Task<ActionResult<ResponseDto>> UpdateUserPermit(int id, UpdateUserPermitDto userPermitDto)
        {
            try
            {
                var userPermit = await _unitOfWork.UserPermits.GetByIdAsync(id);

                if (userPermit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "User permit not found"
                    });
                }

                userPermit.PermitApplication.PhoneNumber = userPermitDto.PhoneNumber;
                userPermit.PermitApplication.PhoneNumberCountry = userPermitDto.PhoneNumberCountry;

                if (userPermitDto.LicenseImgPath != null)
                    userPermit.PermitApplication.LicenseImgPath = userPermitDto.LicenseImgPath;

                userPermit.Vehicle.PlateNumber = userPermitDto.Vehicle.PlateNumber;
                userPermit.Vehicle.Nationality = userPermitDto.Vehicle.Nationality;
                userPermit.Vehicle.Make = userPermitDto.Vehicle.Make;
                userPermit.Vehicle.Model = userPermitDto.Vehicle.Model;
                userPermit.Vehicle.Color = userPermitDto.Vehicle.Color;
                userPermit.Vehicle.Year = userPermitDto.Vehicle.Year;
                if (userPermitDto.Vehicle.RegistrationDocImgPath != null)
                    userPermit.Vehicle.RegistrationDocImgPath = userPermitDto.Vehicle.RegistrationDocImgPath;

                if (userPermitDto.PermitId != null)
                {
                    var permit = await _unitOfWork.Permits.GetByIdAsync((int)userPermitDto.PermitId);
                    if (permit != null)
                    {
                        userPermit.Permit = permit;
                        userPermit.PermitId = permit.Id;
                    }
                }

                _unitOfWork.UserPermits.Update(userPermit);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully updated user permit"
                    });
                }

                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Something went wrong"
                });
                
            } catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// Withdraws user permit. Admin only.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("withdraw/{id}")]
        public async Task<ActionResult<ResponseDto>> WithdrawPermit(int id)
        {
            try
            {
                var userPermit = await _unitOfWork.UserPermits.GetByIdAsync(id);

                if (userPermit == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "User permit not found"
                    });
                }

                if ((userPermit.Permit.Expiry < DateTime.Now))
                    return BadRequest(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "User permit already expired"
                    });

                if (userPermit.Status == UserPermitStatus.Valid)
                {
                    userPermit.Status = UserPermitStatus.Withdrawn;
                } else
                {
                    userPermit.Status = UserPermitStatus.Valid;
                }

                _unitOfWork.UserPermits.Update(userPermit);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    if (userPermit.Status == UserPermitStatus.Withdrawn)
                    {
                        await _notificationService.SendGeneralNotificationAsync(new List<AppUser> { userPermit.User }, "Your permit has been withdrawn", "Your permit " + userPermit.Permit.Name + " has been withdrwan by an admin.");
                    } else if (userPermit.Status == UserPermitStatus.Valid)
                    {
                        await _notificationService.SendGeneralNotificationAsync(new List<AppUser> { userPermit.User }, "Your permit has been reinstated", "Your permit " + userPermit.Permit.Name + " has been reinstated by an admin.");
                    }
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully updated user permit"
                    });
                }

                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Something went wrong"
                });
            } catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// Submits an update request for user permit
        /// </summary>
        /// <param name="updateRequestDto"></param>
        /// <returns></returns>
        [HttpPost("/update")]
        public async Task<ActionResult<ResponseDto>> SubmitUpdateRequest(CreateUpdateRequestDto updateRequestDto)
        {
            try
            {
                var userPermit = await _unitOfWork.UserPermits.FirstOrDefaultAsync(
               up => (up.UserId == User.GetUserId()) &&
               (up.Status == UserPermitStatus.Valid || up.Status == UserPermitStatus.Expired));

                var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

                if (userPermit == null || user == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find user permit"
                    });
                }

                if (userPermit.UpdateRequests.Any(ur => ur.Status == UpdateRequestStatus.Pending))
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "You already have a pending update request."
                    });
                }

                var vehicle = _mapper.Map<Vehicle>(updateRequestDto.Vehicle);
                vehicle.User = user;

                if (vehicle.RegistrationDocImgPath == null)
                {
                    vehicle.RegistrationDocImgPath = userPermit.Vehicle.RegistrationDocImgPath;
                }


                var updateRequest = new UpdateRequest
                {
                    UserPermit = userPermit,
                    UpdatedVehicle = vehicle,
                    PhoneNumber = updateRequestDto.PhoneNumber,
                    PhoneNumberCountry = updateRequestDto.PhoneNumberCountry,
                    DrivingLicenseImgPath = updateRequestDto.DrivingLicenseImgPath
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
        /// Sends a general notification to the user of a user permit.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notificationDto"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("notification/{id}")]
        public async Task<ActionResult<ResponseDto>> SendGeneralNotification(int id, NotificationDto notificationDto)
        {
            var userpermit = await _unitOfWork.UserPermits.GetByIdAsync(id);

            if (userpermit == null)
            {
                return NotFound(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Could not find user permit"
                });
            }

            var notification = new Notification
            {
                Title = notificationDto.Title,
                Body = notificationDto.Body,
                //User = userpermit.User
            };

            await _unitOfWork.Notifications.AddAsync(notification);

            if (await _unitOfWork.CompleteAsync() < 0)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Something went wrong"
                });
            }

            var response = await _notificationService.SendGeneralNotificationAsync(new List<AppUser> { userpermit.User }, notification.Title, notification.Body);
            if (response.IsSuccessStatusCode)
            {
                return Ok(new ResponseDto
                {
                    ResponseCode = ResponseCode.Success,
                    Message = "Notification sent."
                });
            }

            return BadRequest(new ResponseDto
            {
                ResponseCode = ResponseCode.Failed,
                Message = "Something went wrong"
            });
        }

        /// <summary>
        /// Get all update requests
        /// </summary>
        /// <returns></returns>
        [HttpGet("update-requests")]
        public async Task<ActionResult<IEnumerable<UpdateRequestDto>>> GetUpdateRequests([FromQuery]UpdateRequestsParams requestsParams)
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
            var updateRequest = await _unitOfWork.UpdateRequests.FindAsync(
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

                    userPermit.PermitApplication.PhoneNumber = updateRequest.PhoneNumber;
                    userPermit.PermitApplication.PhoneNumberCountry = updateRequest.PhoneNumberCountry;

                    userPermit.Vehicle = updateRequest.UpdatedVehicle;

                    _unitOfWork.UserPermits.Update(userPermit);

                }
                else
                {
                    updateRequest.Status = UpdateRequestStatus.Denied;
                }

                _unitOfWork.UpdateRequests.Update(updateRequest);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    await _notificationService.SendGeneralNotificationAsync(new List<AppUser> { updateRequest.UserPermit.User }, "New response to your update request", "Your update request has been " + (updateRequest.Status == UpdateRequestStatus.Accepted ? "accpted" : "denied"));

                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully " + (accept ? "accepted" : "denied") + " update request"
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

