using System;
using Radarr.Core.Datastore.Main.Models;

namespace Radarr.Core.ProgressMessaging
{
    public static class ProgressMessageContext
    {
        [ThreadStatic]
        private static CommandModel _commandModel;

        [ThreadStatic]
        private static bool _reentrancyLock;

        public static CommandModel CommandModel
        {
            get => _commandModel;
            set => _commandModel = value;
        }

        public static bool LockReentrancy()
        {
            if (_reentrancyLock)
                return false;

            _reentrancyLock = true;
            return true;
        }

        public static void UnlockReentrancy()
        {
            _reentrancyLock = false;
        }
    }
}
