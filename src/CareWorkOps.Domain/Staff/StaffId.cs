using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Staff;

public readonly record struct StaffId(Guid Value)
{
    public static StaffId New()
    {
        return new StaffId(Guid.NewGuid());
    }

    public static StaffId From(Guid value)
    {
        Guard.AgainstEmptyGuid(value, nameof(StaffId));
        return new StaffId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}