﻿using System;
using Radarr.Common.Exceptions;

namespace Radarr.Core.Messaging.Commands.Exceptions
{
    public class CommandFailedException : RadarrException
    {
        public CommandFailedException(string message, params object[] args) : base(message, args)
        {
        }

        public CommandFailedException(string message) : base(message)
        {
        }

        public CommandFailedException(string message, Exception innerException, params object[] args) : base(message, innerException, args)
        {
        }

        public CommandFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CommandFailedException(Exception innerException) : base("Failed", innerException)
        {
        }
    }
}
