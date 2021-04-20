---
title: ModelBuilders
sidebarDepth: 5
---

# ModelBuilders - Introduction

Xenial. Framework's Modelbuilders follow an [imperative/procedural](https://en.wikipedia.org/wiki/Imperative_programming) method to add metadata and attributes to the excelent [XAF TypesInfo system](https://docs.devexpress.com/eXpressAppFramework/113669/concepts/business-model-design/types-info-subsystem).

They are based upon a [Fluent Interface Pattern](https://www.martinfowler.com/bliki/FluentInterface.html) highly influenced by [EntityFramework's ModelBuilder's](https://docs.microsoft.com/en-us/ef/core/modeling/).

ModelBuilders are one of the many `NonVisual Components` of Xenial.Framework designed to promote best practice and efficient working in large teams, which in turn are equally applicable to smaller teams and projects and even individuals working on a single application.

## Installation

In your [platform agnostic module](https://docs.devexpress.com/eXpressAppFramework/118045/concepts/application-solution-components/application-solution-structure#projects) install the [Xenial.Framework](https://www.nuget.org/packages/Xenial.Framework/) package.

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

::: tip INFORMATION
By convention the platform agnostic module is usually named `<Your Application>.Module`.
If you're unfamiliar with the Command Line Interface (cli) you can always use the Nuget package manager.

Whilst the Xenial.Framework can of course be used in platform specific modules, for the purposes of this documentation emphasis will be given to its use in the platform agnostic module of your project.
:::

## Using ModelBuilders

There are several ways to use `ModelBuilders` in your application ranging from a fluent inline approach to a complete [buddy type](https://stackoverflow.com/a/38373456/2075758) ( essentially a secondary utility class that specifies the metadata for a business object).

To illustrate these approaches to using ModelBuilders consider the following [XPO business class](https://docs.devexpress.com/eXpressAppFramework/113640/getting-started/in-depth-tutorial-winforms-aspnet/business-model-design/business-model-design-with-express-persistent-objects) based on the Contact/Task Management XAF Demo.

<<< @/guide/samples/DemoTaskBeforeModelBuilder.cs

::: tip INFORMATION
Xenial.Framework ModelBuilders can also be used in NonPersistent classes and EntityFramework classes but for clarity this documentation will concentrate on expressPersistentObjects (XPO) business objects..
:::

### Naming Conventions

Before continuing the following points about the naming conventions used in the code examples should be noted.
  
If the attribute is singular or it describes *an attribute of* a business object,  it will start with the term `Has`.  
If the attribute is plural or it describes the *behavior of* a business object,  it will start with the term `With`.

::: tip
There are a lot of [Built-in Attributes](/guide/model-builders-built-in.md) provided by Xenial.  If you are missing one, that should be built into the framework, [let us know](https://github.com/xenial-io/Xenial.Framework/issues/)!
:::
