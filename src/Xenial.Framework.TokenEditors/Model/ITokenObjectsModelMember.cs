﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

using Xenial.Framework.Model.Core;
using Xenial.Framework.TokenEditors.Model;
using Xenial.Framework.TokenEditors.PubTernal;

namespace Xenial.Framework.TokenEditors.Model
{
    /// <summary>
    /// Interface ITokenObjectsModelMember
    /// </summary>
    /// <autogeneratedoc />
    public interface ITokenObjectsModelMember
    {
        /// <summary>
        /// Gets or sets the token drop down show mode.
        /// </summary>
        /// <value>The token drop down show mode.</value>
        /// <autogeneratedoc />
        [ModelBrowsable(typeof(TokenObjectsEditorTypeVisibilityCalculator))]
        [Category(TokenEditorAliases.TokenObjectsPropertyEditor)]
        [ModelPersistentName(TokenEditorAliases.TokenObjectsPropertyEditor + "." + nameof(TokenDropDownShowMode))]
        TokenDropDownShowMode? TokenDropDownShowMode { get; set; }
    }

    /// <summary>
    /// Interface ITokenStringModelPropertyEditor
    /// </summary>
    /// <autogeneratedoc />
    public interface ITokenObjectsModelPropertyEditor
    {
        /// <summary>
        /// Gets or sets the token drop down show mode.
        /// </summary>
        /// <value>The token drop down show mode.</value>
        /// <autogeneratedoc />
        [ModelValueCalculator(nameof(IModelPropertyEditor.ModelMember), typeof(ITokenStringModelMember), nameof(ITokenStringModelMember.TokenDropDownShowMode))]
        [ModelBrowsable(typeof(TokenObjectsEditorTypeVisibilityCalculator))]
        [Category(TokenEditorAliases.TokenObjectsPropertyEditor)]
        [ModelPersistentName(TokenEditorAliases.TokenObjectsPropertyEditor + "." + nameof(TokenDropDownShowMode))]
        TokenDropDownShowMode? TokenDropDownShowMode { get; set; }
    }

    /// <summary>
    /// Class TokenObjectsEditorTypeVisibilityCalculator. This class cannot be inherited.
    /// Implements the <see cref="Xenial.Framework.Model.Core.EditorTypeVisibilityCalculator" />
    /// </summary>
    /// <seealso cref="Xenial.Framework.Model.Core.EditorTypeVisibilityCalculator" />
    /// <autogeneratedoc />
    public sealed class TokenObjectsEditorTypeVisibilityCalculator : EditorTypeAliasVisibilityCalculator
    {
        /// <summary>
        /// Gets the name of the editor alias.
        /// </summary>
        /// <value>The name of the editor alias.</value>
        /// <autogeneratedoc />
        protected override string EditorAliasName => TokenEditorAliases.TokenObjectsPropertyEditor;
    }

#pragma warning disable CA1707 //Convention by XAF
    /// <summary>
    /// Class TokenObjectsModelMemberDomainLogic.
    /// </summary>
    /// <autogeneratedoc />
    [DomainLogic(typeof(ITokenObjectsModelMember))]
    public static class TokenObjectsModelMemberDomainLogic
    {
        /// <summary>
        /// Gets the token drop down show mode.
        /// </summary>
        /// <param name="modelMember">The model member.</param>
        /// <returns>DevExpress.Persistent.Base.TokenDropDownShowMode?.</returns>
        /// <autogeneratedoc />
        public static TokenDropDownShowMode? Get_TokenDropDownShowMode(IModelMember modelMember)
        {
            _ = modelMember ?? throw new ArgumentNullException(nameof(modelMember));
            var attribute = modelMember.MemberInfo.FindAttribute<TokenObjectsEditorAttribute>();
            if (attribute is not null)
            {
                return attribute.DropDownShowMode;
            }
            return null;
        }
    }
#pragma warning restore CA1707
}

namespace DevExpress.ExpressApp.Model
{
    /// <summary>
    /// Class ModelInterfaceExtendersLayoutBuilderExtensions.
    /// </summary>
    /// <autogeneratedoc />
    public static partial class ModelInterfaceExtendersTokenEditorsExtensions
    {
        /// <summary>
        /// Uses the token objects property editors.
        /// </summary>
        /// <param name="extenders">The extenders.</param>
        /// <returns>DevExpress.ExpressApp.Model.ModelInterfaceExtenders.</returns>
        /// <autogeneratedoc />
        public static ModelInterfaceExtenders UseTokenObjectsPropertyEditors(this ModelInterfaceExtenders extenders)
        {
            _ = extenders ?? throw new ArgumentNullException(nameof(extenders));

            extenders.Add<IModelMember, ITokenObjectsModelMember>();
            extenders.Add<IModelPropertyEditor, ITokenObjectsModelPropertyEditor>();

            return extenders;
        }
    }
}

namespace DevExpress.ExpressApp.Model
{
    /// <summary>
    /// Class XenialTokenEditorsModelTypeList.
    /// </summary>
    /// <autogeneratedoc />
    public static partial class XenialTokenEditorsModelTypeList
    {
        /// <summary>
        /// Uses the token objects editor regular types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Type&gt;.</returns>
        /// <autogeneratedoc />
        public static IEnumerable<Type> UseTokenObjectsEditorRegularTypes(this IEnumerable<Type> types)
            => types.Concat(new[]
            {
                typeof(ITokenObjectsModelMember),
                typeof(ITokenObjectsModelPropertyEditor),
                typeof(TokenObjectsModelMemberDomainLogic),
                typeof(TokenObjectsEditorTypeVisibilityCalculator)
            });
    }
}

