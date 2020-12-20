using System;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Tests.ModelBuilders
{
    public class FakePropertyEditor : PropertyEditor
    {
        public FakePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override object CreateControlCore() => throw new NotImplementedException();
        protected override object GetControlValueCore() => throw new NotImplementedException();
        protected override void ReadValueCore() => throw new NotImplementedException();
    }
}
