---
title: 'ListViewColumnBuilders - ModelBuilder Syntax'
---

# ListViewColumnBuilders - `ModelBuilder` Syntax

`ListViewColumnBuilders` are fully integrated into [ModelBuilders](/guide/model-builders.md).

## Registration

Registration is exactly the same as before, override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseListViewColumnBuilders()` extension method.

<<< @/guide/samples/column-builders-simple/RegisterInModule.cs{8,12}

## Build the Columns

As with [ModelBuilders](/guide/model-builders.md) this example will use fluent syntax combined with the `WithListViewColumns`.

<!--Once again the font size of this detail tip seems out of kilter -->

> The overload `WithListViewColumns` takes a signature of `BuildColumnsFunctor` which is a method that just returns a `Columns` object. The second overload provides a [`ColumnsBuilder<T>`](/guide/column-builders-advanced-syntax.md) obviating the need to create the builder independently.

<<< @/guide/samples/column-builders-model-builders/Module.cs{31,32}

As `ModelBuilders` are used to add the attribute to the `Person` class, there is no requirement to specify anything special:

<<< @/guide/samples/layout-builders-simple/Person.cs

::: tip
This is the optimal way to add Columns to classes to which source access is either unavailable or when it is desirable to keep the model clean from attributes.
:::
