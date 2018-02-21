using System;

namespace Radarr.Common.Exceptions
{
    public abstract class RadarrException : ApplicationException
    {
        protected RadarrException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        protected RadarrException(string message) : base(message)
        {
        }

        protected RadarrException(string message, Exception innerException, params object[] args)
            : base(string.Format(message, args), innerException)
        {
        }

        protected RadarrException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}