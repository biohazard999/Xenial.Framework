---
title: ModelBuilders - Inline approach
---

# ModelBuilders - Inline approach

This approach is very useful when applying  just a couple of attributes to a business object. It is both quick and avoids the requirement to create an additional  (buddy)class.

## Class Level

The following code samples illustrate the use of inline ModelBuilders to replace Class Level attributes.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{11-12}

In order to replace the two class level attributes it is necessary to override the 'CustomTypesInfo' method utilising the 'ModelBuilder.Create<T>()' method to create an inline 'ModelBuilder'.
    
::: tip
It is a requirement that the 'ModelBuilder' 'Build' method is called at the end in order to refresh the 'ITypesInfo' instance.
:::

Before anything else import the correct namespace by using the `using Xenial.Framework.ModelBuilders;` statement.

<<< @/guide/samples/model-builder-inline/DemoTaskModelBuilderInline.cs{9,21-24}

Using the 'ModelBuilder' built-in methods `WithDefaultClassOptions` and `HasCaption` those attributes can be applied to the business class and once that has been done the attributes themselves can be removed from the business object's code. 

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClass.cs{11-12}

## Property Level

Property level attributes require a slightly different approach.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{17}

Once again the 'CustomizeTypesInfo' is overidden but in this instance the static `ModelBuilder.Create<T>()` method is called to create an inline model builder. It returns an instance of a `ModelBuilder` that can be used to apply property attributes as well. Use the `builder.For(member => member.MemberName)` linq like syntax to provide a refactor and type safe way to specify the property of the business object for which the attribute is to be removed.

As before add a reference to the  correct namespace by using the `using Xenial.Framework.ModelBuilders;` statement.

<<< @/guide/samples/model-builder-inline/DemoTaskModelBuilderInlineProperties.cs{9,23-24}

Use the built in method `HasTooltip` to apply the attribute to the business class property. Once done the attribute can be removed from the code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClassProperties.cs{17}

## Summary

The use of 'imperative' code over traditional attributes allows attributes to be applied to code which may be in a different assembly and potentially inaccesible, whilst retaining all the advantages profferd by C# to increase maintainability (string interpolation, etc).

On completion the business object's code will be as follows:

<<< @/guide/samples/DemoTaskModelBuilderModelSummary.cs

<<< @/guide/samples/model-builder-inline/DemoTaskModelBuilderInlineSummaryBuilder.cs

