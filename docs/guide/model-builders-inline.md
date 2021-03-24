---
title: ModelBuilders - Inline approach
---

# ModelBuilders - Inline approach

The first way you can leverage `ModelBuilders` in your code is inline. This approach is very useful if you apply just a couple of attributes to your model. It's a quick and dirty way without going through the ceremony of writing a buddy type for your class. 

## Class Level

We will focus now on class level attributes using the inline approach.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{11-12}

To replace those 2 attributes, we need to override the `CustomizeTypesInfo` method and use the static `ModelBuilder.Create<T>()` method to create an inline model builder. It's important to call the `Build` method at the end, to refresh the `ITypeInfo` instance.

But first we need to import the correct namespace by using the `using Xenial.Framework.ModelBuilders;` statement.

<<< @/guide/samples/model-builder-inline/DemoTaskModelBuilderInline.cs{9,21-24}

We use the built in methods `WithDefaultClassOptions` and `HasCaption` to apply those attributes to our business class. Now we can remove the attributes from our code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClass.cs{11-12}

## Property Level

Next topic is how we apply attributes on property level using the inline approach.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{17}

To replace the tooltip attribute, we need to override the `CustomizeTypesInfo` method and use the static `ModelBuilder.Create<T>()` method to create an inline model builder. It returns an instance of a `ModelBuilder` that we can use to apply property attributes as well. We use the `builder.For(member => member.MemberName)` linq like syntax to provide a refactor and type save way to specify a property of our business object.

Make sure we need import the correct namespace by using the `using Xenial.Framework.ModelBuilders;` statement like in the last chapter.

<<< @/guide/samples/model-builder-inline/DemoTaskModelBuilderInlineProperties.cs{9,23-24}

We use the built in method `HasTooltip` to apply the attribute to our business classes property. Now we can remove the attribute from our code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClassProperties.cs{17}

## Summary

Because we use imperative code instead of pure attributes we can apply attributes even to code, which is in a different assembly (we may not have code access to). It's of course possible to use every language feature C# provides to increase maintainability (string interpolation, etc).

Our final code looks like this:

<<< @/guide/samples/DemoTaskModelBuilderModelSummary.cs

<<< @/guide/samples/model-builder-inline/DemoTaskModelBuilderInlineSummaryBuilder.cs

