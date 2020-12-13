using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;
using Xenial.Framework.WebView.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    public class WebViewEditorDemoModelBuilder : ModelBuilder<WebViewEditorDemo>
    {
        public WebViewEditorDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            For(m => m.Uri)
                .UsingWebViewPropertyEditor();
        }
    }
}
