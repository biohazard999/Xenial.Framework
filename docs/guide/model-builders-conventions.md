---
title: ModelBuilders - Conventions
---

# ModelBuilders - Conventions

Because `ModelBuilders` work with the powerful [XAF TypesInfo system](https://docs.devexpress.com/eXpressAppFramework/113669/concepts/business-model-design/types-info-subsystem) we can apply metadata in an imperative way.

## Imperative simple example

We can use all the power of C# to apply attributes, like if statements, foreach loops, string interpolation etc.:

<<< @/guide/samples/model-builder-convention/DemoNaive.cs{15-28}

As you can see, we can use the power of C# to build our metadata on the fly. This opens up a load of possibilities for advanced scenarios like reading metadata out of a configuration file or database, or other runtime modifications.

::: warning
Because XAF reads metadata at application startup you need to restart the application to apply the changes.
:::

## Imperative advanced example

Imagine we have `Sensors` business object where we have a broad table with values from `Value1` to `Value5`.

<<< @/guide/samples/model-builder-convention/Sensor.cs{15,18,21,24,27}

We want to apply metadata to those but we want to do that in an imperative way. We just can use a `for loop` to do so.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.cs{15-19}

As you can see we can use imperative code to apply the caption dynamically.

## ConventionBuilder - ForProperties

But that leads us to a not refactor safe way of doing things. So we can use the `ForProperties` that accepts a range of properties:

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForProperties.cs{15-21}

::: tip
This is especially useful if you want to apply a certain editor to a couple of properties.
:::

## ConventionBuilder - ForAllProperties

Sometimes it can be very useful to apply attributes to a whole range of properties. For example we want to set the `AllowEdit` property to false.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForAllProperties.cs{15-16}

This will add an `ModelDefaultAttribute("AllowEdit", "False")` to every property of this class.

::: tip
You also can use the shorthand `NotAllowingEdit()` to achieve the same result.
:::

## ConventionBuilder - Except

But often you will hit the problem that you don't want to apply those attributes to all of the classes? You can use the `Except` method that acts as a filter. Let's imagine we want `Value2` and `Value4` to be editable.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForAllProperties.Except.cs{16-19}

::: tip
There are several overloads that will help you build more complex filters.
:::

## ConventionBuilder - ForPropertiesOfType

Sometimes its super useful to apply attributes to a specific data type. Let's imagine we want to apply `DisplayFormat` and `EditMask` to all integer properties.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForPropertiesOfType.cs{15-17}

::: tip
You can use this with the `Except` filter as well!
:::
