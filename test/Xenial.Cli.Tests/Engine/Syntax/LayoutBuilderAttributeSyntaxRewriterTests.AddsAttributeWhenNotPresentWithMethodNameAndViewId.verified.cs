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
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder))]
    [DetailViewLayoutBuilder(typeof(PositionLayoutBuilder), nameof(PositionLayoutBuilder.BuildCompactLayout), ViewId = "Position_Compact_DetailView")]
    public class Position : BaseObject
    {
    }
}
