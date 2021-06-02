using System;
using System.Runtime.Serialization;

namespace VueCore.Models
{
    public class MediaException : Exception
    {
        public MediaException(string message) : base(message)
        {
        }

        public MediaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MediaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}