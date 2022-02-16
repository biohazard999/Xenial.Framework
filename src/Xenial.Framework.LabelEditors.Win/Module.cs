﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xenial.Framework.LabelEditors.Win;

/// <summary>
/// Class XenialLabelEditorsWindowsFormsModule.
/// Implements the <see cref="Xenial.Framework.XenialModuleBase" />
/// </summary>
/// <seealso cref="Xenial.Framework.XenialModuleBase" />
/// <autogeneratedoc />
[XenialCheckLicense]
public sealed partial class XenialLabelEditorsWindowsFormsModule : XenialModuleBase
{
    /// <summary>
    /// Adds the DevExpress.ExpressApp.SystemModule.SystemModule to the collection.
    /// </summary>
    ///
    /// <returns>   ModuleTypeList. </returns>

    protected override ModuleTypeList GetRequiredModuleTypesCore()
        => base.GetRequiredModuleTypesCore()
            .AndModuleTypes(new[]
            {
                    typeof(SystemWindowsFormsModule),
                    typeof(XenialLabelEditorsModule)
            });

    /// <summary>   Registers the editor descriptors. </summary>
    ///
    /// <param name="editorDescriptorsFactory"> The editor descriptors factory. </param>

    protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
    {
        base.RegisterEditorDescriptors(editorDescriptorsFactory);

        editorDescriptorsFactory
            .UseLabelStringPropertyEditorsWin()
            .UseLabelHyperlinkStringPropertyEditorsWin();
    }
}