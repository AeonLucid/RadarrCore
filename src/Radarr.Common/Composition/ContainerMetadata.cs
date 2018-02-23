using System;
using System.Collections.Generic;

namespace Radarr.Common.Composition
{
    public class ContainerMetadata
    {
        public ContainerMetadata(List<Type> loadedTypes)
        {
            LoadedTypes = loadedTypes;
        }

        public List<Type> LoadedTypes { get; }
    }
}
