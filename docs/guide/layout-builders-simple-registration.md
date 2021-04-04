---
title: DetailViewLayoutBuilders - Simple registration
---

# DetailViewLayoutBuilders - Simple registration

Because `Xenial.Framework` is designed to be flexible and tries to minimize your overhead let's start with the first way of using `LayoutBuilders` with the `simple registration` approach.  

But before we dig into the details of building a layout, we need to tell XAF to use the `LayoutBuilders`.  
For this we need to override the `AddGeneratorUpdaters` in our platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Defining the builder method

Next step is that we need to declare an public static method in the class we want to provide a layout for called `BuildLayout` that returns an `Xenial.Framework.Layouts.Items.Base.Layout` instance and decorate it with the `DetailViewLayoutBuilderAttribute`.  
The `DetailViewLayoutBuilderAttribute` defines the method and type that is responsible for building our `DetailView`.

<<< @/guide/samples/layout-builders-simple/RegisterSimple1.cs{11,14-17}

After registering our builder and restart the application we should see an empty layout, but no worries we will fill the void in just a moment.

![Person Void Layout](/images/guide/layout-builders/person-void-layout.png)

::: tip
If you don't like this convention there are [some overloads for stricter](#other-registrations) registration patterns.
:::

::: warning
If your don't see a blank page at this stage, make sure your `Model.DesignedDiffs.xafml` files (also in the `Win` project) for this `DetailView` has **no differences** and make sure to delete or disable the `User differences` file.  
Normally this file is located in the Application output directory called `Model.User.xafml`.
:::

## Building the layout

All the components used to build the layout are normal C# classes and are designed to work well with C#'s initializer syntax. However there are more compact and [advanced syntax](/guide/layout-builders-advanced-registration.md) patterns. See the [reference for the used classes](/guide/layout-builders-reference.md) for more details.

<<< @/guide/samples/layout-builders-simple/SimpleLayout.cs

Don't panic on this very verbose and long syntax. We will go through the basics one by one, but first look at the result:

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
Since C#6 we can use [Expression-bodied members](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members) to shorten the syntax to:

```cs
public static Layout BuildLayout() => new Layout {};
```
:::

The basic building blocks for defining layouts are the `VerticalLayoutGroupItem` and `HorizontalLayoutGroupItem` classes. To define tabbed layouts we use the `LayoutTabbedGroupItem` and `LayoutTabGroupItem` classes. A special node is the `LayoutEmptySpaceItem` to specify free room. Let's look at how this layout is structured:

* <code style='color: red; background-color: transparent;'>VerticalLayoutGroupItem</code> specifies a `LayoutGroupItem` with *vertical aligned children*
* <code style='color: green; background-color: transparent;'>HorizontalLayoutGroupItem</code> specifies a `LayoutGroupItem` with *horizontal aligned children*
* <code style='color: blue; background-color: transparent;'>LayoutTabbedGroupItem</code> A specialized container that holds tabs.
* <code style='color: limegreen; background-color: transparent;'>LayoutTabGroupItem</code> A container that represents a tab. By default *children are aligned vertical*
* <code style='color: gray; background-color: transparent;'>LayoutEmptySpaceItem</code> A special node that takes up the *remaining* empty space.

![Person Layout Structure](/images/guide/layout-builders/person-result-layout-simple-analyze.png)

By default each node in a container takes up space evenly. So if you define 2 elements they will take up 50% each, for 3 they will take 33%, for 4 25% and so on. You can override this behavior by defining the `RelativeSize` of a node. The `LayoutEmptySpaceItem` acts like any node, so it follows the rules mentioned earlier, but it also acts as a *layout stretching* mechanism for tab pages, because XAF tries to shrink them by default. 

::: tip
You can specify any valid `double` value for the `RelativeSize`, but using percentage values turned out to be most consistent.  
:::

The only thing left is the `LayoutPropertyEditorItem`. In the constructor your can specify the ID of the `IModelPropertyEditor` node in the detail view. Because we used the [`ExpandObjectMembersAttribute`](https://docs.devexpress.com/eXpressAppFramework/DevExpress.Persistent.Base.ExpandObjectMembersAttribute), XAF will generate separate property editors for the specified nested objects for example `Address1.Street`.

::: tip
There are several properties that you can specify like `CaptionLocation` and `Caption`, `MinSize`, `MaxSize` etc.  
For group nodes you can use the `Children` property to initialize them, or use the default `Add` method called by the initializer, if you don't need to specify any properties.
:::

## Refactoring

The code so far looks not that bad, but as you clearly can see we are repeating our selfs with the `Address` pages. Because we are using regular C# to define the layout, we can extract this part into a separate method and call it with `Address1` and `Address2`.

<<< @/guide/samples/layout-builders-simple/SimpleLayoutRefactor.cs{56,61,72-101}

::: tip
Using string interpolation to specify the property names and stay refactoring safe is not mandatory but recommended.  
By using the base class `LayoutItem` as a return value helps reducing maintenance costs in the future, because you can change the internals of the `CreateAddressGroup` method, without the need of updating it's usage.
:::

## Other registrations

If you don't like the convention based `BuildLayout`, you also can provide an custom method name by passing it as a parameter to the `DetailViewLayoutBuilderAttribute`.

<<< @/guide/samples/layout-builders-simple/RegisterSimple2.cs{11,14}

::: tip
Because layouts can be big in code size, you can move your layout to a separate file using the [`partial class` pattern](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods).  
:::

If you want to keep your builder in a separate class instead (for example if you want to split XPO/XAF into separate assemblies), you also can provide the type of the class:

<<< @/guide/samples/layout-builders-simple/RegisterSimple3.cs{11,14-20}

::: tip
The convention based naming approach also works for external types, by just removing the target method name `[DetailViewLayoutBuilder(typeof(PersonLayouts))]`.  
Then of course the method name would be `BuildLayout` in the `PersonLayouts` class.
:::

