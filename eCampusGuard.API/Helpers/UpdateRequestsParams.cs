using System;
using System.Linq.Expressions;
using eCampusGuard.Core.Entities;
using LinqKit;

namespace eCampusGuard.API.Helpers
{
	public class UpdateRequestsParams : PaginationParams
	{
        public string? StudentId { get; set; }
        public string? PlateNumber { get; set; }
        public int? PermitId { get; set; }
        public UpdateRequestStatus? Status { get; set; }

        public Expression<Func<UpdateRequest, bool>> Criteria(bool isAdmin, AppUser user)
        {
            var predicate = PredicateBuilder.New<UpdateRequest>();

            if (StudentId != null)
            {
                predicate.And((p) => !isAdmin ? p.UserPermit.User.UserName == user.UserName : p.UserPermit.User.UserName == StudentId);
            }
            else if (!isAdmin)
            {
                predicate.And((p) => p.UserPermit.User.UserName == user.UserName);
            }

            if (PermitId != null)
            {
                predicate.And((p) => p.UserPermit.PermitId == PermitId);
            }

            if (Status != null)
            {
                predicate.And((p) => p.Status == Status);
            }

            if (PlateNumber != null)
            {
                predicate.And((p) => p.UserPermit.Vehicle.PlateNumber == PlateNumber);
            }

            if (Status == null && PermitId == null && StudentId == null && PlateNumber == null)
            {
                predicate.Or((p) => true);
            }
            //(p) =>
            //(!isAdmin ? p.UserId == user.Id : StudentId != null ? (p.UserId == int.Parse(StudentId)) : true) &&
            //(!isAdmin ? p.User.Name == user.Name : p.User.Name == Name) &&
            //(p.Year == Year) &&
            //(p.PermitId == PermitId) &&
            //(p.Status == Status);

            return predicate;
        }
    }
}

