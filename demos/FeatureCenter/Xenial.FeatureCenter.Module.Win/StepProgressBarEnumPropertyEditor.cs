using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.XtraEditors;

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Xenial.FeatureCenter.Module.Win
{
    public class StepProgressBarEnumPropertyEditor : WinPropertyEditor
    {
        public StepProgressBarEnumPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
            => ControlBindingProperty = nameof(Control.EditValue);

        public override bool IsCaptionVisible => false;

        protected override object CreateControlCore() => new XenialEnumStepProgressBar(MemberInfo, NullText);

        public new XenialEnumStepProgressBar Control => (XenialEnumStepProgressBar)base.Control;
    }

    public class XenialEnumStepProgressBar : StepProgressBar
    {
        private object? editValue;
        public object? EditValue
        {
            get => editValue;
            set
            {
                editValue = value;

                var selectedItem = Items.FirstOrDefault(item => (item.Tag == null && editValue == null) ? true : item.Tag?.Equals(editValue) == true);
                if (selectedItem is not null)
                {
                    SelectedItemIndex = Items.IndexOf(selectedItem);
                }
                else
                {
                    SelectedItemIndex = -1;
                }
            }
        }

        public IMemberInfo MemberInfo { get; }

        public XenialEnumStepProgressBar(IMemberInfo memberInfo, string? nullText)
        {
            _ = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
            MemberInfo = memberInfo;


            var descriptor = new EnumDescriptor(MemberInfo.MemberType);

            foreach (var value in descriptor.Values)
            {
                var caption = descriptor.GetCaption(value);

                if (value == null && !string.IsNullOrEmpty(nullText))
                {
                    caption = nullText;
                }

                var item = new StepProgressBarItem(caption)
                {
                    Tag = value
                };

                var imageInfo = descriptor.GetImageInfo(value);
                ImageOptionsHelper.AssignImage(item.ContentBlock1.ActiveStateImageOptions, imageInfo, new System.Drawing.Size(32, 32));
                ImageOptionsHelper.AssignImage(item.ContentBlock1.InactiveStateImageOptions, imageInfo, new System.Drawing.Size(32, 32));
                var valueName = value != null ? value.ToString() : null;
                if (TryGetDescription(descriptor.EnumType, valueName, out var description))
                {
                    item.ContentBlock2.Description = description;
                }

                Items.Add(item);
            }

            AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            AllowHtmlTextInToolTip = DevExpress.Utils.DefaultBoolean.True;
            ContentAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            DistanceBetweenContentBlockElements = 2;
            ItemOptions.Indicator.ActiveStateImageOptions.SvgImageSize = new System.Drawing.Size(14, 14);
            ItemOptions.Indicator.InactiveStateDrawMode = IndicatorDrawMode.Outline;
            ItemOptions.Indicator.Width = 24;
        }

        private static bool TryGetDescription(Type enumType, string? valueName, out string? description)
        {
            //TODO: Localization
            if (valueName is not null)
            {
                var info = enumType.GetField(valueName);
                if (info is not null)
                {
                    var attr = AttributeHelper.GetAttributes<DXDescriptionAttribute>(info, false);
                    if (attr.Length == 1)
                    {
                        description = attr[0].Description;
                        return true;
                    }
                }
            }
            description = null;
            return false;
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
