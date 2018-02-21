using System.Diagnostics;
using Radarr.Common.EnsureThat.Resources;

namespace Radarr.Common.EnsureThat
{
    public static class EnsureNullableValueTypeExtensions
    {
        [DebuggerStepThrough]
        public static Param<T?> IsNotNull<T>(this Param<T?> param) where T : struct
        {
            if (param.Value == null || !param.Value.HasValue)
                throw ExceptionFactory.CreateForParamNullValidation(param.Name, ExceptionMessages.EnsureExtensions_IsNotNull);

            return param;
        }
    }
}