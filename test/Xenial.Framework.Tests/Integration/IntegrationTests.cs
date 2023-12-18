using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shouldly;

using Xunit;

namespace Xenial.Framework.Tests.Integration
{
    public class IntegrationTests
    {
        [Fact]
        public void Assert() => true.ShouldBe(true);
    }
}
