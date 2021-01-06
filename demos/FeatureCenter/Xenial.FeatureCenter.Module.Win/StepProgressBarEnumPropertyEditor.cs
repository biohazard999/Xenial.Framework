using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;

namespace Xenial.FeatureCenter.Module.Win
{
    public class StepProgressBarEnumPropertyEditor : WinPropertyEditor
    {
        public StepProgressBarEnumPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
            => ControlBindingProperty = nameof(Control.EditValue);

        public override void RefreshDataSource() => base.RefreshDataSource();

        protected override object CreateControlCore() => new XenialStepProgressBar();

        public new XenialStepProgressBar Control => (XenialStepProgressBar)base.Control;
    }

    public class XenialStepProgressBar : StepProgressBar
    {
        public object? EditValue { get; set; }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var info = CalcHitInfo(e.Location);
            if (info.InItem)
            {
                var item = info.Item;
            }
            base.OnMouseClick(e);
        }
    }
}
