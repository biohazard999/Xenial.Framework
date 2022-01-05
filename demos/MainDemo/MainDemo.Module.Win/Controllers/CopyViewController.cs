using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Acme.Module.Helpers;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace MainDemo.Module.Win.Controllers;

public class CopyViewController : ViewController
{
    public SimpleAction CopyViewSimpleAction { get; }
    public CopyViewController()
    {
        CopyViewSimpleAction = new SimpleAction(this, nameof(CopyViewSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.Tools);
        CopyViewSimpleAction.Execute += CopyViewSimpleAction_Execute;
    }

    private void CopyViewSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        var node = VisualizeNodeHelper.PrintModelNode(View.Model);

        Clipboard.SetText(node, TextDataFormat.Text);
    }
}
