#pragma warning disable CA1724 //Conflicts with Winforms: Should not conflict in practice

using System;
using System.Linq.Expressions;

using DevExpress.ExpressApp.Layout;

using Xenial.Framework.Layouts.ColumnItems;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

#pragma warning disable CA1822 //By design for fluent interface

namespace Xenial.Framework.Layouts
{
    /// <summary>
    /// 
    /// </summary>
    [XenialCheckLicense]
    public partial class LayoutBuilder<TModelClass> : LayoutBuilder
        where TModelClass : class
    {
        /// <summary>   Properties the editor. </summary>
        ///
        /// <typeparam name="TProperty">    The type of the t property. </typeparam>
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>
        /// Xenial.Framework.Layouts.Items.LayoutPropertyEditorItem&lt;TModelClass&gt;.
        /// </returns>

        public LayoutPropertyEditorItem<TModelClass> PropertyEditor<TProperty>(Expression<Func<TModelClass, TProperty>> expression)
            => LayoutPropertyEditorItem<TModelClass>.Create(expression);

        /// <summary>   Properties the editor. </summary>
        ///
        /// <typeparam name="TProperty">    The type of the t property. </typeparam>
        /// <param name="expression">               The expression. </param>
        /// <param name="configurePropertyEditor">  The configure property editor. </param>
        ///
        /// <returns>   LayoutPropertyEditorItem&lt;TModelClass&gt;. </returns>

        public LayoutPropertyEditorItem<TModelClass> PropertyEditor<TProperty>(Expression<Func<TModelClass, TProperty>> expression, Action<LayoutPropertyEditorItem<TModelClass>> configurePropertyEditor)
            => LayoutPropertyEditorItem<TModelClass>.Create(expression, configurePropertyEditor);
    }

    /// <summary>
    /// 
    /// </summary>
    [XenialCheckLicense]
    public partial class LayoutBuilder
    {
        #region Leaf Items

        /// <summary>   Properties the editor. </summary>
        ///
        /// <typeparam name="TModelClass">  The type of the t model class. </typeparam>
        /// <typeparam name="TProperty">    The type of the t property. </typeparam>
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>
        /// Xenial.Framework.Layouts.Items.LayoutPropertyEditorItem&lt;TModelClass&gt;.
        /// </returns>

        public LayoutPropertyEditorItem<TModelClass> PropertyEditor<TModelClass, TProperty>(Expression<Func<TModelClass, TProperty>> expression)
            where TModelClass : class
                => LayoutPropertyEditorItem<TModelClass>.Create(expression);

        /// <summary>   Properties the editor. </summary>
        ///
        /// <typeparam name="TModelClass">  The type of the t model class. </typeparam>
        /// <typeparam name="TProperty">    The type of the t property. </typeparam>
        /// <param name="expression">               The expression. </param>
        /// <param name="configurePropertyEditor">  The configure property editor. </param>
        ///
        /// <returns>   LayoutPropertyEditorItem&lt;TModelClass&gt;. </returns>

        public LayoutPropertyEditorItem<TModelClass> PropertyEditor<TModelClass, TProperty>(Expression<Func<TModelClass, TProperty>> expression, Action<LayoutPropertyEditorItem<TModelClass>> configurePropertyEditor)
            where TModelClass : class
                => LayoutPropertyEditorItem<TModelClass>.Create(expression, configurePropertyEditor);

        /// <summary>   Properties the editor. </summary>
        ///
        /// <param name="propertyEditorId"> The property editor identifier. </param>
        ///
        /// <returns>   LayoutPropertyEditorItem. </returns>

        public LayoutPropertyEditorItem PropertyEditor(string propertyEditorId)
            => LayoutPropertyEditorItem.Create(propertyEditorId);

        /// <summary>   Properties the editor. </summary>
        ///
        /// <param name="propertyEditorId">         The property editor identifier. </param>
        /// <param name="configurePropertyEditor">  The configure property editor. </param>
        ///
        /// <returns>   LayoutPropertyEditorItem. </returns>

        public LayoutPropertyEditorItem PropertyEditor(string propertyEditorId, Action<LayoutPropertyEditorItem> configurePropertyEditor)
            => LayoutPropertyEditorItem.Create(propertyEditorId, configurePropertyEditor);

        /// <summary>   Views the item. </summary>
        ///
        /// <param name="viewItemId">   The view item identifier. </param>
        ///
        /// <returns>   LayoutViewItem. </returns>

        public LayoutViewItem ViewItem(string viewItemId)
            => LayoutViewItem.Create(viewItemId);

        /// <summary>   Empties the space item. </summary>
        ///
        /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

        public LayoutEmptySpaceItem EmptySpaceItem()
            => LayoutEmptySpaceItem.Create();

