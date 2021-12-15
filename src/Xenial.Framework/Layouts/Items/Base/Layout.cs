using System;
using System.Linq;

#pragma warning disable CA1724 //Conflicts with Winforms: Should not conflict in practice
#pragma warning disable CA2227 //Collection fields should not have a setter: By Design

namespace Xenial.Framework.Layouts.Items.Base;

/// <summary>   (Immutable) a layout. </summary>
[XenialCheckLicense]
public partial record Layout : LayoutItem
{
}
