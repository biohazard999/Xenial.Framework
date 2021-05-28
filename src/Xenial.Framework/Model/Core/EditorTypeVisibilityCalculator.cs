﻿using System;

using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xenial.Framework.Model.Core
{
    /// <summary>
    /// Class EditorTypeAliasVisibilityCalculator. Implements the
    /// <see cref="Xenial.Framework.Model.Core.EditorTypeVisibilityCalculator" />
    /// </summary>
    ///
    /// <seealso cref="EditorTypeVisibilityCalculator"/>
    /// <seealso cref="Xenial.Framework.Model.Core.EditorTypeVisibilityCalculator"> <autogeneratedoc /></seealso>

    public abstract class EditorTypeAliasVisibilityCalculator : EditorTypeVisibilityCalculator
    {
        /// <summary>   Gets the name of the editor alias. </summary>
        ///
        /// <value> The name of the editor alias. </value>

        protected abstract string EditorAliasName { get; }

        /// <summary>   Determines whether the specified node is visible. </summary>
        ///
        /// <param name="node">         The node. </param>
        /// <param name="propertyName"> Name of the property. </param>
        ///
        /// <returns>   <c>true</c> if the specified node is visible; otherwise, <c>false</c>. </returns>
        ///
        /// ### <exception cref="NotImplementedException">  . </exception>

        public override bool IsVisible(IModelNode node, string propertyName)
        {
            var propertyEditorType = FindPropertyEditorType(node, EditorAliasName);
            if (HasEditorAliasAttribute(node, EditorAliasName) == true)
            {
                if (propertyEditorType is null) //Base Module -> No Editor registered
                {
                    return true;
                }
            }
            var editorType = FindEditorType(node);
            if (editorType is not null)
            {
                if (propertyEditorType is not null)
                {
                    return editorType == propertyEditorType;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Class EditorTypeVisibilityCalculator. Implements the
    /// <see cref="DevExpress.ExpressApp.Model.IModelIsVisible" />
    /// </summary>
    ///
    /// <seealso cref="IModelIsVisible"/>
    /// <seealso cref="DevExpress.ExpressApp.Model.IModelIsVisible">    <autogeneratedoc /></seealso>

    public abstract class EditorTypeVisibilityCalculator : IModelIsVisible
    {
        /// <summary>   Editors the type. </summary>
        ///
        /// <param name="node"> The node. </param>
        ///
        /// <returns>   Type. </returns>
        ///
        /// ### <exception cref="NotImplementedException">  . </exception>

        protected static Type? FindEditorType(IModelNode node)
        {
            if (node is IModelMember modelMember)
            {
                return modelMember.PropertyEditorType;
            }

            if (node is IModelListView modelListView)
            {
                return modelListView.EditorType;
            }

            if (node is IModelColumn modelColumn)
            {
                return modelColumn.PropertyEditorType;
            }

            if (node is IModelPropertyEditor modelPropertyEditor)
            {
                return modelPropertyEditor.PropertyEditorType;
            }

            return null;
        }

        /// <summary>   Determines whether [has editor alias attribute] [the specified node]. </summary>
        ///
        /// <param name="node">         The node. </param>
        /// <param name="editorAlias">  The editor alias. </param>
        ///
        /// <returns>   bool?. </returns>

        protected static bool? HasEditorAliasAttribute(IModelNode node, string editorAlias)
        {
            if (node is IModelMember modelMember)
            {
                var attribute = modelMember.MemberInfo.FindAttribute<EditorAliasAttribute>();
                if (attribute is not null && attribute.Alias == editorAlias)
                {
                    return true;
                }
            }

            if (node is IModelColumn modelColumn)
            {
                return HasEditorAliasAttribute(modelColumn.ModelMember, editorAlias);
            }

            if (node is IModelPropertyEditor modelPropertyEditor)
            {
                return HasEditorAliasAttribute(modelPropertyEditor.ModelMember, editorAlias);
            }

            if (node is IModelListView modelListView && modelListView.ModelClass is not null && modelListView.ModelClass.TypeInfo is not null)
            {
                var attribute = modelListView.ModelClass.TypeInfo.FindAttribute<EditorAliasAttribute>();
                if (attribute is not null && attribute.Alias == editorAlias)
                {
                    return true;
                }
            }
            if (node is IModelDetailView modelDetailView && modelDetailView.ModelClass is not null && modelDetailView.ModelClass.TypeInfo is not null)
            {
                var attribute = modelDetailView.ModelClass.TypeInfo.FindAttribute<EditorAliasAttribute>();
                if (attribute is not null && attribute.Alias == editorAlias)
                {
                    return true;
                }
            }
            return null;
        }

        /// <summary>   Finds the type of the property editor. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="node">         The node. </param>
        /// <param name="editorAlias">  The editor alias. </param>
        ///
        /// <returns>   System.Nullable&lt;Type&gt;. </returns>

        public static Type? FindPropertyEditorType(IModelNode node, string editorAlias)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            var editorDescriptors = ((IModelSources)node.Application).EditorDescriptors;
            if (editorDescriptors != null)
            {
                foreach (var typeRegistration in editorDescriptors.PropertyEditorRegistrations)
                {
                    if (typeRegistration.Alias == editorAlias)
                    {
                        return typeRegistration.EditorType;
                    }
                }
            }
            return null;
        }

        /// <summary>   Determines whether the specified node is visible. </summary>
        ///
        /// <param name="node">         The node. </param>
        /// <param name="propertyName"> Name of the property. </param>
        ///
        /// <returns>   <c>true</c> if the specified node is visible; otherwise, <c>false</c>. </returns>

        public abstract bool IsVisible(IModelNode node, string propertyName);
    }
}
