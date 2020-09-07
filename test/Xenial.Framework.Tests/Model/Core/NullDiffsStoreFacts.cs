using System;

using Shouldly;

using Xenial.Framework.Model.Core;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Model.Core
{
    public static class NullDiffsStoreFacts
    {
        public static void NullDiffsStoreTests() => Describe(nameof(NullDiffsStore), () =>
        {
            var sut = new NullDiffsStore(typeof(NullDiffsStoreFacts).Assembly);

            It("should be readonly",
                () => sut.ReadOnly
            );

            It("should include AssemblyName in Name",
                () => sut.Name.ShouldContain(typeof(NullDiffsStoreFacts).Assembly.FullName)
            );
        });
    }
}
