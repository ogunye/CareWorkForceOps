using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Common
{
    public sealed class DomainException : Exception
    {
        public DomainException(string message)
            : base(message)
        {
        }
    }
}
