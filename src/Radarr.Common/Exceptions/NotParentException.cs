namespace Radarr.Common.Exceptions
{
    public class NotParentException : RadarrException
    {
        public NotParentException(string message, params object[] args) : base(message, args)
        {
        }

        public NotParentException(string message) : base(message)
        {
        }
    }
}