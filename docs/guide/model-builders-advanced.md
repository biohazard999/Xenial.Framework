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
I may seem tempting to use the `IModelBuilder` and `IPropertyBuilder` interfaces directly, but then you use chainability in the most cases. See the [type safe chapter](#type-safe) for more information.
:::

## Combining attributes

Now we know how to apply single custom attributes, we can apply multiple attributes at once too!

<<< @/guide/samples/advanced/MultipleAttributes.cs

Combining multiple attributes is not that hard, but there is a catch here. This domain extension only makes sense for int properties. So let's look into the [type safe chapter](#type-safe) how to fix that.

::: tip
We use that internally for example to combine the visibility properties like `NotVisibleInAnyView()`.  
See [Built in Attributes](/guide/model-builders-built-in.md) for more information.
:::

## Type safe

`ModelBuilders` use the power of C# generics to provide a fluent API to specify metadata for your business objects. Using type inference and generic constraints we can limit what methods can be called on our builders. Let's look into classes first.

### Class restriction

Imagine we have a business object that is like [auditable light](https://supportcenter.devexpress.com/ticket/details/k18352/how-to-implement-the-createdby-createdon-and-updatedby-updatedon-properties-in-a)

<<< @/guide/samples/advanced/AuditLight.cs{12,31,37,43,49,56-61}

We now can use the power of generics to apply those attributes only for classes that implement the `IAuditableLight` interface.

<<< @/guide/samples/advanced/DomainExtensionsTypeSafeClass.cs{8,14,18-19,23,27-28}

::: tip
In this simple example we could have used a custom base object, but often that's not possible in XPO because you can't use associations in non persistent classes.  
This way we can keep this metadata consistent over multiple classes.
:::

### Property restriction

In our last example we had a display format for DateTime properties. Let's built an extension for that.

<<< @/guide/samples/advanced/DomainExtensionsTypeSafeProperties.cs

This time it's a little bit different from the previous example, because we don't use an constraint, but rather the type it self.  
You may ask your self why. The reason is simple: `DateTime` is a struct and hence can't be inherited. That way we help the compiler and IntelliSense to only show the extension methods for `DateTime` properties.

::: tip
You can see we provide two overloads one for nullable dates and one for non nullable dates.  
This is a restriction of the C# compiler. It is a little bit more work to provide both overloads, but it pays of very quickly. 
:::

::: warning
If your extension does not show up in IntelliSense or the compiler reports errors, it might by you swapped the generic parameters.  

```cs
//WRONG:
IPropertyBuilder<TClassType, DateTime>
//CORRECT:
IPropertyBuilder<DateTime, TClassType>
```
:::

## Advanced builders