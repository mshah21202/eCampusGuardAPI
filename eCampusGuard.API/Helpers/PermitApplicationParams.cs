using System;
using System.Linq.Expressions;
using eCampusGuard.Core.Entities;
using LinqKit;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.API.Helpers
{
	

	public class PermitApplicationParams : PaginationParams
	{
		public string? StudentId { get; set; }
		public string? Name { get; set; }
		public AcademicYear? Year { get; set; }
		public int? PermitId { get; set; }
		public PermitApplicationStatus? Status { get; set; }
		public PermitApplicationOrderBy OrderBy { get; set; } = PermitApplicationOrderBy.Status;
		public string OrderByDirection { get; set; } = "ASC";

		public Expression<Func<PermitApplication, object>> OrderByMember()
		{
			switch (OrderBy)
			{
				case PermitApplicationOrderBy.Name:
					return (p) => p.User.Name;
				case PermitApplicationOrderBy.AcademicYear:
					return (p) => p.Year;
				case PermitApplicationOrderBy.PermitType:
					return (p) => p.Permit.Name;
				case PermitApplicationOrderBy.Status:
					return (p) => p.Status;
				default:
				case PermitApplicationOrderBy.StudentId:
					return (p) => p.Id;
			}
		}

		public Expression<Func<PermitApplication, bool>> Criteria(bool isAdmin, AppUser user)
		{
			//Expression<Func<PermitApplication, bool>> criteria = (p) => true;
			var predicate = PredicateBuilder.New<PermitApplication>();
			if (StudentId != null)
			{
				predicate.And((p) => !isAdmin ? p.User.UserName == user.UserName : p.User.UserName == StudentId);
			} else if (!isAdmin)
			{
				predicate.And((p) => p.User.UserName == user.UserName);
			}

			if (Name != null)
			{
				predicate.And((p) => !isAdmin ? p.User.Name == user.Name : p.User.Name == Name);
			} else if (!isAdmin)
			{
                predicate.And((p) => p.User.Name == user.Name);
            }

            if (Year != null)
			{
				predicate.And((p) => p.Year == Year);
			}

			if (PermitId != null)
			{
				predicate.And((p) => p.PermitId == PermitId);
			}

			if (Status != null)
			{
				predicate.And((p) => p.Status == Status);
			}

			if (Status == null && PermitId == null && Year == null && Name == null && StudentId == null)
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

