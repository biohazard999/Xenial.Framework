using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.ModelBuilders
{
    public class ModelBuilderBasicPropertiesDemoModelBuilder : ModelBuilder<ModelBuilderBasicPropertiesDemo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBuilderBasicPropertiesDemoModelBuilder`1"/> class.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        public ModelBuilderBasicPropertiesDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("ModelBuilders - ModelClass Properties")
                .WithDefaultClassOptions()
                .HasImage("direction1")
                .IsSingleton(autoCommit: true)
            ;
        }
    }
}
