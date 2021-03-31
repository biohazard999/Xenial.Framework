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
| `AllowingEdit()`                                  | [`ModelDefaultAttribute("AllowEdit", "True")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowEdit)                      |
| `NotAllowingEdit()`                               | [`ModelDefaultAttribute("AllowEdit", "False")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowEdit)                     |
| `AllowingNew()`                                   | [`ModelDefaultAttribute("AllowNew", "True")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowNew)                       |
| `NotAllowingNew()`                                | [`ModelDefaultAttribute("AllowNew", "False")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowNew)                      |
| `AllowingDelete()`                                | [`ModelDefaultAttribute("AllowDelete", "True")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowDelete)                    |
| `NotAllowingDelete()`                             | [`ModelDefaultAttribute("AllowDelete", "False")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowDelete)                   |
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
| `AllowingEdit`                                    | [`ModelDefaultAttribute("AllowEdit", "True")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelCommonMemberViewItem.AllowEdit)                 |
| `NotAllowingEdit`                                 | [`ModelDefaultAttribute("AllowEdit", "False")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelCommonMemberViewItem.AllowEdit)                 |
| `AllowingNew`                                     | [`ModelDefaultAttribute("AllowNew", "True")`]()                 |
| `NotAllowingNew`                                  | [`ModelDefaultAttribute("AllowNew", "False")`]()                 |
| `AllowingDelete`                                  | [`ModelDefaultAttribute("AllowDelete", "True")`]()                 |
| `NotAllowingDelete`                               | [`ModelDefaultAttribute("AllowDelete", "False")`]()                 |
| `AllowingEverything()`                            | `AllowingDelete`<br>`AllowingEdit`<br>`AllowingNew`               |
| `AllowingNothing()`                               | `NotAllowingDelete`<br>`NotAllowingEdit`<br>`NotAllowingNew`      |
| `UsingEditorAlias`                                | [`EditorAliasAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.EditorAliasAttribute)                 |
| `UsingPropertyEditor`                                | [`ModelDefaultAttribute("PropertyEditorType")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelCommonMemberViewItem.PropertyEditorType)                 |
| ``                            | [``]()                 |

