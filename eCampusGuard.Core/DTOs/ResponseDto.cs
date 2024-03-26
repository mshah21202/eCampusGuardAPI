using System;
namespace eCampusGuard.Core.DTOs
{
	

	public class ResponseDto
	{
		public ResponseCodeEnum ResponseCode { get; set; }
		public object Message { get; set; }
	}
}

