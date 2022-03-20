using System;
using System.Collections.Generic;

using DevExpress.Xpo;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using Xenial.Framework.Layouts;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty(nameof(Position.Title))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder), ViewId = "Position_Complex_DetailView")]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder))]
    public class Position : BaseObject
    {
    }
}
