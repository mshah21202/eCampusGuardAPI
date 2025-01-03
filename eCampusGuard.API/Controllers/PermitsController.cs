﻿using System;
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
	public class PermitsController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PermitsController(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all permits
        /// </summary>
        /// <returns></returns>
		[HttpGet()]
		public async Task<ActionResult<IEnumerable<PermitDto>>> GetPermits([FromQuery]PaginationParams paginationParams)
		{
            var permits = (await _unitOfWork.Permits.FindAllAsync(
                (p) => true,
                null,
                null,
                "ASC",
                paginationParams.PageSize,
                paginationParams.PageSize * paginationParams.PageNumber)).AsQueryable();

            var totalItems = await _unitOfWork.Permits.CountAsync();

            Response.AddPaginationHeader(paginationParams.PageNumber, paginationParams.PageSize, totalItems, (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize));

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
                        ResponseCode = ResponseCode.Failed,
                        Message = "Could not find area"
                    });
                }

                var permit = new Permit
                {
                    Name = permitDto.Name,
                    Days = permitDto.Days,
                    Price = permitDto.Price,
                    Capacity = permitDto.Capacity,
                    Area = area,
                    Expiry = permitDto.Expiry
                };

                await _unitOfWork.Permits.AddAsync(permit);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Permit created successfully"
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
        public async Task<ActionResult<ResponseDto>> UpdatePermit(int id, CreatePermitDto permitDto)
        {
            try
            {
                var area = await _unitOfWork.Areas.GetByIdAsync(permitDto.AreaId);

                if (area == null)
                {
                    return NotFound(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Failed,
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
                    Area = area,
                    Expiry = permitDto.Expiry
                };

                _unitOfWork.Permits.Update(permit);

                if (await _unitOfWork.CompleteAsync() > 0)
                {
                    return Ok(new ResponseDto
                    {
                        ResponseCode = ResponseCode.Success,
                        Message = "Permit updated successfully"
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
                        ResponseCode = ResponseCode.Success,
                        Message = "Permit deleted successfully"
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

