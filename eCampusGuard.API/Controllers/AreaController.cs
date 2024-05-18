using System;
using System.Net.WebSockets;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using eCampusGuard.API.Extensions;
using eCampusGuard.API.Helpers;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.MSSQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace eCampusGuard.API.Controllers
{
    [Authorize]
    public class AreaController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHubContext<AreaHub> _hubContext;

        public AreaController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IHubContext<AreaHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AreaDto>>> GetAreas([FromQuery] PaginationParams paginationParams)
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
                var area = await _unitOfWork.Areas.GetByIdAsync(id);

                if (area == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Area not found"
                    });
                }

                area.Capacity = areaDto.Capacity;
                area.Name = areaDto.Name;
                area.Gate = areaDto.Gate;

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
                    Message = e.Message
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
                    Message = e.Message
                });
            }

            return BadRequest(new ResponseDto
            {
                ResponseCode = ResponseCode.Failed,
                Message = "Something went wrong"
            });
        }


        [Authorize(Policy = "RequireGateStaffRole")]
        [HttpGet("details/{id}")]
        public async Task<ActionResult<AreaScreenDto>> GetAreaDetails(int id)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(id);

            var result = _mapper.Map<AreaScreenDto>(area);

            return Ok(result);
        }

        [Authorize(Policy = "RequireGateStaffRole")]
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<AreaScreenDto>>> GetAreasDetails()
        {
            try
            {
                var areas = (await _unitOfWork.Areas.GetAllAsync()).AsQueryable();

                return Ok(areas.ProjectTo<AreaScreenDto>(_mapper.ConfigurationProvider).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// The ANPLR posts the license plate to create an access log, etc.
        /// </summary>
        /// <param name="id">Area Id</param>
        /// <param name="anplrResult"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireGateStaffRole")]
        [HttpPost("details/anplr/{id}")]
        public async Task<ActionResult<AnplrResultDto>> AnplrHandler(int id, AnplrDto anplrDto)
        {
            try
            {
                var userpermit = await _unitOfWork.UserPermits.FindAsync(up => up.Vehicle.PlateNumber == anplrDto.PlateNumber);

                if (userpermit == null)
                {
                    return BadRequest(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find user permit details"
                    });
                }

                var area = await _unitOfWork.Areas.GetByIdAsync(id);

                if (area == null)
                {
                    return BadRequest(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find area"
                    });
                }

                var allowedToEnter = userpermit.Permit.AreaId == area.Id && userpermit.IsPermitValid() && userpermit.Status != UserPermitStatus.Withdrawn;

                var anplrResult = new AnplrResultDto
                {
                    PlateNumber = userpermit.Vehicle.PlateNumber,
                    AllowedToEnter = allowedToEnter,
                    Days = userpermit.Permit.Days,
                    PermitName = userpermit.Permit.Name,
                    Status = userpermit.Status,
                };

                await _hubContext.Clients.Group(id.ToString()).SendAsync("ReceiveAnplrResult", anplrResult);

                if (userpermit.Permit.AreaId == area.Id)
                {
                    if (userpermit.AccessLogs.Count() > 0)
                    {
                        if (userpermit.AccessLogs.OrderBy(al => al.Id).Last().Type == (anplrDto.Entry ? AccessLogType.Entry : AccessLogType.Exit))
                        {
                            return BadRequest(new ResponseDto
                            {
                                ResponseCode = ResponseCode.Failed,
                                Message = $"Invalid operation. User is {(anplrDto.Entry ? "already inside this area." : "already outside the area.")}"
                            });
                        }
                    } else if (!anplrDto.Entry)
                    {
                        return BadRequest(new ResponseDto
                        {
                            ResponseCode = ResponseCode.Failed,
                            Message = "Invalid operation. User is already outside the area."
                        });
                    }

                    var accessLog = new AccessLog
                    {
                        Area = area,
                        UserPermit = userpermit,
                        Type = anplrDto.Entry ? AccessLogType.Entry : AccessLogType.Exit,
                        Timestamp = DateTime.Now
                    };

                    await _unitOfWork.AccessLogs.AddAsync(accessLog);

                    if (await _unitOfWork.CompleteAsync() > 0)
                    {
                        return Ok(new ResponseDto
                        {
                            ResponseCode = ResponseCode.Success,
                            Message = "Successfully logged access."
                        });
                    }
                }
                else
                {
                    return BadRequest(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "User permit does not belong to this area"
                    });
                }


                return BadRequest(new ResponseDto
                {
                    ResponseCode = ResponseCode.Failed,
                    Message = "Something went wrong."
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
        /// Registers the camera stream url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireGateStaffRole")]
        [HttpPost("details/anplr/stream/{id}")]
        public async Task<ActionResult> RegisterCameraUrl(int id, [FromQuery]string url, [FromQuery]bool entry)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(id);

            if (area == null)
            {
                return NotFound("Area with id:" + id.ToString() + "could not be found");
            }

            if (entry)
            {
                area.EntryCameraStreamUrl = url;
            } else
            {
                area.ExitCameraStreamUrl = url;
            }

            _unitOfWork.Areas.Update(area);

            if (await _unitOfWork.CompleteAsync() > 0)
            {
                return Ok("Successfully registered camera url");
            }

            return BadRequest("Something went wrong");
        }
    }
}

