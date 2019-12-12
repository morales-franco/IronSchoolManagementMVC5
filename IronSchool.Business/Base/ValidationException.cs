using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronSchool.Business.Base
{
    public class ValidationException : Exception
    {
        
        public enum Codes : int
        {
            InvalidAddress = 100001,
            CannotFindCustomer = 100002,
            IncompatibleAppVersion = 100003,
            InvalidUserNamePassword = 100004,
            InvalidDiscountCode = 100005,
            BackofficeValidation = 100006,
            BackofficeInternalError = 100007
        }

        public int ValidationCode {get; set;}
        public bool IgnoreResourceMessage { get; set; }
        public object[] KeyNames { get; set; }

        public ValidationException(string message)
            : base(message)
        {
            this.IgnoreResourceMessage = false;            
        }

        public ValidationException(string message, params object[] keyNames)
            : base(message)
        {
            this.IgnoreResourceMessage = false;
            this.KeyNames = keyNames;
        }

        public ValidationException(string message, int code)
            : base(message)
        {
            this.ValidationCode = code;
            this.IgnoreResourceMessage = false;
        }

        public ValidationException(string message, int code, bool ignoreResourceMessage)
            : base(message)
        {
            this.ValidationCode = code;
            this.IgnoreResourceMessage = ignoreResourceMessage;
        }

    }
}
