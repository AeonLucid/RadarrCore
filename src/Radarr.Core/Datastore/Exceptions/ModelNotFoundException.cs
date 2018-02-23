using System;
using Radarr.Common.Exceptions;

namespace Radarr.Core.Datastore.Exceptions
{
    public class ModelNotFoundException : RadarrException
    {
        public ModelNotFoundException(Type modelType, int modelId) : base("{0} with ID {1} does not exist", modelType.Name, modelId)
        {

        }
    }
}
