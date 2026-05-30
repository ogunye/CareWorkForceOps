using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Identity
{
    public sealed record Permission(
     string Code,
     string Name,
     PermissionGroup Group,
     string Description);
}
