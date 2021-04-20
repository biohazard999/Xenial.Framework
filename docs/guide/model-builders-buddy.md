---
title: ModelBuilders - Buddy class approach
---

# ModelBuilders - Buddy class approach

The second way to leverage `ModelBuilders` in code is through the use of 'buddy' classes. This should be considered the prefered approach for model builders as it realises the power of generics ensuring more robust code and substantially easier refactoring. It requires more effort because of the need to write a second class but in doing one can take full advantage of the power bestowed by C#.

As in the documentation on the inline approach the same basic business object will be used as an example.

## Class Level


<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{11-12}

Replacing the two class level attributes requires the creation of a new derived class from the `ModelBuilder<T>` type and then overriding it's `Build()` method to apply the attributes. As before use the built in methods `WithDefaultClassOptions` and `HasCaption` to apply those attributes to the business class.

<<< @/guide/samples/model-builder-buddy/DemoTaskModelBuilderClassAttributes.cs{7,15-16}

With that done, as before with the inline approach, it is necessary to register the `ModelBuilder` in the `CustomizeTypesInfo` method and then to call the `Build()` method. This can be done with the built in `CreateModelBuilder<T>()` extension method on the `ITypesInfo` class. 

<<< @/guide/samples/model-builder-buddy/RegisterModelBuilder.cs{21-23}

 Now the attributes can be removed from the business object's code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClass.cs{11-12}

::: tip
By following the recommended naming covention of `ModelClass.ModelBuilder.cs` for  these model builders and applying the suffix ModelBuilder to the original class name  
one can make full use of file naming in VisualStudio.
:::

## Property Level

Property level attributes follow exactly the same convention.

Register the `ModelBuilder` in the module's `CustomizeTypesInfo` method and call the `Build()` method.

<<< @/guide/samples/model-builder-buddy/DemoTaskModelBuilderPropertyAttributes.cs{15-16}

Use the built in method `HasTooltip` to apply the attribute to the business class's property.

<<< @/guide/samples/model-builder-buddy/RegisterModelBuilder.cs{21-23}

Remove the attribute from the original code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClassProperties.cs{17}

## Summary

As with the inline approach this allows the application of attributes to code that may reside in a different assembly and to which access may not be possible whislt leveraging all of the benefits that C# has to offer.

## In Conclusion

On completion there should be three distinct classes as illustrate below.

<<< @/guide/samples/DemoTaskModelBuilderModelSummary.cs

<<< @/guide/samples/model-builder-buddy/DemoTaskModelBuilderSummary.cs

<<< @/guide/samples/model-builder-buddy/RegisterModelBuilder.cs
