using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shouldly;

using Xenial.Framework.DevTools.X2C;

using Xunit;

namespace Xenial.Framework.DevTools.Tests.X2C;

public class X2CEngineTests
{
    [Fact]
    public void DefaultDetailViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<DetailView Id=""FooBarPersistent_DetailView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildLayout");
    }

    [Fact]
    public void DefaultListViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<ListView Id=""FooBarPersistent_ListView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildColumns");
    }

    [Fact]
    public void DefaultLookupListViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<ListView Id=""FooBarPersistent_LookupListView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildLookupColumns");
    }

    [Fact]
    public void CustomDetailViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<DetailView Id=""FooBarPersistent_Compact_DetailView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildCompactLayout");
    }

    [Fact]
    public void CollapseDetailViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<DetailView Id=""FooBarPersistent_CompactLayout_DetailView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildCompactLayout");
    }

    [Fact]
    public void CustomListViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<ListView Id=""FooBarPersistent_MoreInfo_ListView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildMoreInfoColumns");
    }

    [Fact]
    public void CollapseListViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<ListView Id=""FooBarPersistent_FewColumns_ListView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildFewColumns");
    }

    [Fact]
    public void CustomLookupListViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<ListView Id=""FooBarPersistent_MoreInfo_LookupListView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildMoreInfoLookupColumns");
    }

    [Fact]
    public void CollapseLookupListViewId()
    {
        var result = X2CEngine.ConvertToCode(@"<ListView Id=""FooBarPersistent_MoreInfoColumns_LookupListView""
            ClassName=""HtmlDemoXAFApplication.Module.BusinessObjects.FooBarPersistent"" />");

        result.MethodName.ShouldBe("BuildMoreInfoLookupColumns");
    }
}
