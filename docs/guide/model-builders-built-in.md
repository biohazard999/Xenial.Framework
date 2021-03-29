---
title: ModelBuilders - Built-in Attributes
---

# ModelBuilders - Built-in Attributes

## Class Attributes

### ModelDefault

| Extension                             | Attribute                                                         |
| ------------------------------------- |------------------------------------------------------------------ |
| `WithModelDefault()`                  | `ModelDefaultAttribute`                                           |
| `HasCaption()`                        | `ModelDefaultAttribute("Caption")`                                |
| `HasDefaultDetailViewImage()`         | `ModelDefaultAttribute("DefaultDetailViewImage")`                 |
| `HasDefaultListViewImage()`           | `ModelDefaultAttribute("DefaultListViewImage")`                   |
| `HasDefaultDetailViewId()`            | `ModelDefaultAttribute("DefaultDetailView")`                      |
| `HasDefaultListViewId()`              | `ModelDefaultAttribute("DefaultListView")`                        |
| `HasDefaultLookupListViewId()`        | `ModelDefaultAttribute("DefaultLookupListView")`                  |
| `ForListViewsDefaultAllowEdit()`      | `ModelDefaultAttribute("DefaultListViewAllowEdit")`               |

### Behavior

| Extension                             | Attribute                                                         |
| ------------------------------------- |------------------------------------------------------------------ |
| `HasImage()`                          | `ImageNameAttribute`                                              |
| `IsCreatableItem()`                   | `CreatableItemAttribute`                                          |