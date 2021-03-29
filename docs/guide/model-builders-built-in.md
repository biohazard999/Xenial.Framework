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
| `IsNotVisibleInReports()`             | `VisibleInReportsAttribute(false)`                                |
| `IsVisibleInDashboards()`             | [`VisibleInDashboardsAttribute(true)`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.VisibleInDashboardsAttribute)                              |
| `IsNotVisibleInDashboards()`          | `VisibleInDashboardsAttribute(false)`                             |
| `WithDefaultListViewOptions()`        | [`DefaultListViewOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.DefaultListViewOptionsAttribute)                                 |


### ModelDefault

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `WithModelDefault()`                              | [`ModelDefaultAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.ModelDefaultAttribute)                                          |
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


#### Xenial specific

| Xenial Extension                      | XAF Attribute                                                     |
| ------------------------------------- |------------------------------------------------------------------ |
| `IsSingleton()`                       | `SingletonAttribute`<br>`NotAllowingDelete`<br>`NotAllowingNew`   |