using System;
using System.Linq.Expressions;
using eCampusGuard.Core.Entities;
using LinqKit;
using static eCampusGuard.Core.Entities.UserPermit;

namespace eCampusGuard.API.Helpers
{
	public class UserPermitParams : PaginationParams
	{
		

		public string? StudentId { get; set; }
		public string? PlateNumber { get; set; }
		public int? PermitId { get; set; }
		public UserPermitStatus? Status { get; set; }
        public UserPermitOrderBy? OrderBy { get; set; }
        public string OrderByDirection { get; set; } = "ASC";

        public Expression<Func<UserPermit, object>> OrderByMember()
        {
            switch (OrderBy)
            {
                case UserPermitOrderBy.PlateNumber:
                    return (userPermit) => userPermit.Vehicle.PlateNumber;
                case UserPermitOrderBy.Status:
                    return (userPermit) => userPermit.Status;
                default:
                case UserPermitOrderBy.StudentId:
                    return (userPermit) => userPermit.UserId;
            }
        }

        public Expression<Func<UserPermit, bool>> Criteria(bool isAdmin, AppUser user)
        {
            var predicate = PredicateBuilder.New<UserPermit>();
            if (StudentId != null && isAdmin)
            {
                predicate.And((p) => p.User.UserName == StudentId);
            }
            else if (!isAdmin)
            {
                predicate.And((p) => p.User.UserName == user.UserName);
            }

            if (PermitId != null)
            {
                predicate.And((p) => p.Permit.Id == PermitId);
            }

            if (PlateNumber != null && isAdmin)
            {
                predicate.And((p) => p.Vehicle.PlateNumber == PlateNumber);
            }

            if (Status != null)
            {
                predicate.And((p) => p.Status == Status);
            }

            if (isAdmin && Status == null && StudentId == null && PlateNumber == null && PermitId == null)
            {
                predicate.Or((p) => true);
            }

            return predicate;
        }
    }
}

