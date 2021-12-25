﻿using System;

using DevExpress.ExpressApp.Editors;

namespace Xenial.Framework.LabelEditors;

/// <summary>
/// Class XenialLabelEditorsModule.
/// Implements the <see cref="Xenial.Framework.XenialModuleBase" />
/// </summary>
/// <seealso cref="Xenial.Framework.XenialModuleBase" />
/// <autogeneratedoc />
[XenialCheckLicense]
public sealed partial class XenialLabelEditorsModule : XenialModuleBase
{
    /// <summary>   Registers the editor descriptors. </summary>
    ///
    /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>

    protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
    {
        base.RegisterEditorDescriptors(editorDescriptorsFactory);

        editorDescriptorsFactory
            .UseLabelStringPropertyEditors()
            .UseLabelHyperlinkStringPropertyEditors();
    }
}
