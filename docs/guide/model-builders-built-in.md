---
title: ModelBuilders - Built-in Attributes
---

# ModelBuilders - Built-in Attributes

## Class Attributes

### Behavior

| Xenial Extension                      | XAF Attribute                                                     |
| ------------------------------------- |------------------------------------------------------------------ |
| `WithDefaultClassOptions()`           | [`DefaultClassOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.DefaultClassOptionsAttribute)                                    |
| `HasCaption()`                        | [`ModelDefaultAttribute("Caption")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.ModelDefaultAttribute)                                |
| `HasImage()`                          | [`ImageNameAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ImageNameAttribute)                                              |
| `HasDefaultProperty()`                | `DefaultPropertyAttribute`<br>[`XafDefaultPropertyAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.DC.XafDefaultPropertyAttribute)       |
| `HasFriendlyKeyProperty()`            | [`FriendlyKeyPropertyAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.FriendlyKeyPropertyAttribute)                                    |
| `HasNavigationItem()`                 | [`NavigationItemAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.NavigationItemAttribute)                                         |
| `HasObjectCaptionFormat()`            | [`ObjectCaptionFormatAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ObjectCaptionFormatAttribute)                                    |
| `IsCreatableItem()`                   | [`CreatableItemAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.CreatableItemAttribute)                                          |
| `IsVisibleInReports()`                | [`VisibleInReportsAttribute(true)`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.VisibleInReportsAttribute)                                 |
| `IsNotVisibleInReports()`             | [`VisibleInReportsAttribute(false)`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.VisibleInReportsAttribute)                                 |
| `IsVisibleInDashboards()`             | [`VisibleInDashboardsAttribute(true)`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.VisibleInDashboardsAttribute)                              |
| `IsNotVisibleInDashboards()`          | [`VisibleInDashboardsAttribute(false)`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.VisibleInDashboardsAttribute)                              |
| `WithDefaultListViewOptions()`        | [`DefaultListViewOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.DefaultListViewOptionsAttribute)                                 |


### ModelDefault

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `WithModelDefault()`                              | [`ModelDefaultAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.ModelDefaultAttribute)                                          |
| `HasDefaultDetailViewImage()`                     | [`ModelDefaultAttribute("DefaultDetailViewImage")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultDetailViewImage)                 |
| `HasDefaultListViewImage()`                       | [`ModelDefaultAttribute("DefaultListViewImage")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultListViewImage)                   |
| `HasDefaultDetailViewId()`                        | [`ModelDefaultAttribute("DefaultDetailView")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultDetailView)                      |
| `HasDefaultListViewId()`                          | [`ModelDefaultAttribute("DefaultListView")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultListView)                        |
| `HasDefaultLookupListViewId()`                    | [`ModelDefaultAttribute("DefaultLookupListView")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultLookupListView)                  |
| `ForListViewsDefaultAllowEdit()`                  | [`ModelDefaultAttribute("DefaultListViewAllowEdit")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultListViewAllowEdit)               |
| `ForListViewsDefaultShowAutoFilterRow()`          | [`ModelDefaultAttribute("DefaultListViewShowAutoFilterRow")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.SystemModule.IModelClassShowAutoFilterRow.DefaultListViewShowAutoFilterRow)       |
| `ForListViewsDefaultShowFindPanel()`              | [`ModelDefaultAttribute("DefaultListViewShowFindPanel")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.SystemModule.IModelClassShowFindPanel.DefaultListViewShowFindPanel)           |
| `ForListViewsDefaultMasterDetailMode()`           | [`ModelDefaultAttribute("DefaultListViewMasterDetailMode")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultListViewMasterDetailMode)    |
| `HasDefaultLookupEditorMode()`                    | [`ModelDefaultAttribute("DefaultLookupEditorMode")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelClass.DefaultLookupEditorMode)                |
| `ForListViewsDefaultNewItemRowPosition()`         | [`ModelDefaultAttribute("DefaultListViewNewItemRowPosition")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.SystemModule.IModelClassNewItemRow.DefaultListViewNewItemRowPosition)      |
| `AllowingEdit()`                                  | [`ModelDefaultAttribute("AllowEdit", "True")`]()                      |
| `NotAllowingEdit()`                               | [`ModelDefaultAttribute("AllowEdit", "False")`]()                     |
| `AllowingNew()`                                   | [`ModelDefaultAttribute("AllowNew", "True")`]()                       |
| `NotAllowingNew()`                                | [`ModelDefaultAttribute("AllowNew", "False")`]()                      |
| `AllowingDelete()`                                | [`ModelDefaultAttribute("AllowDelete", "True")`]()                    |
| `NotAllowingDelete()`                             | [`ModelDefaultAttribute("AllowDelete", "False")`]()                   |
| `AllowingEverything()`                            | `AllowingDelete`<br>`AllowingEdit`<br>`AllowingNew`               |
| `AllowingNothing()`                               | `NotAllowingDelete`<br>`NotAllowingEdit`<br>`NotAllowingNew`      |


### Xenial specific

| Xenial Extension                      | XAF Attribute                                                     |
| ------------------------------------- |------------------------------------------------------------------ |
| `IsSingleton()`                       | `SingletonAttribute`<br>`NotAllowingDelete`<br>`NotAllowingNew`   |

## Property Attributes

### Behavior

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `HasCaption()`                                    | [`ModelDefaultAttribute("Caption")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.ModelDefaultAttribute)                                |
| `ImmediatePostsData()`                            | [`ImmediatePostDataAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ImmediatePostDataAttribute)                 |
| `HasIndex()`                                      | [`IndexAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.IndexAttribute)                 |
| `HasEditMask()`                                   | [`ModelDefaultAttribute("EditMask")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Editors.PropertyEditor.EditMask)                 |
| `HasDisplayFormat()`                              | [`ModelDefaultAttribute("DisplayFormat")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Editors.PropertyEditor.DisplayFormat)                 |
| `HasTooltip()`                                    | [`ModelDefaultAttribute("ToolTip")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ToolTipAttribute)                 |
| `HasTooltipTitle()`                               | [`ModelDefaultAttribute("ToolTipTitle")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ToolTipAttribute)                 |
| `HasTooltipIconType()`                            | [`ModelDefaultAttribute("ToolTipIconType")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ToolTipAttribute)                 |
| `HasNullText()`                                   | [`ModelDefaultAttribute("NullText")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelCommonMemberViewItem.NullText)                 |
| `WithModelDefault`                                | [`ModelDefaultAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.ModelDefaultAttribute)                                          |
| `AllowingEdit`                                    | [`ModelDefaultAttribute("AllowEdit", "True")`]()                 |
| `NotAllowingEdit`                                 | [`ModelDefaultAttribute("AllowEdit", "False")`]()                 |
| `AllowingNew`                                     | [`ModelDefaultAttribute("AllowNew", "True")`]()                 |
| `NotAllowingNew`                                  | [`ModelDefaultAttribute("AllowNew", "False")`]()                 |
| `AllowingDelete`                                  | [`ModelDefaultAttribute("AllowDelete", "True")`]()                 |
| `NotAllowingDelete`                               | [`ModelDefaultAttribute("AllowDelete", "False")`]()                 |
| `AllowingEverything()`                            | `AllowingDelete`<br>`AllowingEdit`<br>`AllowingNew`               |
| `AllowingNothing()`                               | `NotAllowingDelete`<br>`NotAllowingEdit`<br>`NotAllowingNew`      |
| ``                            | [``]()                 |