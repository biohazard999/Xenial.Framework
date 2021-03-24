---
title: ModelBuilders - Buddy class approach
---

# ModelBuilders - Buddy class approach

The second way you can leverage `ModelBuilders` in your code is using buddy classes. This approach should is the preferred way for model builders. It uses the power of generics to help you write more robust code and make it way more refactoring sage. It's has a little bit more ceremony because you need to write a second class, but it provides all the power of C# as it advantage. 

## Class Level

We use the same sample class as the [inline version](model-builders-inline.md).

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{11-12}

To replace those 2 attributes, we need to create a new derived class from the `ModelBuilder<T>` type and override the `Build()` method to apply any attributes. Afterwards we need to override the `CustomizeTypesInfo` like in the inline approach and register the usage from there. 

<<< @/guide/samples/model-builder-buddy/DemoTaskModelBuilderClassAttributes.cs{7,15-16}

Now we need to register our `ModelBuilder` in the modules `CustomizeTypesInfo` method and we need to call the `Build()` method. We can use the built in `CreateModelBuilder<T>()` extension method on the `ITypesInfo` class. 

<<< @/guide/samples/model-builder-buddy/RegisterModelBuilder.cs{21-23}

Again we use the built in methods `WithDefaultClassOptions` and `HasCaption` to apply those attributes to our business class. Now we can remove the attributes from our code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClass.cs{11-12}

::: tip
As a naming convention we recommend you name your builders with the classname as a prefix and a postfix of `ModelBuilder`.  
Using the file naming convention `ModelClass.ModelBuilder.cs` allows use file naming in VisualStudio.
:::

## Property Level

Next topic is how we apply attributes on property level using the buddy class approach. As you may have guessed its not very different from the class level attributes in the last topic.

<<< @/guide/samples/model-builder-buddy/DemoTaskModelBuilderPropertyAttributes.cs{15-16}

Now we need to register our `ModelBuilder` in the modules `CustomizeTypesInfo` method and we need to call the `Build()` method.

<<< @/guide/samples/model-builder-buddy/RegisterModelBuilder.cs{21-23}

We use the built in method `HasTooltip` to apply the attribute to our business classes property. Now we can remove the attribute from our code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClassProperties.cs{17}

## Summary

Because we use imperative code instead of pure attributes we can apply attributes even to code, which is in a different assembly (we may not have code access to). It's of course possible to use every language feature C# provides to increase maintainability (string interpolation, etc).

Our final code looks like this:

<<< @/guide/samples/DemoTaskModelBuilderModelSummary.cs

<<< @/guide/samples/model-builder-buddy/DemoTaskModelBuilderSummary.cs

<<< @/guide/samples/model-builder-buddy/RegisterModelBuilder.cs