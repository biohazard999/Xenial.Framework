---
title: DetailViewLayoutBuilders - Simple Layout
---

# DetailViewLayoutBuilders - Simple Layout

As has been stated `Xenial.Framework` is designed to be flexible and  to minimize overheads. This is exemplified by the `simple layout` approach of `LayoutBuilders`.  

The first task is to tell XAF to use the `LayoutBuilders`.  
To do this it is necessary to override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Defining the builder method

With that done it is now necessary to declare a public static method in the business object class for which the layout is to be created called `BuildLayout` that returns a `Xenial.Framework.Layouts.Items.Base.Layout` instance and decorate it with the `DetailViewLayoutBuilderAttribute`.  
The `DetailViewLayoutBuilderAttribute` defines the method and type that is responsible for building the `DetailView`.

<<< @/guide/samples/layout-builders-simple/RegisterSimple1.cs{11,14-17}

After registering the builder and restarting the application(recall that XAF requires an application restart to register changes to metadata) there is now an empty layout becuase as yet there is no code within the `LayoutBuilders` to construct the view.

![Person Void Layout](/images/guide/layout-builders/person-void-layout.png)

::: tip
There are [some overloads for stricter](#other-registrations) registration patterns.
:::

::: warning
If a blank page is not visible at this stage, make sure that the  `Model.DesignedDiffs.xafml` files (also in the `Win` project) for this `DetailView` have **no differences** and be sure to delete or disable the `User differences` file.  
Normally this file is located in the Application output directory called `Model.User.xafml`.
:::

## Building the layout

All the components used to build the layout are normal C# classes and are designed to work well with C#'s initializer syntax as illustrated in the code below. 

<<< @/guide/samples/layout-builders-simple/SimpleLayout.cs

This may appear to be very verbose and long syntax (there are more compact and [advanced syntax](/guide/layout-builders-advanced-registration.md) patterns. See the [reference for the used classes](/guide/layout-builders-reference.md) for more details) and in the next section it will be examined in detail but before that examine the result:

![Person Result Layout](/images/guide/layout-builders/person-result-layout-simple.png)

## Layout-Code-Review

The `Layout` class is the container for the layout. It serves as a generic container for all kind of `LayoutNodes`.

```cs{3}
    public static Layout BuildLayout()
    {
        return new Layout
        {
            /* ... */
        }
    }
```


::: tip
From C#6 it has been possible to use [Expression-bodied members](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members) to shorten the syntax to:

```cs
public static Layout BuildLayout() => new Layout {};
```
:::

The basic building blocks for defining layouts are the `VerticalLayoutGroupItem` and `HorizontalLayoutGroupItem` classes. To define tabbed layouts use the `LayoutTabbedGroupItem` and `LayoutTabGroupItem` classes. To define empty space there is a  special node `LayoutEmptySpaceItem`. 

The table below and the illustration immediatly following it show how the layout is structured.

* <code style='color: red; background-color: transparent;'>VerticalLayoutGroupItem</code> specifies a `LayoutGroupItem` with *vertical aligned children*
* <code style='color: green; background-color: transparent;'>HorizontalLayoutGroupItem</code> specifies a `LayoutGroupItem` with *horizontal aligned children*
* <code style='color: blue; background-color: transparent;'>LayoutTabbedGroupItem</code> A specialized container that holds tabs.
* <code style='color: limegreen; background-color: transparent;'>LayoutTabGroupItem</code> A container that represents a tab. By default *children are aligned vertical*
* <code style='color: gray; background-color: transparent;'>LayoutEmptySpaceItem</code> A special node that takes up the *remaining* empty space.

![Person Layout Structure](/images/guide/layout-builders/person-result-layout-simple-analyze.png)

By default each the nodes in a container will have space allocated to them evenly (two elements would each get 50% of the space, three 33% and so on). Yhis may not be the desired result so this behavious ca be overriden by defining the `RelativeSize` of a node. The `LayoutEmptySpaceItem` acts like any other node and follows the same rules but it also acts as a *layout stretching* mechanism for tab pages, because XAF tries to shrink them by default. 

::: tip
Whilst it is possible to specify any valid `double` value for the `RelativeSize`using percentage values will produce more consistent results.  
:::

The last thing to examine is the `LayoutPropertyEditorItem`. In the constructor you can specify the ID of the `IModelPropertyEditor` node in the detail view. Because of the use of the [`ExpandObjectMembersAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ExpandObjectMembersAttribute), XAF will generate separate property editors for the specified nested objects, for example `Address1.Street`.

::: tip
There are several properties that can specified like `CaptionLocation` and `Caption`, `MinSize`, `MaxSize` etc.  
For group nodes use the `Children` property to initialize them, or use the default `Add` method called by the initializer, if there isn't a requirement to specify any properties.
:::

## Refactoring

The code to create the layout is not that complex in reality but it is repetitive in places (creating the address tabs being a case in point). Because this is using regular C# to define the layout that part could be extracted  into a separate method and called  with `Address1` and `Address2` as a parameter.

<<< @/guide/samples/layout-builders-simple/SimpleLayoutRefactor.cs{53,58,69-98}

::: tip
Whilst it isn't mandatory the use string interpolation to specify the property names is recommended to facilitate easier and safer refactoring.

By using the base class `LayoutItem` as a return value future maintenance costs are reduced, because it is possible to change the internals of the `CreateAddressGroup` method, without the need to update it's usage.
:::

## Other registrations

If the convention based `BuildLayout` is not suitable , there is the option to provide an custom method name by passing it as a parameter to the `DetailViewLayoutBuilderAttribute`.

<<< @/guide/samples/layout-builders-simple/RegisterSimple2.cs{11,14}

::: tip
The creation of layouts in code can lead to large code files. Layout code can be moved to a separate file using the [`partial class` pattern](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods).  
:::

LayoutBuilders  can be created in a separate class if for example,  there is a requirement  to split XPO/XAF into separate assemblies, by providing the type of the class:

<<< @/guide/samples/layout-builders-simple/RegisterSimple3.cs{11,14-20}


<!-- I get what you're trying to say in the tip below but there is either a sentence missing or better yet a small code sample missing -->

::: tip
The convention based naming approach also works for external types by just removing the target method name `[DetailViewLayoutBuilder(typeof(PersonLayouts))]`.  
Then of course the method name would be `BuildLayout` in the `PersonLayouts` class.
:::

