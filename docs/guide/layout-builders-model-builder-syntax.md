---
title: 'DetailViewLayoutBuilders - ModelBuilder Syntax'
---

# DetailViewLayoutBuilders - `ModelBuilder` Syntax

LayoutBuilders are fully integrated into [ModelBuilders](/guide/modelbuilders.md).

## Registration

Registration is exactly the same as before, override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Build the Layout

As with [ModelBuilders](/guide/modelbuilders.md) this example will use  fluent syntax combined with the `WithDetailViewLayout`.

> The overload `WithDetailViewLayout` takes a signature of `BuildLayoutFunctor` which is a method that just returns a `Layout` object. The second overload provides a [`LayoutBuilder<T>`](/guide/layout-builders-advanced-syntax.md) obviating the need to create the builder independently.

<<< @/guide/samples/layout-builders-model-builders/Module.cs{27,28}

As  `ModelBuilders` are used to add the attribute to the `Person` class, there is no requirement to specify anything special:

<<< @/guide/samples/layout-builders-simple/Person.cs

::: tip
This is the optimal way to add Layouts to to classes to which source access is either unavailable or when it is desirable to keep the model clean from attributes.
:::
