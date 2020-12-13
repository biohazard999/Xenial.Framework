using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using DevExpress.ExpressApp.DC;

using Xenial.Utils;
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

            this.HasCaption("Editors - WebView")
                .WithDefaultClassOptions()
                .IsSingleton()
                .HasImage("Business_World");

            this.ForX(m => m.UrlString)
                .HasCaption("Foo");

            this.ForX(m => m.Uri)
                .HasCaption("Foo");
        }
    }

    public static class Foo
    {
        /// <summary>
        /// Fors the specified property.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property.</param>
        /// <returns></returns>
        public static PropertyBuilder<TPropertyType?, TClassType> ForX<TPropertyType, TClassType>(this IModelBuilder<TClassType> builder, Expression<Func<TClassType, TPropertyType?>> propertyExpression)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));

            var propertyName = ExpressionHelper.Create<TClassType>().Property(propertyExpression);
            if (propertyName is not null)
            {
                var memberInfo = builder.TypeInfo.FindMember(propertyName);
                if (memberInfo is not null)
                {
                    var propertyBuilder = PropertyBuilder.PropertyBuilderFor<TPropertyType?, TClassType>(memberInfo);

                    if (builder is IBuilderManager b)
                    {
                        b.Add(propertyBuilder);
                    }
                    return propertyBuilder;
                }
            }

            throw new InvalidOperationException("Could not find Property");
        }
    }
}
