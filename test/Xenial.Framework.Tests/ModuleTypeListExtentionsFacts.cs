using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using Shouldly;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests
{
    public static class ModuleTypeListExtentionsFacts
    {
        public class TestModule : ModuleBase { }
        public class TestModule1 : ModuleBase { }

        public static void ModuleTypeListExtentionsTests() => Describe(nameof(ModuleTypeListExtentions), () =>
        {
            ModuleTypeList CreateModuleTypeList() => new ModuleTypeList();

            It("should add a single type", () =>
            {
                var moduleTypeList = CreateModuleTypeList();
                moduleTypeList.AndModuleTypes(new[] { typeof(TestModule) });
                moduleTypeList.Count.ShouldBe(1);
            });

            It("should add only unique types", () =>
            {
                var moduleTypeList = CreateModuleTypeList();
                moduleTypeList.AndModuleTypes(new[] { typeof(TestModule), typeof(TestModule) });
                moduleTypeList.Count.ShouldBe(1);
            });

            It("should add multiple types", () =>
            {
                var moduleTypeList = CreateModuleTypeList();
                moduleTypeList.AndModuleTypes(new[] { typeof(TestModule), typeof(TestModule1) });
                moduleTypeList.Count.ShouldBe(2);
            });
        });
    }
}
