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

