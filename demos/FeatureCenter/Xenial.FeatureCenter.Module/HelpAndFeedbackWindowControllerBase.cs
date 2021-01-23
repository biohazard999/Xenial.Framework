using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace Xenial.FeatureCenter.Module
{
    public abstract class HelpAndFeedbackWindowControllerBase : WindowController
    {
        public SimpleAction HelpAndFeedbackSimpleAction { get; }

        public HelpAndFeedbackWindowControllerBase()
        {
            TargetWindowType = WindowType.Main;
            HelpAndFeedbackSimpleAction = new SimpleAction(this, nameof(HelpAndFeedbackSimpleAction), PredefinedCategory.RecordsNavigation)
            {
                Caption = "Help & Feedback",
                ImageName = "Actions_Question",
                PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
            };

            HelpAndFeedbackSimpleAction.Execute += HelpAndFeedbackSimpleAction_Execute;
        }

        private void HelpAndFeedbackSimpleAction_Execute(object? sender, SimpleActionExecuteEventArgs e)
            => OpenHelpAndFeedbackLink("https://github.com/xenial-io/Xenial.Framework/issues");

        protected abstract void OpenHelpAndFeedbackLink(string uri);
    }
}
