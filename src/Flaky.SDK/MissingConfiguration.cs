using System;
using System.Runtime.Serialization;

namespace Flaky.SDK
{
    [Serializable]
    internal class MissingConfiguration : Exception
    {
        public MissingConfiguration()
        {
        }

        public MissingConfiguration(string message) : base(message)
        {
        }

        public MissingConfiguration(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingConfiguration(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}