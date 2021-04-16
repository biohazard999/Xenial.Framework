---
title: ModelBuilders - Advanced
---

# ModelBuilders - Advanced

The documentation on `ModelBuilders` has, thus far,  concentrated on the different ways in which they can be utilised in code. What has not be touched upon as yet is their support for Domain Specific Language (DSL) within individual projects. Whilst neither complicated or difficult to implement their use with DSL does require a basic understanding of c# generics and concepts such as type inference and generic constraints.


## Custom attributes

`ModelBuilders` were designed to be extensible a consequence of which is that they only deal with the following primitives:

- `IModelBuilder<TClassType> : IModelBuilder`
- `IPropertyBuilder<TPropertyType, TClassType> : IPropertyBuilder`
- `System.Attribute`

The benefit that this brings is that solutions and individual projects are not tied to the specific attributes supported by XAF. `ModelBuilders` have the ability to support custom attributes. As an example of this the code below illustrates a custom attribute `ExportFormatAttibute` and then illustrates how it can be referenced in a `ModelBuilder`.

<<< @/guide/samples/advanced/CustomAttributes.cs{31,33-34}

By making use of the `WithAttribute()` method it has been possible to apply the custom attribute to the business object.

::: tip
There is another overload that accepts an action to configure the attribute.

```cs
.WithAttribute<ExportFormatAttribute>(a => a.Format = "My Format");
```
:::

::: warning
Given that the majority of attributes are written to be immutable this convenience method may not as expected.
:::

## Domain-extensions

Most developers will at some point encounter the difficulties that can occur when using domain types such as currency or dates. Whilst consistency is the ultimate goal it can be hard to achieve when for example a `decimal` type can describe money, percentages, pressure or any other unit and the United States persist with a date format that is unrecognisable to the vast majority. Within XAF itself it is not easy to solve these issues however the `Xenial.Framework` allows the creation of extension methods that can provide consistency in these situations and help to keep code [DRY](https://de.wikipedia.org/wiki/Don%E2%80%99t_repeat_yourself).

The code below illustrates the application of a domain extension for the custom attribute  `ExportFormatAttribute` by adding an extension method for the `IModelBuilder<TClassType>` and `IPropertyBuilder<TPropertyType, TClassType>` interfaces.

<<< @/guide/samples/advanced/DomainExtensions.cs

::: tip
To make full use of IntelliSense  and have it see these domain extensions automatically use the `Xenial.Framework.ModelBuilders` namespace.
If using multiple domain extensions classes can be marked as `partial` and reused across different files and projects.
:::

::: warning
Avoid using the `IModelBuilder` and `IPropertyBuilder` interfaces directly as that may prevent these extension methods from remaining type safe. See the [type safe section](#type-safe) for more information.
:::

## Combining attributes

`Xenial.Framework` is not restricted to the application of a single custom attributes it is possible to apply multiple attributes at once as well as is illustrated in the code below.

<<< @/guide/samples/advanced/MultipleAttributes.cs

The code above illustrates succinctly that the addition of multiple custom attributes isn't difficult but it also masks a distict issue. This domain extension only makes sense for properties of type int. The next section examines how it is possible to circumvent this issue.

<!--  Not sure that the tip below should be used here as it seems at odds with what has been said up to now)

::: tip
We use that internally for example to combine the visibility properties like `NotVisibleInAnyView()`.  
See [Built-in Attributes](/guide/model-builders-built-in.md) for more information.
:::

## Type safe

`ModelBuilders` use the power of C# generics to provide a fluent API to specify metadata for business objects. By using type inference and generic constraints it is possible to limit what methods can be called on them.

As with previous examples involving `ModelBuilders` these concepts can be applied to both Classes and Properties.

### Class restriction

In the code below there is a business object that is not unlike the [auditable light](https://supportcenter.devexpress.com/ticket/details/k18352/how-to-implement-the-createdby-createdon-and-updatedby-updatedon-properties-in-a) concept illustrated in the Dev Express knowledge base.

<<< @/guide/samples/advanced/AuditLight.cs{12,31,37,43,49,56-61}

Using the power of generics it becomes possible to apply those attributes just to those classes that implement the `IAuditableLight` interface.

<<< @/guide/samples/advanced/DomainExtensionsTypeSafeClass.cs{8,14,18-19,23,27-28}

::: tip
The code above is a very simple example for which a custom base object could have been used but that would be missing the point that frequently that's not possible because XPO does not allow the use of associations in non persistent classes.  
With this approach metadata can be kept consistent across multiple classes.
:::

### Property restriction

The example used above for classes had a display format for DateTime properties. In the one below an extension will be created for that.

<<< @/guide/samples/advanced/DomainExtensionsTypeSafeProperties.cs

The example above doesn't use a contraint, rather it uses the type itself. There is a sound logical reason for this. `DateTime` is a struct and and as such it can't be inherited. By adopting this approach the compiler and IntelliSense will only show the extension methods for `DateTime` properties.


::: tip
Because of a restriction in the C# compiler Xenial.Framework provides two date overloads, one for nullable dates and the other for non nullable dates.  
Whilst it may seem onerous at first to have to differentiate between the two it will soon become apparent that in confers benefits that wouldn't be possible with just a single overload for all dates.
:::

::: warning
If your extension does not show up in IntelliSense or the compiler reports errors, it might be because the generic parameters are the wrong way around.  

```cs
//WRONG:
IPropertyBuilder<TClassType, DateTime>
//CORRECT:
IPropertyBuilder<DateTime, TClassType>
```
:::

## Advanced builders

For more advanced builders such as for example the convention builder it is possible to work with the primitive `IBuilder`.  

Whilst an example of that is not provide within this documentation there are examples of it's use in the sources code of the framework and the tests that accompany it. If problems are encountered whilst using these please raise them in the [issues](https://github.com/xenial-io/Xenial.Framework/issues/).

<<< @../../src/Xenial.Framework/ModelBuilders/ModelBuilder.ForAllProperties.cs
<<< @../../src/Xenial.Framework/ModelBuilders/PropertyBuilder.Aggregated.cs
