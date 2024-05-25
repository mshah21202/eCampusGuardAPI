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
using SendGrid;
using static eCampusGuard.Core.Entities.PermitApplication;
using static eCampusGuard.Core.Entities.UserPermit;

namespace eCampusGuard.API.Controllers
{
    [Authorize]
    public class PermitApplicationController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly INotificationService<Response> _notificationService;

        public PermitApplicationController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, INotificationService<Response> notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets all permit applications for user, or gets all permit applications for all users if request is made by admin
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PermitApplicationInfoDto>>> GetPermitApplications([FromQuery] PermitApplicationParams applicationParams)
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Any(r => r == "Admin");
            var applications = (await _unitOfWork.PermitApplications.FindAllAsync(
               applicationParams.Criteria(isAdmin, user),
               null,
               applicationParams.OrderByMember(),
               applicationParams.OrderByDirection,
               applicationParams.PageSize,
               applicationParams.PageNumber * applicationParams.PageSize)).AsQueryable();

            var count = await _unitOfWork.PermitApplications.CountAsync(applicationParams.Criteria(isAdmin, user));

            Response.AddPaginationHeader(applicationParams.PageNumber, applicationParams.PageSize, count, (int)Math.Ceiling(count / (double)applicationParams.PageSize));

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
            var isAdmin = roles.Any(r => r == "Admin");
            var application = await _unitOfWork.PermitApplications.FindAsync(
                a => (a.Id == id) && (isAdmin || a.UserId == User.GetUserId())
            );

            
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
            if (await _unitOfWork.PermitApplications.CountAsync(p =>
            p.UserId == User.GetUserId() &&
            (p.Status == PermitApplicationStatus.Pending ||
            p.Status == PermitApplicationStatus.AwaitingPayment)) > 0)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "You already have a pending application."
                });
            }

            try
            {
                PermitApplication application = _mapper.Map<PermitApplication>(permitApplicationDto);

                var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

                if (user == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
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
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find permit"
                    });
                }

                application.Permit = permit;

                await _unitOfWork.PermitApplications.AddAsync(application);
                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    if (user.Email != null)
                    {
                        await _notificationService.SendApplicationSubmittedAsync(user, application.Id);
                    }
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully submitted application"
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e
                });
            }


            return BadRequest(new ResponseDto
            {
                ResponseCode = ResponseCode.Failed,
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
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find application with id " + id
                    });
                }

                var updatedApplication = _mapper.Map<PermitApplication>(permitApplicationDto);
                application.Permit = await _unitOfWork.Permits.GetByIdAsync(updatedApplication.Permit.Id);
                application.PermitId = updatedApplication.PermitId;
                application.AttendingDays = new List<bool>(updatedApplication.AttendingDays);
                application.PhoneNumber = updatedApplication.PhoneNumber;
                application.SiblingsCount = updatedApplication.SiblingsCount;
                application.Status = updatedApplication.Status;
                application.PhoneNumber = updatedApplication.PhoneNumber;
                application.Vehicle.Color = updatedApplication.Vehicle.Color;
                application.Vehicle.Make = updatedApplication.Vehicle.Make;
                application.Vehicle.Model = updatedApplication.Vehicle.Model;
                application.Vehicle.Nationality = updatedApplication.Vehicle.Nationality;
                application.Vehicle.PlateNumber = updatedApplication.Vehicle.PlateNumber;
                application.Vehicle.Year = updatedApplication.Vehicle.Year;
                application.Year = updatedApplication.Year;


                _unitOfWork.PermitApplications.Update(application);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    var user = await _unitOfWork.AppUsers.GetByIdAsync(application.UserId);
                    if (user != null && user.Email != null)
                    {
                        if (application.Status == PermitApplicationStatus.AwaitingPayment)
                        {
                            //await _notificationService.SendApplicationAcceptedAsync(user, application.Id);
                        }

                        if (application.Status == PermitApplicationStatus.Denied)
                        {
                            //await _notificationService.SendApplicationDeniedAsync(user, application.Id);
                        }

                        if (application.Status == PermitApplicationStatus.Paid)
                        {
                            //await _notificationService.SendPaymentSuccessfulAsync(user, application.Id);
                        }
                    }
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully updated permit application"
                    });
                }

                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Could not save permit application"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = e.ToString()
                });
            }

        }

        [HttpPost("pay/{id}")]
        public async Task<ActionResult<ResponseDto>> OnPaymentSuccessful(int id)
        {
            try
            {
                var application = await _unitOfWork.PermitApplications.GetByIdAsync(id);

                if (application == null)
                {
                    return BadRequest(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find application with id " + id
                    });
                }

                if (application.Status != PermitApplicationStatus.AwaitingPayment)
                {
                    return BadRequest(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Application's status is not awaiting payment"
                    });
                }

                var userPermit = new UserPermit
                {
                    Status = UserPermitStatus.Valid,
                    User = application.User,
                    Permit = application.Permit,
                    Vehicle = application.Vehicle,
                    PermitApplication = application
                };

                await _unitOfWork.UserPermits.AddAsync(userPermit);

                application.Status = PermitApplicationStatus.Paid;
                application.UserPermit = userPermit;

                _unitOfWork.PermitApplications.Update(application);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    var user = await _unitOfWork.AppUsers.GetByIdAsync(application.UserId);
                    if (user != null && user.Email != null)
                    {
                        if (application.Status == PermitApplicationStatus.Paid)
                        {
                            var response = await _notificationService.SendPaymentSuccessfulAsync(user, application.Id);
                        }
                    }
                        
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Successfully paid"
                    });
                }

                return Ok(new ResponseDto
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

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("summary")]
        public async Task<ActionResult<List<ApplicationSummaryDto>>> GetApplicationSummary()
        {
            // Categories = [Needs Review, Awaiting Payment, Paid]

            try
            {
                var needsReviewCount = await _unitOfWork.PermitApplications.CountAsync(a => a.Status == PermitApplicationStatus.Pending);
                var awaitingPaymentCount = await _unitOfWork.PermitApplications.CountAsync(a => a.Status == PermitApplicationStatus.AwaitingPayment);
                var paidCount = await _unitOfWork.PermitApplications.CountAsync(a => a.Status == PermitApplicationStatus.Paid);

                List<ApplicationSummaryDto> result = new()
                {
                    new ApplicationSummaryDto
                    {
                        Title = "Needs Review",
                        Count = needsReviewCount,
                        Icon = "0xe51c",
                        Route = "status=0"
                    },
                    new ApplicationSummaryDto
                    {
                        Title = "Awaiting Payment",
                        Count = awaitingPaymentCount,
                        Icon = "0xe481",
                        Route = "status=1"
                    },
                    new ApplicationSummaryDto
                    {
                        Title = "Paid",
                        Count = paidCount,
                        Icon = "0xe46a",
                        Route = "status=3"
                    },
                };

                return Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}

