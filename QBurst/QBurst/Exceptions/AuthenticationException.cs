using System;
using System.Collections.Generic;
using System.Text;

namespace QBurst.Exceptions
{
    public class AuthenticationException : Exception
    {
        public string Content { get; }

        public AuthenticationException()
        {
        }

        public AuthenticationException(string content)
        {
            Content = content;
        }
    }
}
