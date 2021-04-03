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

## Refactoring

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

