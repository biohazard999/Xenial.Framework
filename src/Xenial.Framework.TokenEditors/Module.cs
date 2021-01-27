﻿using System;
using System.Collections.Generic;

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xenial.Framework.TokenEditors
{
    /// <summary>
    /// Class XenialTokenEditorsWindowsFormsModule.
    /// Implements the <see cref="Xenial.Framework.XenialModuleBase" />
    /// </summary>
    /// <seealso cref="Xenial.Framework.XenialModuleBase" />
    /// <autogeneratedoc />
    [XenialCheckLicence]
    public sealed partial class XenialTokenEditorsModule : XenialModuleBase
    {
        /// <summary>
        /// Registers the editor descriptors.
        /// </summary>
        /// <param name="editorDescriptorsFactory">The editor descriptors factory.</param>
        /// <autogeneratedoc />
        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.UseTokenObjectsPropertyEditors();
            editorDescriptorsFactory.UseTokenStringPropertyEditors();
        }

        /// <summary>
        /// returns empty types
        /// </summary>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        /// <autogeneratedoc />
        protected override IEnumerable<Type> GetRegularTypes() => base.GetRegularTypes()
            .UseTokenStringEditorRegularTypes()
            .UseTokenObjectsEditorRegularTypes();

        /// <summary>
        /// Extends the Application Model.
        /// </summary>
        /// <param name="extenders">A ModelInterfaceExtenders object that is a collection of Application Model interface extenders.</param>
        /// <autogeneratedoc />
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.UseTokenStringPropertyEditors();
            extenders.UseTokenObjectsPropertyEditors();
        }

        /// <summary>
        /// Registers the Generator Updaters. These are classes, used to customize the Application Model's zero layer after it has been generated.
        /// </summary>
        /// <param name="updaters">A ModelNodesGeneratorUpdaters object providing access to the list of Generator Updaters.</param>
        /// <autogeneratedoc />
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.UseTokenObjectsPropertyEditors();
        }
    }
}
