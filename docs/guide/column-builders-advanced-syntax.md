---
title: 'ListViewColumnBuilders - ColumnsBuilder<T> Syntax'
---

# ListViewColumnBuilders - `ColumnsBuilder<T>` Syntax

The last section demonstrated that columns are essentially domain specific language which can, through the power of C#, produce in code, pixel perfect results which are both refactor safe and have a consistent look and feel.

Whilst the basic column building blocks can be verbose `Xenial.Framework` does provide a more robust and IntelliSense driven way to craft columns in code.

By way of a quick reminder the illustration below shows what the final layout should look like;

![Person Target Columns](/images/guide/column-builders/person-target-columns.png)

## Registration

As before the first task is to tell XAF to use the `ListViewColumnBuilders`.

Override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseListViewColumnBuilders()` extension method.

<<< @/guide/samples/column-builders-simple/RegisterInModule.cs{8,12}

## Defining the builder method

Once again declare a public static method in the business object class for which the layout is to be created called `BuildColumns` that returns a `Xenial.Framework.Layouts.Columns` instance and decorate the business object with the `ListViewColumnsBuilderAttribute`.

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

## Using the `ColumnsBuilder<T>` instance

At this stage the workflow moves from using the initializer syntax to hand craft the object graph to using the `ColumnsBuilder<T>` class as illustrated below;

<<< @/guide/samples/column-builders-advanced/Intro.cs{15}

The column builder has a number of methods and overloads that accept some basic parameters like `Caption` and `Width` first, followed by a callback method `Action<Column>`.  

These allow the use of a more compact syntax, without loosing any functionality over the traditional initializer syntax.

> In the code sample below the parameters `m` and `c` are used, where `m` is short for `Member` and `c` for `Column`.  
This convention is regarded as best practice but in reality any naming convention could be used.

<<< @/guide/samples/column-builders-advanced/BuilderInstance.cs

The benefit of this approach is that IntelliSense is available to guide the column building process obviating the need to remember all the type names, however it is a much denser syntax which may not be to all tastes.

::: tip
It is perfectly acceptable to mix both initializer and functional style to suit personal preference or team guidelines.  
:::

## Inherit from `ColumnsBuilder<T>`

Thus far the `ColumnsBuilder<T>` has been used as an instance utilizing the *convention based* registration pattern for the builder.

By inheriting from `ColumnsBuilder<T>` and using the `typed` overload of the `ListViewColumnsBuilderAttribute` it is possible to reduce additional noise from the syntax.  This is achieved by inheriting from the `ColumnsBuilder<T>` class and changing the registration as shown below.

<<< @/guide/samples/layout-builders-advanced/BuilderInherit.cs
