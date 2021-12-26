using System.Collections.Generic;
using System.Configuration;

using DevExpress.DXperience.Demos;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;

namespace Demos.Feedback
{
    public class XAFFeedbackHelper
    {
        private static readonly HashSet<string> openedViews = new HashSet<string>();
        private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e)
        {
            if (e.Context == TemplateContext.ApplicationWindow)
            {
                if (e.Template is IDockManagerHolder)
                {
                    ((IDockManagerHolder)e.Template).DockManager.TopZIndexControls.Add("DevExpress.DXperience.Demos.XafFeedbackPanelControl");
                }
                var feedbackPanel = FeedbackHelper.AddFeedbackPanelToXtraFrom(e.Template as XtraForm, (string)ConfigurationManager.AppSettings["FeedbackDescription"]);
                if (feedbackPanel != null)
                {
                    feedbackPanel.PostFeedback += (s, args) =>
                    {
                        FeedbackHelper.PostFeedbackAsync(new FeedbackObject() { ModuleName = (string)ConfigurationManager.AppSettings["FeedbackDemoName"], Feedback = GetOpenedViews() + args.Feedback, Value = args.Value, Email = string.Empty });
                        openedViews.Clear();
                    };
                }
            }
        }
        private string GetOpenedViews()
        {
            var result = "Opened Views \n";
            foreach (var view in openedViews)
            {
                result += view + "\n";
            }
            return result;
        }
        private void Application_ViewCreated(object sender, ViewCreatedEventArgs e) => openedViews.Add(e.View.Id);
        public XAFFeedbackHelper(XafApplication application)
        {
            application.CustomizeTemplate += Application_CustomizeTemplate;
            application.ViewCreated += Application_ViewCreated;
        }
    }
}
