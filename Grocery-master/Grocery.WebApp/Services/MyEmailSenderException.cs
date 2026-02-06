using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Services
{

    public class MyEmailSenderException :ApplicationException
    {
        private const string StandardErrorMessage = "Something went wrong while sending the email";

        public MyEmailSenderException() : base(StandardErrorMessage)
        {

        }

        public MyEmailSenderException(string message) : base(message)
        {

        }

        public MyEmailSenderException(string message, Exception innerexception) : base(message, innerexception)
        {

        }
    }
}
