---
title: DetailViewLayoutBuilders - Advanced Syntax
---

# DetailViewLayoutBuilders - Advanced Syntax

As we learned in the last chapter, layouts are basically *just* a domain specific language around a couple of building blocks. By using the power of C# we are able to write our layouts in code, produce pixel perfect results and be refactoring safe for a more consistent application look and feel.  
However, the basic building blocks are a little bit verbose to write, so `Xenial.Framework` provides a more robust and IntelliSence driven way to write layouts.

As a quick reminder our final layout should look like in the past example:

![Person Result Layout](/images/guide/layout-builders/person-result-layout-simple.png)

## Registration

The registration in the module is exactly the same as in the last chapter, we need to tell XAF to use the `LayoutBuilders`.  
For this we need to override the `AddGeneratorUpdaters` in our platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.  
Nothing additional is needed to use the advanced syntax

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Defining the builder method

Next step is that we need to declare an public static method in the class we want to provide a layout for called `BuildLayout` that returns an `Xenial.Framework.Layouts.Items.Base.Layout` instance and decorate it with the `DetailViewLayoutBuilderAttribute`.  
The `DetailViewLayoutBuilderAttribute` defines the method and type that is responsible for building our `DetailView`.  
This step is exactly the same as in the last chapter.

<<< @/guide/samples/layout-builders-simple/RegisterSimple1.cs{11,14-17}

## Using the `LayoutBuilder<T>`

However, this time we are not using the initializer syntax and hand craft our object graph. Instead we are going to use our new friend, the `LayoutBuilder<T>` class:

<<< @/guide/samples/layout-builders-advanced/Intro.cs{18}

