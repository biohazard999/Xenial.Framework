using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;

namespace Xenial.Framework.Lab.Win;

/// <summary>   A controller for handling extended layouts. </summary>
///
/// <seealso cref="ViewController{DetailView}"/>

public class ExtendedLayoutController : ViewController<DetailView>
{
    private readonly List<System.Windows.Forms.Control> controls = new();

    /// <summary>   Executes the 'activated' action. </summary>
    protected override void OnActivated()
    {
        base.OnActivated();

        View.CustomizeViewItemControl<DXPropertyEditor>(this, viewItem =>
        {
            if (viewItem.Control.Properties is RepositoryItemTextEdit r)
            {
                r.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.True;
                r.AdvancedModeOptions.Label = viewItem.Caption;
                if (View.IsControlCreated && View.Control is XafLayoutControl xafLayoutControl)
                {
                    CustomizeView(xafLayoutControl, viewItem.Control);
                }
                else
                {
                    controls.Add(viewItem.Control);
                }
            }
        });

        if (View.IsControlCreated && View.Control is XafLayoutControl xafLayoutControl)
        {
            CustomizeView(xafLayoutControl);
        }
        else if (!View.IsControlCreated)
        {
            View.ControlsCreated -= View_ControlsCreated;
            View.ControlsCreated += View_ControlsCreated;
            void View_ControlsCreated(object? sender, EventArgs? e)
            {
                View.ControlsCreated -= View_ControlsCreated;
                if (View.IsControlCreated && View.Control is XafLayoutControl xafLayoutControl)
                {
                    CustomizeView(xafLayoutControl);
                }
            }
        }
    }

    private void CustomizeView(XafLayoutControl xafLayoutControl)
    {
        foreach (var control in controls)
        {
            CustomizeView(xafLayoutControl, control);
        }
    }

    private static void CustomizeView(XafLayoutControl xafLayoutControl, System.Windows.Forms.Control control)
    {
        var item = xafLayoutControl.Items.OfType<LayoutControlItem>().FirstOrDefault(m => m.Control == control);
        if (item is not null)
        {
            item.TextVisible = false;
        }
    }
}