        /// <summary>   Empties the space item. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   Xenial.Framework.Layouts.Items.LayoutEmptySpaceItem. </returns>

        public LayoutEmptySpaceItem EmptySpaceItem(string id)
            => LayoutEmptySpaceItem.Create(id);

        #endregion

        #region Group Items

        /// <summary>   Creates this instance. </summary>
        ///
        /// <returns>   Xenial.Framework.Layouts.Items.LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup()
            => LayoutGroupItem.Create();

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(params LayoutItemNode[] nodes)
            => LayoutGroupItem.Create(nodes);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption)
            => LayoutGroupItem.Create(caption);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption, params LayoutItemNode[] nodes)
            => LayoutGroupItem.Create(caption, nodes);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption, FlowDirection flowDirection)
            => LayoutGroupItem.Create(caption, flowDirection);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => LayoutGroupItem.Create(caption, flowDirection, nodes);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        ///
        /// <returns>   Xenial.Framework.Layouts.Items.LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption, string imageName, FlowDirection flowDirection)
            => LayoutGroupItem.Create(caption, imageName, flowDirection);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption, string imageName, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => LayoutGroupItem.Create(caption, imageName, flowDirection, nodes);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="id">               The identifier. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption, string? imageName, string id, FlowDirection flowDirection)
            => LayoutGroupItem.Create(caption, imageName, id, flowDirection);

        /// <summary>   Layouts the group. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="id">               The identifier. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   LayoutGroupItem. </returns>

        public LayoutGroupItem LayoutGroup(string caption, string? imageName, string id, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => LayoutGroupItem.Create(caption, imageName, id, flowDirection, nodes);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup()
            => HorizontalLayoutGroupItem.Create();

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="configureGroup">   The configure group. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(Action<HorizontalLayoutGroupItem> configureGroup)
              => HorizontalLayoutGroupItem.Create(configureGroup);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="configureGroup">   The configure group. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(Action<HorizontalLayoutGroupItem> configureGroup, params LayoutItemNode[] nodes)
              => HorizontalLayoutGroupItem.Create(configureGroup, nodes);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(params LayoutItemNode[] nodes)
              => HorizontalLayoutGroupItem.Create(nodes);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(string caption)
            => HorizontalLayoutGroupItem.Create(caption);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(string caption, params LayoutItemNode[] nodes)
            => HorizontalLayoutGroupItem.Create(caption, nodes);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(string caption, string imageName)
            => HorizontalLayoutGroupItem.Create(caption, imageName);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        /// <param name="nodes">        The nodes. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(string caption, string imageName, params LayoutItemNode[] nodes)
            => HorizontalLayoutGroupItem.Create(caption, imageName, nodes);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        /// <param name="id">           The identifier. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(string caption, string? imageName, string id)
            => HorizontalLayoutGroupItem.Create(caption, imageName, id);

        /// <summary>   Horizontals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        /// <param name="id">           The identifier. </param>
        /// <param name="nodes">        The nodes. </param>
        ///
        /// <returns>   HorizontalLayoutGroupItem. </returns>

        public HorizontalLayoutGroupItem HorizontalGroup(string caption, string? imageName, string id, params LayoutItemNode[] nodes)
            => HorizontalLayoutGroupItem.Create(caption, imageName, id, nodes);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup()
            => VerticalLayoutGroupItem.Create();

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="configureGroup">   The configure group. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(Action<VerticalLayoutGroupItem> configureGroup)
              => VerticalLayoutGroupItem.Create(configureGroup);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="configureGroup">   The configure group. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(Action<VerticalLayoutGroupItem> configureGroup, params LayoutItemNode[] nodes)
              => VerticalLayoutGroupItem.Create(configureGroup, nodes);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(params LayoutItemNode[] nodes)
            => VerticalLayoutGroupItem.Create(nodes);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(string caption)
            => VerticalLayoutGroupItem.Create(caption);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(string caption, params LayoutItemNode[] nodes)
            => VerticalLayoutGroupItem.Create(caption, nodes);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(string caption, string imageName)
            => VerticalLayoutGroupItem.Create(caption, imageName);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        /// <param name="nodes">        The nodes. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(string caption, string imageName, params LayoutItemNode[] nodes)
            => VerticalLayoutGroupItem.Create(caption, imageName, nodes);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        /// <param name="id">           The identifier. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(string caption, string? imageName, string id)
            => VerticalLayoutGroupItem.Create(caption, imageName, id);

        /// <summary>   Verticals the group. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        /// <param name="id">           The identifier. </param>
        /// <param name="nodes">        The nodes. </param>
        ///
        /// <returns>   VerticalLayoutGroupItem. </returns>

        public VerticalLayoutGroupItem VerticalGroup(string caption, string? imageName, string id, params LayoutItemNode[] nodes)
            => VerticalLayoutGroupItem.Create(caption, imageName, id, nodes);

        /// <summary>   Tabbeds the group. </summary>
        ///
        /// <returns>   LayoutTabbedGroupItem. </returns>

        public LayoutTabbedGroupItem TabbedGroup()
            => LayoutTabbedGroupItem.Create();

        /// <summary>   Tabbeds the group. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   LayoutTabbedGroupItem. </returns>

        public LayoutTabbedGroupItem TabbedGroup(string id)
            => LayoutTabbedGroupItem.Create(id);

        /// <summary>   Tabbeds the group. </summary>
        ///
        /// <param name="id">       The identifier. </param>
        /// <param name="items">    The items. </param>
        ///
        /// <returns>   LayoutTabbedGroupItem. </returns>

        public LayoutTabbedGroupItem TabbedGroup(string id, params LayoutTabGroupItem[] items)
            => LayoutTabbedGroupItem.Create(id, items);

        /// <summary>   Tabbeds the group. </summary>
        ///
        /// <param name="items">    The items. </param>
        ///
        /// <returns>   LayoutTabbedGroupItem. </returns>

        public LayoutTabbedGroupItem TabbedGroup(params LayoutTabGroupItem[] items)
           => LayoutTabbedGroupItem.Create(items);

        /// <summary>   Tabs this instance. </summary>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab()
            => LayoutTabGroupItem.Create();

        /// <summary>   Tabs the specified nodes. </summary>
        ///
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(params LayoutItemNode[] nodes)
            => LayoutTabGroupItem.Create(nodes);

        /// <summary>   Tabs the specified configure tab group item. </summary>
        ///
        /// <param name="configureTabGroupItem">    The configure tab group item. </param>
        /// <param name="nodes">                    The nodes. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(Action<LayoutTabGroupItem> configureTabGroupItem, params LayoutItemNode[] nodes)
            => LayoutTabGroupItem.Create(configureTabGroupItem, nodes);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption)
            => LayoutTabGroupItem.Create(caption);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">  The caption. </param>
        /// <param name="nodes">    The nodes. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, params LayoutItemNode[] nodes)
            => LayoutTabGroupItem.Create(caption, nodes);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, string imageName)
            => LayoutTabGroupItem.Create(caption, imageName);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">      The caption. </param>
        /// <param name="imageName">    Name of the image. </param>
        /// <param name="nodes">        The nodes. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, string imageName, params LayoutItemNode[] nodes)
            => LayoutTabGroupItem.Create(caption, imageName, nodes);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, FlowDirection flowDirection)
            => LayoutTabGroupItem.Create(caption, flowDirection);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => LayoutTabGroupItem.Create(caption, flowDirection, nodes);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, string imageName, FlowDirection flowDirection)
            => LayoutTabGroupItem.Create(caption, imageName, flowDirection);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, string imageName, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => LayoutTabGroupItem.Create(caption, imageName, flowDirection, nodes);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="id">               The identifier. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, string imageName, string id, FlowDirection flowDirection)
            => LayoutTabGroupItem.Create(caption, imageName, id, flowDirection);

        /// <summary>   Tabs the specified caption. </summary>
        ///
        /// <param name="caption">          The caption. </param>
        /// <param name="imageName">        Name of the image. </param>
        /// <param name="id">               The identifier. </param>
        /// <param name="flowDirection">    The flow direction. </param>
        /// <param name="nodes">            The nodes. </param>
        ///
        /// <returns>   LayoutTabGroupItem. </returns>

        public LayoutTabGroupItem Tab(string caption, string imageName, string id, FlowDirection flowDirection, params LayoutItemNode[] nodes)
            => LayoutTabGroupItem.Create(caption, imageName, id, flowDirection, nodes);

        /// <summary>   Horizontals the tab. </summary>
        ///
        /// <returns>   HorizontalLayoutTabGroupItem. </returns>

        public HorizontalLayoutTabGroupItem HorizontalTab()
            => HorizontalLayoutTabGroupItem.Create();

        /// <summary>   Horizontals the tab. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   HorizontalLayoutTabGroupItem. </returns>

        public HorizontalLayoutTabGroupItem HorizontalTab(string id)
            => HorizontalLayoutTabGroupItem.Create(id);

        /// <summary>   Verticals the tab. </summary>
        ///
        /// <returns>   VerticalLayoutTabGroupItem. </returns>

        public VerticalLayoutTabGroupItem VerticalTab()
            => VerticalLayoutTabGroupItem.Create();

        /// <summary>   Verticals the tab. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   VerticalLayoutTabGroupItem. </returns>

        public VerticalLayoutTabGroupItem VerticalTab(string id)
            => VerticalLayoutTabGroupItem.Create(id);

        #endregion
    }
}
