using System;
using Radarr.Common.Exceptions;

namespace Radarr.Core.Configuration.Exceptions
{
    public class InvalidConfigFileException : RadarrException
    {
        public InvalidConfigFileException(string message) : base(message)
        {
        }

        public InvalidConfigFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
