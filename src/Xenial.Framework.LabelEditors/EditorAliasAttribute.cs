﻿using System;

using Xenial;
using Xenial.Framework.LabelEditors.PubTernal;

namespace DevExpress.Persistent.Base;

/// <summary>
/// Class LabelStringEditorAttribute.
/// Implements the <see cref="DevExpress.Persistent.Base.EditorAliasAttribute" />
/// </summary>
/// <seealso cref="DevExpress.Persistent.Base.EditorAliasAttribute" />
/// <autogeneratedoc />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
[XenialCheckLicense]
public sealed partial class LabelStringEditorAttribute : EditorAliasAttribute
{
    /// <summary>
    /// <para>Initializes a new instance of the <see cref="LabelStringEditorAttribute"/>
    /// class.</para>
    /// </summary>

    public LabelStringEditorAttribute() : base(LabelEditorAliases.LabelStringPropertyEditor) { }
}


/// <summary>
/// Class LabelHyperlinkStringEditorAttribute.
/// Implements the <see cref="DevExpress.Persistent.Base.EditorAliasAttribute" />
/// </summary>
/// <seealso cref="DevExpress.Persistent.Base.EditorAliasAttribute" />
/// <autogeneratedoc />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
[XenialCheckLicense]
public sealed partial class LabelHyperlinkStringEditorAttribute : EditorAliasAttribute
{
    /// <summary>
    /// <para>Initializes a new instance of the <see cref="LabelStringEditorAttribute"/>
    /// class.</para>
    /// </summary>

    public LabelHyperlinkStringEditorAttribute() : base(LabelEditorAliases.LabelHyperlinkStringPropertyEditor) { }
}