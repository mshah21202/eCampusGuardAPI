using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	public class AreaController : BaseApiController
	{
		public AreaController()
		{
		}

		[HttpGet()]
		public async Task<IEnumerable<AreaDto>> GetAreas()
		{
			return new List<AreaDto>();
		}

		[HttpPost()]
		public async Task<ResponseDto> CreateArea(AreaDto areaDto)
		{
			return new ResponseDto();
		}

        [HttpPost("{id}")]
        public async Task<ResponseDto> UpdateArea(int id, AreaDto areaDto)
        {
            return new ResponseDto();
        }

        [HttpGet("{id}")]
		public async Task<AreaDto> GetArea(int id)
		{
			return new AreaDto();
		}

        [HttpGet("details/{id}")]
        public async Task<AreaScreenDto> GetAreaDetails(int id)
		{
			return new AreaScreenDto();
		}

        [HttpDelete("{id}")]
        public async Task<ResponseDto> DeleteArea(int id)
        {
            return new ResponseDto();
        }
    }
}

