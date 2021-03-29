---
title: ModelBuilders - Advanced
---

# ModelBuilders - Advanced

So far we have seen a lot of code how to utilize `ModelBuilders` in different ways, but all of that was an introduction to show you how to **use** the tool, but there is a key thing missing: Write your own DSL (domain specific language) for your project. Don't worry, it's not that complicated, nor hard. But you need to wrap your head around C# generics and a couple of principals.

## Custom attributes

`ModelBuilders` are designed to be extensible from the ground up. So internal it only deals with a couple of primitives:

- `IModelBuilder<TClassType> : IModelBuilder`
- `IPropertyBuilder<TPropertyType, TClassType> : IPropertyBuilder`
- `System.Attribute`

That way we can make sure we don't make our solutions to specific, but that also helps you to add your custom attributes with ease to your project. Let's imagine we have a custom `ExportFormatAttibute`.

<<< @/guide/samples/advanced/CustomAttributes.cs{31,33-34}

You can see we can use the `WithAttribute()` method to apply any attribute to our business object and properties.

::: tip
There is another overload that accepts an action to configure the attribute.

```cs
.WithAttribute<ExportFormatAttribute>(a => a.Format = "My Format");
```

This is an convenience method, but most attributes are written in an immutable way, hence this overload may not work as expected.

:::

## Domain-extensions

Often you have to deal with domain types like for example, currency or dates. In most of the projects those formats are supposed to be consistent, but a `decimal` type can describe money, percentages, pressure or any other unit. Dates are often formatted different, based on usage as well. Normally in an XAF application this is rather difficult, but with `Xenial.Framework` you can write your own extension methods to make your app consistent and keep it [DRY](https://de.wikipedia.org/wiki/Don%E2%80%99t_repeat_yourself).

To provide an domain extension for our `ExportFormatAttribute` from the last chapter we need to create an extension method for the `IModelBuilder<TClassType>` and `IPropertyBuilder<TPropertyType, TClassType>` interfaces.

<<< @/guide/samples/advanced/DomainExtensions.cs

::: tip
To make sure that your domain extensions appear in IntelliSense automatically, you can use the `Xenial.Framework.ModelBuilders` namespace.
If you have multiple domain extensions you can mark the class as `partial` and reuse the class for different extensions, but still can split up the classes into separate files.
:::

::: warning
You also can use the `IModelBuilder` and `IPropertyBuilder` interfaces directly, but then you use chainability in the most cases. See the [type safe chapter](#type-safe) for more information.
:::

## Combining attributes

## Type safe 

## Advanced builders