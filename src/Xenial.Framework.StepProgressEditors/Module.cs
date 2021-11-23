﻿using System;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;

namespace Xenial.Framework.StepProgressEditors;

/// <summary>
/// Class XenialStepProgressEditorsModule.
/// Implements the <see cref="Xenial.Framework.XenialModuleBase" />
/// </summary>
/// <seealso cref="Xenial.Framework.XenialModuleBase" />
/// <autogeneratedoc />
[XenialCheckLicense]
public sealed partial class XenialStepProgressEditorsModule : XenialModuleBase
{
    /// <summary>   Registers the editor descriptors. </summary>
    ///
    /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>

    protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
    {
        base.RegisterEditorDescriptors(editorDescriptorsFactory);
        editorDescriptorsFactory.UseStepProgressEnumPropertyEditors();
    }
}
