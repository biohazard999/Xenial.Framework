---
title: ModelBuilders
sidebarDepth: 5
---

# ModelBuilders

Modelbuilders are an imperative way of adding metadata and attributes to the excelent [XAF TypesInfo system](https://docs.devexpress.com/eXpressAppFramework/113669/concepts/business-model-design/types-info-subsystem).

It's based around an [Fluent Interface Pattern](https://www.martinfowler.com/bliki/FluentInterface.html) and highly inspired by [EntityFramework's ModelBuilder's](https://docs.microsoft.com/en-us/ef/core/modeling/).

ModelBuilders are one of the many `NonVisual Components` of Xenial.Framework and designed around best practices and working most efficiently in a team, however there are several benefits for smaller teams and projects as well.

## Installation

In you [platform agnostic module](https://docs.devexpress.com/eXpressAppFramework/118045/concepts/application-solution-components/application-solution-structure#projects) install the [Xenial.Framework](https://www.nuget.org/packages/Xenial.Framework/) package.

<code-group>
<code-block title=".NET CLI">

<div class="language-bash"><pre class="language-bash"><code>dotnet add package Xenial.Framework --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>


<code-block title="PackageReference">

<div class="language-xml"><pre class="language-xml"><code>&ltPackageReference Include="Xenial.Framework" Version="{{ $var['xenialVersion'] }}" /&gt</code></pre></div>

</code-block>

<code-block title="Package Manager">

<div class="language-powershell"><pre class="language-powershell"><code>Install-Package Xenial.Framework -Version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

<code-block title="Paket CLI">

<div class="language-bash"><pre><code>paket add Xenial.Framework --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

</code-group>

::: tip
In your project this normally is called `MyApplication.Module.csproj`.  
You of course can use the [NuGet Package Manager Dialog in Visual Studio](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio).  
Usage in platform specific module is of course supported, but we will focus on the platform agnostic perspective in this guide.
:::

## Usage

There are several ways to use `ModelBuilers` in your application. From a fluent inline approach to complete [buddy type](https://stackoverflow.com/a/38373456/2075758).

Imaging we have the following [XPO business class](https://docs.devexpress.com/eXpressAppFramework/113640/getting-started/in-depth-tutorial-winforms-aspnet/business-model-design/business-model-design-with-express-persistent-objects) based on the Contact/Task Management XAF Demo.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs

::: tip
EntityFramework classes should also work fine, but arn't officially supported yet.
:::

### Inline approach

The first way you can leverage `ModelBuilders` in your code is inline. This approach is very useful if you apply just a couple of attributes to your model. It's a quick and dirty way without going through the ceremony of writing a buddy type for your class. 

#### Class Level

We will focus now on class level attributes using the inline approach.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{10-11}

To replace those 2 attributes, we need to override the `CustomizeTypesInfo` method and use the static `ModelBuilder.Create<T>()` method to create an inline model builder. It's important to call the `Build` method at the end, to refresh the `ITypeInfo` instance.

But first we need to import the correct namespace by using the `using Xenial.Framework.ModelBuilders;` statement.

<<< @/guide/samples/DemoTaskModelBuilderInline.cs{9,21-24}

We use the built in methods `WithDefaultClassOptions` and `HasCaption` to apply those attributes to our business class. Now we can remove the attributes from our code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClass.cs{10-11}

#### Property Level

Next topic is how we apply attributes on property level using the inline approach.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs{18}

To replace the tooltip attribute, we need to override the `CustomizeTypesInfo` method and use the static `ModelBuilder.Create<T>()` method to create an inline model builder. It returns an instance of a `ModelBuilder` that we can use to apply property attributes as well. We use the `builder.For(member => member.MemberName)` linq like syntax to provide a refactor and type save way to specify a property of our business object.

Make sure we need import the correct namespace by using the `using Xenial.Framework.ModelBuilders;` statement like in the last chapter.

<<< @/guide/samples/DemoTaskModelBuilderInlineProperties.cs{9,23-24}

We use the built in method `HasTooltip` to apply the attribute to our business classes property. Now we can remove the attribute from our code.

<<< @/guide/samples/DemoTaskHighlightAfterModelBuilderClassProperties.cs{18}

#### Summary

Because we use imperative code instead of pure attributes we can apply attributes even to code, which is in a different assembly (we may not have code access to). It's of course possible to use every language feature C# provides to increase maintainability (string interpolation, etc).

Our final code looks like this:

<<< @/guide/samples/DemoTaskModelBuilderInlineSummary.cs

<<< @/guide/samples/DemoTaskModelBuilderInlineSummaryBuilder.cs

### Naming conventions

The naming convention tries to be self explanatory.  
If the attribute is singular and describes *an attribute of* a business object it starts with the term `Has`.  
If the attribute is plural or describes *behavior of* a business object it starts with the term `With`.

::: tip
There are a lot of built in attributes provided by Xenial.  If you are missing one, that should be built into the framework, [let us know](https://github.com/xenial-io/Xenial.Framework/issues/)!
:::