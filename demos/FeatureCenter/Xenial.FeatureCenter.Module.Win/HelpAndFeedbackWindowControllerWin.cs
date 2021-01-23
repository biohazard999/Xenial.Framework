using System;
using System.Diagnostics;

namespace Xenial.FeatureCenter.Module.Win
{
    public sealed class HelpAndFeedbackWindowControllerWin : HelpAndFeedbackWindowControllerBase
    {
        public HelpAndFeedbackWindowControllerWin()
            => HelpAndFeedbackSimpleAction.Caption = HelpAndFeedbackSimpleAction?.Caption?.Replace("&", "&&"); //To write an ampersand, we need to escape it

        protected override void OpenHelpAndFeedbackLink(string uri)
            => Process.Start(uri);
    }
}
