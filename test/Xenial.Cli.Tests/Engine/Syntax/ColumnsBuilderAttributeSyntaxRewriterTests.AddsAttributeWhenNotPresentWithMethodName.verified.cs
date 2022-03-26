﻿using System;
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
    [ListViewColumnsBuilder(typeof(PositionColumnsBuilder))]
    [ListViewColumnsBuilder(typeof(PositionColumnsBuilder), nameof(PositionColumnsBuilder.BuildCompactColumns))]
    public class Position : BaseObject
    {
    }
}