---
title: 'DetailViewLayoutBuilders - ModelBuilder Syntax'
---

# DetailViewLayoutBuilders - `ModelBuilder` Syntax

If you are using [ModelBuilders](/guide/modelbuilders.md) we of course provide deep integration here as well.

## Registration

The registration in the module is exactly the same as in the last chapter, we need to tell XAF to use the `LayoutBuilders`.  
For this we need to override the `AddGeneratorUpdaters` in our platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.  
Nothing additional is needed to use the record syntax.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Build the Layout

As we learned in the [ModelBuilders](/guide/modelbuilders.md) chapter how to integrate them, we will use the fluent syntax and use the `WithDetailViewLayout` for this sample.

> The overload `WithDetailViewLayout` takes an signature of `BuildLayoutFunctor` which is a method that just returns a `Layout` object. The second overload provides us handily a [`LayoutBuilder<Person>`](/guide/layout-builders-advanced-syntax.md). That way we don't need to create the builder our self.

<<< @/guide/samples/layout-builders-model-builders/Module.cs{27,28}

Because we use `ModelBuilders` to add the attribute to our `Person` class, we don't need to specify anything special:

<<< @/guide/samples/layout-builders-simple/Person.cs

::: tip
This is the optimal way of adding Layouts to your classes you don't have source access to or want to keep your model clean from attributes.
:::