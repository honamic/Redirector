using System;

namespace Honamic.Redirector
{
    public class RedirectorException : Exception
    {
        public RedirectorException(string message, Exception ex = null) : base(message, ex)
        {

        }
    }
}
