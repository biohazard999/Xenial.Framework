---
title: ModelBuilders - BuilderManagers
---

# ModelBuilders - BuilderManagers

In the previous examples `ModelBuilders` were registered directly in the `CustomizeTypesInfo` method. As the number of ModelBuilders in an XAF module grows this has the potential to become a serious maintenace issue. To avoid this problem Xenial.Framework provides `BuilderManagers`. 

## XafBuilderManager

To utilze `BuilderManagers` create a new class (by convention name it <ModuleName>BuilderManager) and derive it from the `XafBuilderManager` class. Within the class override the `GetBuilders` method to create an instance of the `DemoTaskModelBuilder`. 

<<< @/guide/samples/model-builder-manager/DemoTaskModelBuilderManager.cs{19-22}

## Registration

With that done register the `DemoTaskModelBuilderManager` in the `CustomizeTypesInfo` method and call `Build`.

<<< @/guide/samples/model-builder-manager/RegisterModelBuilder.cs{21-22}

## Summary

`BuilderManagers` are simple to use and the recommended way of working with `ModelBuilders`. 

::: tip
`BuilderManagers` have built-in optimizations to minimize any performance overhead.
:::
