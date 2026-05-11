using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Common
{
    public static class Guard
    {
        public static string AgainstNullOrWhiteSpace(
            string? value,
            string parameterName,
            int maxLength = 250)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException($"{parameterName} is required.");
            }

            value = value.Trim();

            if (value.Length > maxLength)
            {
                throw new DomainException($"{parameterName} cannot exceed {maxLength} characters.");
            }

            return value;
        }

        public static Guid AgainstEmptyGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException($"{parameterName} cannot be empty.");
            }

            return value;
        }
    }

}
