---
title: SourceGenerators
sidebarDepth: 5
---

# SourceGenerators - Introduction

Xenial. Framework's SourceGenerators helps you to create more robust compile time safe code and remove a lot of boilerplate code that you otherwise may write by hand.

Most of them reflect on your business model code and can increase performance and reliability of otherwise reflection based lookups.

SourceGenerators are one of the many `NonVisual Components` of Xenial.Framework designed to promote best practice and efficient working in large teams, which in turn are equally applicable to smaller teams and projects and even individuals working on a single application.

## Installation

In your [platform agnostic module](https://docs.devexpress.com/eXpressAppFramework/118045/concepts/application-solution-components/application-solution-structure#projects) install the [Xenial.Framework](https://www.nuget.org/packages/Xenial.Framework/) package.

<code-group>
<code-block title=".NET CLI">

<div class="language-bash"><pre class="language-bash"><code>dotnet add package Xenial.Framework.Generators --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>


<code-block title="PackageReference">

<div class="language-xml"><pre class="language-xml"><code>&ltPackageReference Include="Xenial.Framework.Generators" Version="{{ $var['xenialVersion'] }}" /&gt</code></pre></div>

</code-block>

<code-block title="Package Manager">

<div class="language-powershell"><pre class="language-powershell"><code>Install-Package Xenial.Framework.Generators -Version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

<code-block title="Paket CLI">

<div class="language-bash"><pre><code>paket add Xenial.Framework.Generators --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

</code-group>

::: tip INFORMATION
By convention the platform agnostic module is usually named `<Your Application>.Module`.
If you're unfamiliar with the Command Line Interface (cli) you can always use the Nuget package manager.

Whilst the Xenial.Framework can of course be used in platform specific modules, for the purposes of this documentation emphasis will be given to its use in the platform agnostic module of your project.
:::

## Overview

There are a bunch of generators designed to tackle particular *problems* when developing a larger XAF source base. Because they are compile time features they may not reflect 100% runtime behavior, but are designed to be as close as possible.  Most of the generators focus on reliability and make it possible to create compile time safe, intellisense driven, faster XAF development cycle. Because we are inspecting source code, each generator has pro's and con's but most con's lay on the `reflective` nature of `XAF`.

1. [ViewIdsGenerator](/guide/source-generators-view-ids-generator.md) - Emit view id constants based on your XAF model
1. [ImageNamesGenerator]() - Emit image name constants and access resources statically
1. [XpoBuilderGenerator]() - Emit a builder pattern for creating XPO and non persistent objects
1. [ActionGenerator (CTP)]() - Emit controllers and actions in a command pattern style
1. [LayoutBuilderGenerator]() - Emit strongly typed code to build layouts without reflection cost

All of the generators are designed to work together in harmony, however because of their technical nature they depend on execution order like here listed order to work correctly.

::: tip INFORMATION
This list may not be 100% technical accurate and complete, but will focus on the usage side.

To see a full list you should check out the [Appendix section](/guide/appendix-source-generators.md)
:::

## SourceGenerators are patterns that evolve with your code

Because each generator tries to solve a runtime the documentation will try to follow a [pattern like described by Martin Fowler](https://en.wikipedia.org/wiki/Software_design_pattern#Documentation) which will be structured in the following sections:

* Intent - Why this generator exists
* Usage - What you need to implement
* Generated-Code - What is the outcome based on a sample
* Advantage - Why this is helpful
* Drawbacks/Issues - What limitations the generator has
* API-surface - What API's/Attributes are involved when using this generator
* Diagnostics - What diagnostics the generator emits when a wrong usage is detected
* Options - Often source generators have additional options that change the shape of the generated code to help with very large code bases

::: tip
There are a lot of [Features](/guide/appendix-enhance-xenial-source-generators.md) that are enhanced when using SourceGenerators we highly recommend to use them as a best practice.  However all features are designed to be usable without them. If you encounter a problem when using other features without the use of SourceGenerators [let us know](https://github.com/xenial-io/Xenial.Framework/issues/)!
:::
