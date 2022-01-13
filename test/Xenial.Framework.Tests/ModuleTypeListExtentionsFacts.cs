using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using Shouldly;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests;

/// <summary>   A module type list extentions facts. </summary>
public static class ModuleTypeListExtentionsFacts
{
    private class TestModule : ModuleBase { }
    private class TestModule1 : ModuleBase { }

    /// <summary>   Module type list extentions tests. </summary>
    public static void ModuleTypeListExtentionsTests() => Describe(nameof(ModuleTypeListExtentions), () =>
    {
        _ = new TestModule(); //Fix CA1812
        _ = new TestModule1(); //Fix CA1812

        static ModuleTypeList CreateModuleTypeList() => new();

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
