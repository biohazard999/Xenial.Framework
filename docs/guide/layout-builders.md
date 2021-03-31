---
title: DetailViewLayoutBuilders - Introduction
---

# DetailViewLayoutBuilders - Introduction

DetailView LayoutBuilders are a way of defining `DetailViews` in code. They are a domain specific language (DSL) to build your Views in code rather than using the `ModelEditor`. As always in `Xenial.Framework` there are several ways to define them and how to tell the framework when to consume them. They are an addition to the `ModelEditor based approach`, because it operates a couple of levels below the differences layer.

DetailView LayoutBuilders are one of the many `NonVisual Components` of Xenial.Framework and designed around best practices and working most efficiently in a team, however there are several benefits for smaller teams and projects as well.

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
