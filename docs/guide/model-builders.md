---
title: ModelBuilders
sidebarDepth: 5
---

# ModelBuilders - Introduction

Modelbuilders are an [imperative/procedural](https://en.wikipedia.org/wiki/Imperative_programming) way of adding metadata and attributes to the excelent [XAF TypesInfo system](https://docs.devexpress.com/eXpressAppFramework/113669/concepts/business-model-design/types-info-subsystem).

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

There are several ways to use `ModelBuilders` in your application. From a fluent inline approach to complete [buddy type](https://stackoverflow.com/a/38373456/2075758). That means we have a secondary utility class to specify the metadata for a business object.

Imaging we have the following [XPO business class](https://docs.devexpress.com/eXpressAppFramework/113640/getting-started/in-depth-tutorial-winforms-aspnet/business-model-design/business-model-design-with-express-persistent-objects) based on the Contact/Task Management XAF Demo.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs

::: tip
NonPersistent classes and EntityFramework do also work, but for simplicity we will focus on XPO business objects in this docs.
:::

### Naming conventions

The naming convention tries to be self explanatory.  
If the attribute is singular and describes *an attribute of* a business object it starts with the term `Has`.  
If the attribute is plural or describes *behavior of* a business object it starts with the term `With`.

::: tip
There are a lot of [Built-in Attributes](/guide/model-builders-built-in.md) provided by Xenial.  If you are missing one, that should be built into the framework, [let us know](https://github.com/xenial-io/Xenial.Framework/issues/)!
:::