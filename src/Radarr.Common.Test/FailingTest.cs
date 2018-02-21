using FluentAssertions;
using NUnit.Framework;

namespace Radarr.Common.Test
{
    public class FailingTest
    {
        [Test]
        public void failing_test_example()
        {
            true.Should().BeFalse();
        }
    }
}
