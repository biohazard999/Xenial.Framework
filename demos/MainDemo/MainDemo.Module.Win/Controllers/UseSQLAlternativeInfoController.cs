using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Utils;

namespace Demos.Data.Win
{
    public class UseSQLAlternativeInfoController : ObjectViewController<DetailView, UseSQLAlternativeInfo>
    {
        public UseSQLAlternativeInfoController() : base() { }
        protected override void OnActivated()
        {
            base.OnActivated();
            var sqlIssueItem = (StaticTextViewItem)View.FindItem("SQLIssueText");
            ((IHtmlFormattingSupport)sqlIssueItem).SetHtmlFormattingEnabled(true);
            sqlIssueItem.Text = string.Format("<b><size=+2>{0}</size></b>", ViewCurrentObject.SQLIssue);

            var alternativeItem = (StaticTextViewItem)View.FindItem("AlternativeStaticText");
            ((IHtmlFormattingSupport)alternativeItem).SetHtmlFormattingEnabled(true);
            alternativeItem.Text = string.Format("<b>{0}</b> will be used instead.", ViewCurrentObject.Alternative);

            var noteItem = (StaticTextViewItem)View.FindItem("NoteStaticText");
            ((IHtmlFormattingSupport)noteItem).SetHtmlFormattingEnabled(true);
            noteItem.Text = string.Format("<b>Note:</b> {0}", ViewCurrentObject.Restrictions);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if ((Frame != null) && (Frame.Template != null) && (Frame.Template is PopupForm))
            {
                var popupForm = (PopupForm)Frame.Template;
                popupForm.CustomizeClientSize += (s, e) =>
                {
                    popupForm.ButtonsContainer.Padding = ScaleUtils.ScalePadding(new Padding(12, 12, 12, 12));
                    var formPadding = popupForm.Padding;
                    popupForm.Padding = ScaleUtils.ScalePadding(new Padding(formPadding.Left, formPadding.Top, formPadding.Right, 12));
                    e.CustomSize = ScaleUtils.GetScaleSize(new System.Drawing.Size(480, 220));
                    e.Handled = true;
                };

            }
        }
    }
}
