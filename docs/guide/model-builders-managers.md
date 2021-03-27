---
title: ModelBuilders - Managers
---

# ModelBuilders - Managers

In our last examples we directly registered the managers in the `CustomizeTypesInfo` method. Because there could be a lot of `ModelBuilders` in a module, this becomes quickly a maintenance issue. That's why `Xenial.Framework` provides a solution for this problem: `BuilderManagers`.

## XafBuilderManager

We derive from the `XafBuilderManager` class and override the `GetBuilders` method to create our instance of the `DemoTaskModelBuilder` like we learned in the last chapter.

<<< @/guide/samples/model-builder-manager/DemoTaskModelBuilderManager.cs{19-22}

## Registration

Afterwards we need to register the `DemoTaskModelBuilderManager` in the `CustomizeTypesInfo` method and call `Build`.

<<< @/guide/samples/model-builder-manager/RegisterModelBuilder.cs{21-22}

## Summary

As you can see, its not difficult to use `BuilderManagers`. 

::: tip
`BuilderManagers` are the preferred way to work with `ModelBuilders`. There are some built-in optimizations that try to minimize the performance overhead.
:::
