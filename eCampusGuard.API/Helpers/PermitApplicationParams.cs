using System;
using eCampusGuard.Core.Entities;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.API.Helpers
{
	public enum PermitApplicationOrderBy
	{
		StudentId = 0,
		Name = 1,
		AcademicYear = 2,
		PermitType = 3,
		Status = 4
	}

	public class PermitApplicationParams : PaginationParams
	{
		public string StudentId { get; set; }
		public string Name { get; set; }
		public int AcademicYear { get; set; }
		public int PermitId { get; set; }
		public int Status { get; set; }
		public PermitApplicationOrderBy OrderBy { get; set; } = PermitApplicationOrderBy.StudentId;
		public string OrderByDirection { get; set; } = "ASC";

		public object OrderByMember(PermitApplication permitApplication)
		{
			switch (OrderBy)
			{
				case PermitApplicationOrderBy.Name:
					return permitApplication.User.Name;
				case PermitApplicationOrderBy.AcademicYear:
					return permitApplication.AcademicYear;
				case PermitApplicationOrderBy.PermitType:
					return permitApplication.Permit.Name;
				case PermitApplicationOrderBy.Status:
					return permitApplication.Status;
				default:
				case PermitApplicationOrderBy.StudentId:
					return permitApplication.Id;
			}
		}

		public bool Criteria(PermitApplication permitApplication, bool isAdmin, AppUser user)
		{
			bool userId = !isAdmin ? permitApplication.UserId == user.Id : permitApplication.UserId == int.Parse(StudentId);
			bool name = !isAdmin ? permitApplication.User.Name == user.Name : permitApplication.User.Name == Name;
			bool academicYear = permitApplication.AcademicYear == (AcademicYearEnum)AcademicYear;
			bool permitId = permitApplication.PermitId == PermitId;
			bool status = permitApplication.Status == (PermitApplicationStatusEnum)Status;

            bool criteria = userId && name && academicYear && permitId && status;

			return criteria;
		}
	}
}

