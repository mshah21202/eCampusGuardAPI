using System;
namespace eCampusGuard.Core.DTOs
{
	

	public class ResponseDto
	{
		public ResponseCode ResponseCode { get; set; }
		public object Message { get; set; }
	}
}

