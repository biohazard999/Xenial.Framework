---
title: 'DetailViewLayoutBuilders - LayoutBuilder<T> Syntax'
---

# DetailViewLayoutBuilders - `LayoutBuilder<T>` Syntax

The last section demonstrated that layouts are essentially domain specific language which can, through the power of C#, produce in code pixel perfect results which are both refactor safe and have a consistent look and feel.

Whilst the basic layout building blocks can be verbose `Xenial.Framework` does provide a more robust and IntelliSence driven way to craft layouts in code.

By way of a quick reminder the illustration below shows what the final layout should look like;

![Person Result Layout](/images/guide/layout-builders/person-result-layout-simple.png)

## Registration

As before the first task is to tell XAF to use the `LayoutBuilders`.  
Override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Defining the builder method

Once again declare a public static method in the business object class for which the layout is to be created called `BuildLayout` that returns a `Xenial.Framework.Layouts.Items.Base.Layout` instance and decorate it with the `DetailViewLayoutBuilderAttribute`.  
The `DetailViewLayoutBuilderAttribute` defines the method and type that is responsible for building the `DetailView`.

<<< @/guide/samples/layout-builders-simple/RegisterSimple1.cs{11,14-17}

## Using the `LayoutBuilder<T>` instance

At this stage the workflow moves from using the initializer syntax to hand craft the object graph to using  the `LayoutBuilder<T>` class as illustrated below;

<<< @/guide/samples/layout-builders-advanced/Intro.cs{18}

The layout builder has a number of methods and overloads that accept some basic parameters like `Caption` and `ImageName` first, followed by a `params LayoutItemNode[] nodes`, as well as a callback method `Action<TNodeType>`.  
These allow the use of a more compact syntax, without loosing any functionality over the traditional initializer syntax.

> In the code sample below the parameters `m` and `e` are used, where `m` is short for `Member` and `e` for `Editor`.  
This is regarded as best practice convention but in reality any naming convention could be used.

<<< @/guide/samples/layout-builders-advanced/BuilderInstance.cs

The benefit of this approach is that IntelliSense is available to guide the layout building process obviating the need to remember all the type names. It is a much denser syntax which may not be to all tastes.

::: tip
It is perfectly acceptable to mix both initializer and functional style to suit personal preference or team guildines.  
:::

## Inherit from `LayoutBuilder<T>` 

Thus far the `LayoutBuilder<T>` has been used as an instance utilizing the *convention based* registration pattern for the builder. 

By inheriting from `LayoutBuilder<T>` and using the `typed` overload of the `DetailViewLayoutBuilderAttribute` it is possible to reduce additional noise from the syntax.  This is achieved by inheriting from the `LayoutBuilder<T>` class and changing the registration as shown below.

<<< @/guide/samples/layout-builders-advanced/BuilderInherit.cs
