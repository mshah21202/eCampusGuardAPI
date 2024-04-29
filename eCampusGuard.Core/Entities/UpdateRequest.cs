using System;
namespace eCampusGuard.Core.Entities
{
    public enum UpdateRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Denied = 2
    }

    public class UpdateRequest
    {
        public int Id { get; set; }

        public UpdateRequestStatus Status { get; set; } = UpdateRequestStatus.Pending;

        public virtual UserPermit UserPermit { get; set; }

        public virtual Permit NewPermit { get; set; }
        public int NewPermitId { get; set; }

        public virtual Vehicle UpdatedVehicle { get; set; }
        public int UpdatedVehicleId { get; set; }

    }
}

