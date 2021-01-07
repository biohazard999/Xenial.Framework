
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;

using DevExpress.XtraEditors;

using System;
using System.Linq;
using System.Windows.Forms;

namespace Xenial.FeatureCenter.Module.Win
{
    public class StepProgressBarEnumPropertyEditor : WinPropertyEditor
    {
        public StepProgressBarEnumPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
            => ControlBindingProperty = nameof(Control.EditValue);

        public override void RefreshDataSource() => base.RefreshDataSource();

        protected override object CreateControlCore() => new XenialStepProgressBar(MemberInfo);

        public new XenialStepProgressBar Control => (XenialStepProgressBar)base.Control;
    }

    public class XenialStepProgressBar : StepProgressBar
    {
        private object? editValue;
        public object? EditValue
        {
            get => editValue;
            set
            {
                editValue = value;
                if (editValue is not null)
                {
                    var selectedItem = Items.FirstOrDefault(item => editValue.Equals(item.Tag));
                    if (selectedItem is not null)
                    {
                        SelectedItemIndex = Items.IndexOf(selectedItem);
                    }
                }
            }
        }

        public IMemberInfo MemberInfo { get; }

        public XenialStepProgressBar(IMemberInfo memberInfo)

        {
            _ = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
            MemberInfo = memberInfo;

            if (MemberInfo.MemberType.IsEnum)
            {
                foreach (var value in MemberInfo.MemberType.GetEnumValues())
                {
                    Items.Add(new StepProgressBarItem(value.ToString())
                    {
                        Tag = value
                    });
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var info = CalcHitInfo(e.Location);
            if (info.InItem)
            {
                var item = info.Item;
                if (item is not null)
                {
                    Cursor = Cursors.Hand;
                }
            }
            else
            {
                Cursor = Cursors.Default;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var info = CalcHitInfo(e.Location);
            if (info.InItem)
            {
                var item = info.Item;
                if (item is not null)
                {
                    EditValue = item.Tag;
                    var binding = DataBindings[nameof(EditValue)];
                    if (binding is not null)
                    {
                        binding.WriteValue();
                    }
                }
            }
            base.OnMouseClick(e);
        }
    }
}
