using System;
using eCampusGuard.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	public class TestController : BaseApiController
	{
        private readonly IUnitOfWork _unitOfWork;

        public TestController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet()]
        public async Task Test()
        {
        }
    }
}

