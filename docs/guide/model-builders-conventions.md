---
title: ModelBuilders - Conventions
---

# ModelBuilders - Conventions

Because `ModelBuilders` have been designed to  work with the powerful [XAF TypesInfo system](https://docs.devexpress.com/eXpressAppFramework/113669/concepts/business-model-design/types-info-subsystem) they can apply metadata in an imperative way.

## Imperative simple example

All of the language features of C# (if statements, foreach loops, string interpolation etc) can be used by `ModelBuilders` to apply attributes. 

<<< @/guide/samples/model-builder-convention/DemoNaive.cs{15-28}

The ability to build metadata on the fly opens up a range of possibilities for advanced scenarios such as reading that metadata directly from a configuration file, database, or other runtime modifications.

::: warning
Because XAF reads metadata at application startup you need to restart the application to apply the changes.
:::

## Imperative advanced example

Consider the `Sensors` business object that has a number of similar typed properties named `Value1` to `Value5`.

<<< @/guide/samples/model-builder-convention/Sensor.cs{15,18,21,24,27}

To apply a caption to those properties in an efficient 'imperative' way a `for loop` can be used.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.cs{15-19}


## ConventionBuilder - ForProperties

The code above illustrates the extraordinary power of ModelBuilders but it also demonstrates the ease with which it's possible to create code that could, at some point in the future, become very difficult to refactor. To mitigate this Xenial.Framework has the `ForProperties` construct that can accept a range of properties.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForProperties.cs{15-21}

::: tip
This is especially useful if you only want to apply an attribute to selected properties.
:::

## ConventionBuilder - ForAllProperties

For those occasions when it would be useful to apply an attribute to all the properties of a business object there is the `ForAllProperties` construct.
The code belows illustrates how to set the `AllowEdit` property to false on all of the business object's properties, essentially adding `ModelDefaultAttribute("AllowEdit", "False")`to each one.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForAllProperties.cs{15-16}



::: tip
You also can use the shorthand `NotAllowingEdit()` to achieve the same result.
:::

## ConventionBuilder - Except

If the desired result is to apply an attribute to most but notall of the properties of the business object then the framework has the `Except` method that can be applied to 
`ForAllProperties` which acts acts as a filter. The code below illustrates how only properties `Value2` and `Value4` should be editable, with the rest being un-editable.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForAllProperties.Except.cs{16-19}

::: tip
There are several overloads that make it possible to create more complex filters.
:::

## ConventionBuilder - ForPropertiesOfType

There will be occasions when it would be very useful to apply attributes to a specific data type. The code below demonstrates how it would be possible to apply `DisplayFormat` and `EditMask` to all integer properties.

<<< @/guide/samples/model-builder-convention/Sensor.ModelBuilder.ForPropertiesOfType.cs{15-17}

::: tip
This can be used with the `Except` filter as well!
:::
