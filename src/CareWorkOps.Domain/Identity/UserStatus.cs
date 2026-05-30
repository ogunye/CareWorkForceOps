using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Identity
{
    public enum UserStatus
    {
        Active = 1, 
        Inactive = 2,
        Suspended = 3,
        PendingInvitation = 4,
        Archived = 5,
    }
}
