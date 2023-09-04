using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VM.Core.Exceptions
{
    public class RequiredFieldException : Exception
    {
        public RequiredFieldException() { }
        public RequiredFieldException(string message) : base(message) { }
        public RequiredFieldException(string message, Exception innerException) : base(message, innerException)
        { }

        public RequiredFieldException(Type fieldType, object field) : base($"Required field {field} type {fieldType} is missing.")
        {
        }
    }
}
