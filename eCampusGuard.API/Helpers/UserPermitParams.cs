using System;
using eCampusGuard.Core.Entities;
using static eCampusGuard.Core.Entities.UserPermit;

namespace eCampusGuard.API.Helpers
{
	public class UserPermitParams : PaginationParams
	{
		public enum UserPermitOrderBy
		{
			StudentId = 0,
			PlateNumber = 1,
			Status = 2
		}

		public string StudentId { get; set; }
		public string PlateNumber { get; set; }
		public UserPermitStatus Status { get; set; }
		public UserPermitOrderBy OrderBy { get; set; }
        public string OrderByDirection { get; set; } = "ASC";

        public object OrderByMember(UserPermit userPermit)
        {
            switch (OrderBy)
            {
                case UserPermitOrderBy.PlateNumber:
                    return userPermit.Vehicle.PlateNumber;
                case UserPermitOrderBy.Status:
                    return userPermit.Status;
                default:
                case UserPermitOrderBy.StudentId:
                    return userPermit.UserId;
            }
        }

        public bool Criteria(UserPermit userPermit, bool isAdmin, AppUser user)
        {
            bool studentId = !isAdmin ? userPermit.UserId == user.Id : userPermit.UserId == int.Parse(StudentId);
            bool plateNumber = !isAdmin ? user.Vehicles.Any(v => v.PlateNumber == userPermit.Vehicle.PlateNumber) : userPermit.Vehicle.PlateNumber == PlateNumber;
            bool status = userPermit.Status == Status;

            bool criteria = studentId && plateNumber && status;

            return criteria;
        }
    }
}

