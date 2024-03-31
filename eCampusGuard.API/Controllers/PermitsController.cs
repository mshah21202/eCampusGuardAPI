using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	[Authorize]
	public class PermitsController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public PermitsController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets all permits
        /// </summary>
        /// <returns></returns>
		[HttpGet()]
		public async Task<ActionResult<IEnumerable<PermitDto>>> GetPermits()
		{
            var permits = (await _unitOfWork.Permits.GetAllAsync()).AsQueryable();

            return Ok(permits.ProjectTo<PermitDto>(_mapper.ConfigurationProvider));
		}

        /// <summary>
        /// Gets a permit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PermitDto>> GetPermit(int id)
        {
            return Ok(_mapper.Map<PermitDto>(await _unitOfWork.Permits.GetByIdAsync(id)));
        }

        [Authorize(Policy = "RequireAdminRole")]
		[HttpPost()]
		public async Task<ActionResult<ResponseDto>> CreatePermit(CreatePermitDto permitDto)
		{
            try
            {
                var area = await _unitOfWork.Areas.GetByIdAsync(permitDto.AreaId);

                if (area == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find area"
                    });
                }

                var permit = new Permit
                {
                    Name = permitDto.Name,
                    Days = permitDto.Days,
                    Price = permitDto.Price,
                    Capacity = permitDto.Capacity,
                    Area = area
                };

                await _unitOfWork.Permits.AddAsync(permit);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Success,
                        Message = "Permit created successfully"
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

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("{id}")]
        public async Task<ActionResult<ResponseDto>> UpdatePermit(int id, CreatePermitDto permitDto)
        {
            try
            {
                var area = await _unitOfWork.Areas.GetByIdAsync(permitDto.AreaId);

                if (area == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Failed,
                        Message = "Could not find area"
                    });
                }

                var permit = new Permit
                {
                    Id = id,
                    Name = permitDto.Name,
                    Days = permitDto.Days,
                    Price = permitDto.Price,
                    Capacity = permitDto.Capacity,
                    Area = area
                };

                _unitOfWork.Permits.Update(permit);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Success,
                        Message = "Permit updated successfully"
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

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto>> DeletePermit(int id)
		{
            try
            {
                var permit = await _unitOfWork.Permits.GetByIdAsync(id);

                if (permit == null)
                {
                    return NotFound();
                }

                _unitOfWork.Permits.Delete(permit);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCodeEnum.Success,
                        Message = "Permit deleted successfully"
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

