using DevExpress.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Layouts;

namespace MailClient.Module.BusinessObjects
{
    public class MailModelBuilder : ModelBuilder<Mail>
    {
        public MailModelBuilder(ITypeInfo typeInfo) : base(typeInfo) { }

        public override void Build()
        {
            base.Build();

            this.HasNavigationItem("Mail - Mails")
                .HasImage("Glyph_Mail")
                .HasCaption("Mail")
                .WithDefaultListViewOptions(MasterDetailMode.ListViewAndDetailView)
                .GenerateNoLookupListView()
                .WithAttribute(new DetailViewLayoutBuilderAttribute(typeof(MailLayoutBuilder)))
            ;

            ForAllProperties()
                .NotAllowingEdit();

            ForPropertiesOfType<DateTime>()
                .HasDisplayFormat("{0:G}");

            ForProperties(
                m => m.CC,
                m => m.BCC,
                m => m.FromAll,
                m => m.ToAll
            ).UseTokenStringPropertyEditor();

            this.WithListViewColumns(b => new()
            {
                b.Column(m => m.MessageDateTime) with
                {
                    Caption = "Date",
                    SortOrder = ColumnSortOrder.Descending,
                    Width = 1
                },
                b.Column(m => m.From) with
                {
                    Width = 9
                },
                b.Column(m => m.Subject) with
                {
                    Width = 90
                }
            });
        }
    }
}
