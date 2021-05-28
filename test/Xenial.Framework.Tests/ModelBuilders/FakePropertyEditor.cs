using System;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Tests.ModelBuilders
{
    /// <summary>   Editor for fake property. </summary>
    ///
    /// <seealso cref="PropertyEditor"/>

    public class FakePropertyEditor : PropertyEditor
    {
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="objectType">   Type of the object. </param>
        /// <param name="model">        The model. </param>

        public FakePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        /// <summary>   Creates control core. </summary>
        ///
        /// <returns>   The new control core. </returns>

        protected override object CreateControlCore() => throw new NotImplementedException();

        /// <summary>   Gets control value core. </summary>
        ///
        /// <returns>   The control value core. </returns>

        protected override object GetControlValueCore() => throw new NotImplementedException();
        /// <summary>   Reads value core. </summary>
        protected override void ReadValueCore() => throw new NotImplementedException();
    }
}
