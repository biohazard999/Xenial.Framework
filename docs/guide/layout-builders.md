---
title: DetailViewLayoutBuilders - Introduction
---

# DetailViewLayoutBuilders - Introduction

DetailView LayoutBuilders are a way of defining `DetailViews` in code. They are a domain specific language (DSL) to build  Views in code rather than using the `ModelEditor`. As always in `Xenial.Framework` there are several ways to define them and how to tell the framework when to consume them. They are an **addition** to the `ModelEditor based approach`, because they operate below the differences layer. <!-- differences layer could probably benefit from some additional description or a link to the XAF documentation -->

<!-- replace this line with its equivalent from the model builders into, remembering to substitute layout builders for model builders -->
DetailView LayoutBuilders are one of the many `NonVisual Components` of Xenial.Framework and designed around best practices and working most efficiently in a team, however there are several benefits for smaller teams and projects as well.

<!-- replace the installation section below with the one from patch 1)-->

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

## Usage

Xenial>Framework provides both a simple and advanced registration pattern for LayoutBuilders as well as one specially optimized for use within ModelBuilders.

All of the LayoutBuilder examples in this documention will be based upon the simple 'Person' business object in the code below.

<<< @/guide/samples/layout-builders-simple/Person.cs

The standard  **default** layout that would be produced by XAF is illustrated below;

![Person Default Layout](/images/guide/layout-builders/person-default-layout.png)

The **target** layout that the code examples of LayoutBuilders in ensuing sections will aim to create is illustrated below;

![Person Target Layout](/images/guide/layout-builders/person-target-layout.png)
