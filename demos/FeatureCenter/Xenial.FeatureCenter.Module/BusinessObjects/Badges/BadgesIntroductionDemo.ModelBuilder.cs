using System;

using DevExpress.ExpressApp.DC;

using Xenial.Framework.ModelBuilders;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Badges
{
    public sealed class BadgesIntroductionDemoModelBuilder : ModelBuilder<BadgesIntroductionDemo>
    {
        public BadgesIntroductionDemoModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasCaption("Badges - Introduction")
                .HasImage("BringToFrontOfText")
                .WithDefaultClassOptions()
                .GenerateNoListViews()
                .IsSingleton(autoCommit: true)
            ;

            this.WithDetailViewLayout(l => new()
            {
                l.TabbedGroup(
                    l.Tab("Introduction", "Text") with
                    {
                        Children = new()
                        {
                            l.PropertyEditor(m => m.Introduction)
                        }
                    },
                    l.Tab("Installation", "ShipmentReceived") with
                    {
                        Children = new()
                        {
                            l.PropertyEditor(m => m.Installation)
                        }
                    }
                )
            });
        }
    }
}
