using NUnit.Framework;
using Radarr.Common.Test.DiskTests;
using Radarr.Windows.Disk;

namespace Radarr.Runtime.Windows.Test.DiskProviderTests
{
    [TestFixture]
    public class FreeSpaceFixture : FreeSpaceFixtureBase<DiskProvider>
    {
        public FreeSpaceFixture()
        {
            WindowsOnly();
        }
    }
}
