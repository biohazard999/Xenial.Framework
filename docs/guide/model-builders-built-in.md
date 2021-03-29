---
title: ModelBuilders - Built-in Attributes
---

# ModelBuilders - Built-in Attributes

## Class Attributes

### ModelDefault

| Extension                                         | Attribute                                                         |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `WithModelDefault()`                              | [`ModelDefaultAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.ModelDefaultAttribute)                                          |
| `HasCaption()`                                    | `ModelDefaultAttribute("Caption")`                                |
| `HasDefaultDetailViewImage()`                     | `ModelDefaultAttribute("DefaultDetailViewImage")`                 |
| `HasDefaultListViewImage()`                       | `ModelDefaultAttribute("DefaultListViewImage")`                   |
| `HasDefaultDetailViewId()`                        | `ModelDefaultAttribute("DefaultDetailView")`                      |
| `HasDefaultListViewId()`                          | `ModelDefaultAttribute("DefaultListView")`                        |
| `HasDefaultLookupListViewId()`                    | `ModelDefaultAttribute("DefaultLookupListView")`                  |
| `ForListViewsDefaultAllowEdit()`                  | `ModelDefaultAttribute("DefaultListViewAllowEdit")`               |
| `ForListViewsDefaultShowAutoFilterRow()`          | `ModelDefaultAttribute("DefaultListViewShowAutoFilterRow")`       |
| `ForListViewsDefaultShowFindPanel()`              | `ModelDefaultAttribute("DefaultListViewShowFindPanel")`           |
| `ForListViewsDefaultMasterDetailMode()`           | `ModelDefaultAttribute("ForListViewsDefaultMasterDetailMode")`    |
| `HasDefaultLookupEditorMode()`                    | `ModelDefaultAttribute("DefaultLookupEditorMode")`                |
| `ForListViewsDefaultNewItemRowPosition()`         | `ModelDefaultAttribute("DefaultListViewNewItemRowPosition")`      |
| `AllowingEdit()`                                  | `ModelDefaultAttribute("AllowEdit", "True")`                      |
| `NotAllowingEdit()`                               | `ModelDefaultAttribute("AllowEdit", "False")`                     |
| `AllowingNew()`                                   | `ModelDefaultAttribute("AllowNew", "True")`                       |
| `NotAllowingNew()`                                | `ModelDefaultAttribute("AllowNew", "False")`                      |
| `AllowingDelete()`                                | `ModelDefaultAttribute("AllowDelete", "True")`                    |
| `NotAllowingDelete()`                             | `ModelDefaultAttribute("AllowDelete", "False")`                   |
| `AllowingEverything()`                            | `AllowingDelete`<br>`AllowingEdit`<br>`AllowingNew`               |
| `AllowingNothing()`                               | `NotAllowingDelete`<br>`NotAllowingEdit`<br>`NotAllowingNew`      |

### Behavior

| Extension                             | Attribute                                                         |
| ------------------------------------- |------------------------------------------------------------------ |
| `HasImage()`                          | [`ImageNameAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ImageNameAttribute)                                              |
| `IsCreatableItem()`                   | [`CreatableItemAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.CreatableItemAttribute)                                          |
| `HasDefaultProperty()`                | `DefaultPropertyAttribute`<br>[`XafDefaultPropertyAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.DC.XafDefaultPropertyAttribute)       |
| `HasFriendlyKeyProperty()`            | [`FriendlyKeyPropertyAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.FriendlyKeyPropertyAttribute)                                    |
| `HasObjectCaptionFormat()`            | [`ObjectCaptionFormatAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ObjectCaptionFormatAttribute)                                    |
| `IsVisibleInReports()`                | [`VisibleInReportsAttribute(true)`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.VisibleInReportsAttribute)                                 |
| `IsNotVisibleInReports()`             | `VisibleInReportsAttribute(false)`                                |
| `IsVisibleInDashboards()`             | [`VisibleInDashboardsAttribute(true)`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.VisibleInDashboardsAttribute)                              |
| `IsNotVisibleInDashboards()`          | `VisibleInDashboardsAttribute(false)`                             |
| `WithDefaultClassOptions()`           | [`DefaultClassOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.DefaultClassOptionsAttribute)                                    |
| `HasNavigationItem()`                 | [`NavigationItemAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.NavigationItemAttribute)                                         |
| `WithDefaultListViewOptions()`        | [`DefaultListViewOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.DefaultListViewOptionsAttribute)                                 |

#### Xenial specific

| Extension                             | Attribute                                                         |
| ------------------------------------- |------------------------------------------------------------------ |
| `IsSingleton()`                       | `SingletonAttribute`<br>`NotAllowingDelete`<br>`NotAllowingNew`   |