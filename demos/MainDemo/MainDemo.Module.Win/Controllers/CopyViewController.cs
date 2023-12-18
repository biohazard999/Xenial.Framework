using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Acme.Module.Helpers;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;

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
        if (true)
        {
            var node = VisualizeNodeHelper.PrintModelNode(View.Model);
            Clipboard.SetText(node, TextDataFormat.Text);
        }
        else
        {
            var id = String.Format("{0}_{1}", View.Model.Id, Guid.NewGuid());
            var modelViews = View.Model.Application.Views;
            var copy = (modelViews as DevExpress.ExpressApp.Model.Core.ModelNode).AddClonedNode((DevExpress.ExpressApp.Model.Core.ModelNode)View.Model, id);
            var node = VisualizeNodeHelper.PrintModelNode(copy);
            node = node.Replace(id, View.Model.Id); //Patch ViewId
            Clipboard.SetText(node, TextDataFormat.Text);
            (copy as IModelView).Remove();
        }
    }
}
