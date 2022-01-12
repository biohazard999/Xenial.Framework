---
title: 'DetailViewLayoutBuilders - SourceGenerators Syntax'
---

# DetailViewLayoutBuilders - SourceGenerators Syntax

Starting with [`.NET5` Microsoft introduced `SourceGenerators`](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/). This allows us to inspect source code and generate code for a more powerful experience of Xenial LayoutBuilders. This is [one of the many interpretations](/guide/source-generators.md) of SourceGenerators that helps us to write type safe and more maintainable code.

Whilst `Xenial.Framework` does [provide a robust and IntelliSense driven way](layout-builders-advanced-syntax.md) to craft layouts in code, it is still verbose and requires a lot of lambda expressions, which can lead to a lot of nested braces, which might be hard to read and maintain. Combined with the [record syntax](layout-builders-record-syntax.md) it provides the cleanest and concise way of writing layouts without loosing any type safety.

By way of a quick reminder the illustration below shows what the final layout should look like;

![Person Result Layout](/images/guide/layout-builders/person-result-layout-simple.png)

## Installation

In your [platform agnostic module](https://docs.devexpress.com/eXpressAppFramework/118045/concepts/application-solution-components/application-solution-structure#projects) install the [Xenial.Framework.SourceGenerators](https://www.nuget.org/packages/Xenial.Framework.SourceGenerators/) package.

<code-group>
<code-block title=".NET CLI">

<div class="language-bash"><pre class="language-bash"><code>dotnet add package Xenial.Framework --version {{ $var['xenialVersion'] }}
dotnet add package Xenial.Framework.Generators --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

<code-block title="PackageReference">

<div class="language-xml"><pre class="language-xml"><code>&ltPackageReference Include="Xenial.Framework" Version="{{ $var['xenialVersion'] }}" /&gt
&ltPackageReference Include="Xenial.Framework.Generators" Version="{{ $var['xenialVersion'] }}"&gt
      &ltPrivateAssets&gtall&lt/PrivateAssets&gt
      &ltIncludeAssets&gtruntime; build; native; contentfiles; analyzers; buildtransitive&lt/IncludeAssets&gt
&lt/PackageReference&gt</code></pre></div>

</code-block>

<code-block title="Package Manager">

<div class="language-powershell"><pre class="language-powershell"><code>Install-Package Xenial.Framework -Version {{ $var['xenialVersion'] }}
Install-Package Xenial.Framework.Generators -Version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

<code-block title="Paket CLI">

<div class="language-bash"><pre><code>paket add Xenial.Framework --version {{ $var['xenialVersion'] }}
paket add Xenial.Framework.Generators --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

</code-group>

::: tip INFORMATION
By convention the platform agnostic module is usually named `<Your Application>.Module`.
If you're unfamiliar with the Command Line Interface (cli) you can always use the Nuget package manager.

Whilst the Xenial.Framework can of course be used in platform specific modules, for the purposes of this documentation emphasis will be given to its use in the platform agnostic module of your project.
:::

::: tip
If you use [VisualStudio make sure you at least are using 2019 v16.9 or higher](https://docs.microsoft.com/en-us/visualstudio/releases/2019/release-notes-v16.9). According to our tests VisualStudio 2022 or higher is recommended. You also need to install the [.NET5 SDK (or greater)](https://dotnet.microsoft.com/en-us/download) even if you want to use generators with `net462` (Full Framework)
:::

:::warning Warning

Make sure to restart VisualStudio after installing the SourceGenerator to make sure VisualStudio's intellisense recognizes the generator.  
If your build is working fine, but your intellisense is giving you errors, you forgot to restart VisualStudio.

:::

## Registration

As before the first task is to tell XAF to use the `DetailViewLayoutBuilders`.

Override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Deriving from the `LayoutBuilder<TModelClass>` class

After we installed the `Xenial.Framework.Generators` package, we need derive from `LayoutBuilder<TModelClass>` class and declare it as `partial` as illustrated below;

<<< @/guide/samples/layout-builders-sourcegenerators/SourceGeneratorLayout-partial.cs{17}

We of course need to tell Xenial to use the builder;

<<< @/guide/samples/layout-builders-sourcegenerators/SourceGeneratorLayout-partial.cs{14}

Afterwards we can use the static `Editor` class generated to access all the properties of `TModelClass` to define our layout. With the help of the [Record-Syntax](/layout-builders-record-syntax.md) we can define additional properties;

<<< @/guide/samples/layout-builders-sourcegenerators/SourceGeneratorLayout-editors.cs{14-18,20,22,23,26,27,43}

To write editors for nested objects we need to declare the `[Xenial.ExpandMember("XXX")]` where the generated static `Constants` class get handy;

<<< @/guide/samples/layout-builders-sourcegenerators/SourceGeneratorLayout-nested-1.cs{5-6}

Afterwards we use the nested static `Editor._XXXX` classes generated to access all the nested properties to define our layout;

<<< @/guide/samples/layout-builders-sourcegenerators/SourceGeneratorLayout-nested-2.cs{15,17,18,20,21,28,30,31,33,34}

::: tip

You can nest calls of `[Xenial.ExpandMember]` recursively if you have a deep object graph you need to display.

```cs
    [XenialExpandMember(Constants.Address1)]
    [XenialExpandMember(Constants._Address1.Country)]
    [XenialExpandMember(Constants._Address1._Country.Currency)]
```

:::

### Final result

<<< @/guide/samples/layout-builders-sourcegenerators/SourceGeneratorLayout.cs
