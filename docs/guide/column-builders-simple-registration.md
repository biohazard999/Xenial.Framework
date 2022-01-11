---
title: ListViewColumnBuilders - Simple Columns
---

# ListViewColumnBuilders - Simple Columns

As has been stated `Xenial.Framework` is designed to be flexible and  to minimize overheads. This is exemplified by the `simple columns` approach of `ListViewColumnBuilders`.  

The first task is to tell XAF to use the `ColumnBuilders`.  

Override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseListViewColumnBuilders()` extension method.

<<< @/guide/samples/column-builders-simple/RegisterInModule.cs{8,12}

## Defining the builder method

With that done declare a public static method in the business object class for which the layout is to be created, called `BuildColumns`, that returns a `Xenial.Framework.Layouts.Columns` instance and decorate the business object with the `ListViewColumnsBuilderAttribute`.

The `ListViewColumnsBuilderAttribute` defines the method and type that is responsible for building the `ListView`.

<<< @/guide/samples/column-builders-simple/RegisterSimple1.cs{10,13-16}

After registering the builder and restarting the application (recall that XAF requires an application restart to register and apply changes to metadata) there is now an empty ListView because as yet there is no code within the `ListViewColumnBuilders` to construct the view.

![Person Void Columns](/images/guide/column-builders/person-void-columns.png)

::: tip
There are [some overloads for stricter](#other-registrations) registration patterns.
:::

::: warning
If a blank page is not visible at this stage, make sure that the  `Model.DesignedDiffs.xafml` files (also in the `Win` project) for this `ListView` have **no differences** and be sure to delete or disable the `User differences` file.  

This file is usually located in the Application output directory called and named `Model.User.xafml`.
:::

## Building the columns

All the components used to build the columns are normal C# classes and have been designed to work well with C#'s initializer syntax as illustrated in the code below.

<<< @/guide/samples/column-builders-simple/SimpleColumns.cs

This may appear to be a very verbose and long syntax pattern (Xenial.Framework does provide a more compact and [advanced syntax](/guide/column-builders-advanced-registration.md) patterns, see the [reference for the used classes](/guide/column-builders-reference.md) for more details) which will be examined in greater detail shortly.

Before that examination look at the result:

![Person Target Layout](/images/guide/column-builders/person-target-columns.png)

## Columns-Code-Review

The `Columns` class is the container for the columns.

```cs{3}
    public static Columns BuildColumns()
    {
        return new Columns
        {
            /* ... */
        }
    }
```

::: tip
From C#6 it has been possible to use [Expression-bodied members](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members) to shorten the syntax to:

```cs
public static Columns BuildColumns() => new Columns {};
```
:::

The basic building blocks for defining layouts are currently only the `ListViewOptions` and `Column` classes.

```cs{4,10}
    public static Columns BuildColumns()
    {
        return new Columns(
            new ListViewOptions
            {
                /* ... */
            }
        )
        {
            new Column("PropertyName")
            {
                /* ... */
            }
        }
    }
```

::: tip
There are several `Column` properties that can be specified like `Width` and `SortOrder`, `GroupIndex`, `DisplayFormat` etc.  

The `Columns` class is a collection. So you can use initializer syntax, or use the default `Add` method called by the initializer.
:::

## Other registrations

If the convention based `BuildColumns` is not suitable , there is the option to provide a custom method name by passing it as a parameter to the `ListViewColumnsBuilderAttribute`.

<<< @/guide/samples/column-builders-simple/RegisterSimple2.cs{10,13}

::: tip
The creation of columns in code can lead to large code files. Layout code can be moved to a separate file using the [`partial class` pattern](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods).  
:::

`ListViewColumnBuilders`
 can be created in a separate class if, for example, there is a requirement to split XPO/XAF into separate assemblies, by providing the type of the class:

<<< @/guide/samples/column-builders-simple/RegisterSimple3.cs{10,13-19}

<!-- I get what you're trying to say in the tip below but there is either a sentence missing or better yet a small code sample missing -->

::: tip
The convention based naming approach also works for external types by just removing the target method name `[ListViewColumnsBuilder(typeof(PersonColumns))]`.  
Then of course the method name would be `BuildColumns` in the `PersonColumns` class.
:::

## Declaring LookupListViews

Because LookupListViews are basically just ListViews that follow a specific naming convention Xenial.Framework provides a convenience attribute that has it's own convention by using the `LookupListViewColumnBuilderAttribute` which expects an `BuildLookupColumns` method, but otherwise follows the exact same semantics of the `ListViewColumnsBuilderAttribute`.

<<< @/guide/samples/column-builders-simple/RegisterSimpleLookup.cs{10,13}