### Domain-Extensions `string`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingStringPropertyEditor`                            | [`EditorAliasAttribute("StringPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113528/concepts/business-model-design/data-types-supported-by-built-in-editors/string-properties)                 |
| `UsingRichTextPropertyEditor`                            | [`EditorAliasAttribute("StringPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113528/concepts/business-model-design/data-types-supported-by-built-in-editors/string-properties)                 |
| `UsingCriteriaPropertyEditor`                            | [`EditorAliasAttribute("CriteriaPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113564/concepts/business-model-design/data-types-supported-by-built-in-editors/criteria-properties)<br>[`CriteriaOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Editors.CriteriaOptionsAttribute)                 |
| `UsingExtendedCriteriaPropertyEditor`                            | [`EditorAliasAttribute("ExtendedCriteriaPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113564/concepts/business-model-design/data-types-supported-by-built-in-editors/criteria-properties)<br>[`CriteriaOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Editors.CriteriaOptionsAttribute)                 |
| `UsingPopupCriteriaPropertyEditor`                            | [`EditorAliasAttribute("PopupCriteriaPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113564/concepts/business-model-design/data-types-supported-by-built-in-editors/criteria-properties)<br>[`CriteriaOptionsAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Editors.CriteriaOptionsAttribute)                 |
| `IsPassword`                            | [`ModelDefaultAttribute("IsPassword", "True")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Editors.PropertyEditor.IsPassword)                 |
| `WithPredefinedValues`                            | [`ModelDefaultAttribute("PredefinedValues")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelCommonMemberViewItem.PredefinedValues)                 |
| ``                            | [``]()                 |

### Domain-Extensions `bool` & `bool?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingBooleanPropertyEditor`                            | [`EditorAliasAttribute("BooleanPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113540/concepts/business-model-design/data-types-supported-by-built-in-editors/boolean-properties)                 |
| ``                            | [``]()                 |

### Domain-Extensions `int` & `int?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingIntegerPropertyEditor`                            | [`EditorAliasAttribute("IntegerPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113532/concepts/business-model-design/data-types-supported-by-built-in-editors/numeric-properties#integerpropertyeditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `decimal` & `decimal?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingDecimalPropertyEditor`                            | [`EditorAliasAttribute("DecimalPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113532/concepts/business-model-design/data-types-supported-by-built-in-editors/numeric-properties#decimalpropertyeditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `double` & `double?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingDoublePropertyEditor`                            | [`EditorAliasAttribute("DoublePropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113532/concepts/business-model-design/data-types-supported-by-built-in-editors/numeric-properties#doublepropertyeditor)                 |

### Domain-Extensions `float` & `float?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingFloatPropertyEditor`                            | [`EditorAliasAttribute("FloatPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113532/concepts/business-model-design/data-types-supported-by-built-in-editors/numeric-properties#floatpropertyeditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `long` & `long?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingLongPropertyEditor`                            | [`EditorAliasAttribute("IntegerPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113532/concepts/business-model-design/data-types-supported-by-built-in-editors/numeric-properties#longpropertyeditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `byte` & `byte?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingBytePropertyEditor`                            | [`EditorAliasAttribute("BytePropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113532/concepts/business-model-design/data-types-supported-by-built-in-editors/numeric-properties#bytepropertyeditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `DateTime` & `DateTime?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingDateTimePropertyEditor`                            | [`EditorAliasAttribute("DateTimePropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113536/concepts/business-model-design/data-types-supported-by-built-in-editors/date-and-time-properties#datepropertyeditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `TimeSpan` & `TimeSpan?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingTimeSpanPropertyEditor`                            | [`EditorAliasAttribute("TimeSpanPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113536/concepts/business-model-design/data-types-supported-by-built-in-editors/date-and-time-properties#timespanpropertyeditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `byte[]` & `byte[]?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingImagePropertyEditor`                            | [`EditorAliasAttribute("ImagePropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113544/concepts/business-model-design/data-types-supported-by-built-in-editors/blob-image-properties) <br> [`ImageEditorAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ImageEditorAttribute)                |
| ``                            | [``]()                 |

### Domain-Extensions `enum` & `enum?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingEnumPropertyEditor`                            | [`EditorAliasAttribute("EnumPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113552/concepts/business-model-design/data-types-supported-by-built-in-editors/enumeration-properties)                 |
| ``                            | [``]()                 |

### Domain-Extensions `object` & `object?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingDetailPropertyEditor`                            | [`EditorAliasAttribute("DetailPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113572/concepts/business-model-design/data-types-supported-by-built-in-editors/reference-foreign-key-complex-type-properties#detailpropertyeditor)                 |
| `UsingLookupPropertyEditor`                            | [`EditorAliasAttribute("LookupPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113572/concepts/business-model-design/data-types-supported-by-built-in-editors/reference-foreign-key-complex-type-properties#lookuppropertyeditor)                 |
| `UsingObjectPropertyEditor`                            | [`EditorAliasAttribute("ObjectPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113572/concepts/business-model-design/data-types-supported-by-built-in-editors/reference-foreign-key-complex-type-properties#objectpropertyeditor)                 |
| `UsingDefaultPropertyEditor`                            | [`EditorAliasAttribute("DefaultPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Editors.EditorAliases.DefaultPropertyEditor)                 |
| ``                            | [``]()                 |

### Domain-Extensions `Type` & `Type?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingTypePropertyEditor`                            | [`EditorAliasAttribute("TypePropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113579/concepts/business-model-design/data-types-supported-by-built-in-editors/type-properties)                 |
| `UsingVisibleInReportsTypePropertyEditor`                            | [`EditorAliasAttribute("VisibleInReportsTypePropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113579/concepts/business-model-design/data-types-supported-by-built-in-editors/type-properties)                 |
| ``                            | [``]()                 |

### Domain-Extensions `System.Drawing.Color` & `System.Drawing.Color?`

| Xenial Extension                                  | XAF Attribute                                                     |
| ------------------------------------------------- |------------------------------------------------------------------ |
| `UsingColorPropertyEditor`                            | [`EditorAliasAttribute("ColorPropertyEditor")`](https://docs.devexpress.com/eXpressAppFramework/113658/concepts/business-model-design/data-types-supported-by-built-in-editors/color-properties)                 |
| ``                            | [``]()                 |