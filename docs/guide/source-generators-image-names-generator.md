---
title: SourceGenerators - ImageNamesGenerator
sidebarDepth: 5
demoName: Acme.ImagesDemo
---

# ImageNamesGenerator - Introduction

A generator that helps you avoid mistakes when dealing with ImageNames's.

## Intent

When writing XAF code we often need to deal with `ImageNames`. Most of the time those come in different sizes if we use `PNG` files or as `SVG` files. Regardless of the format, in XAF, we often need to refer them as string constants. If we write a more complex module (like for example a custom editor module) we might need those images as `Embedded Resources`, instances of `System.Drawing.Image` or even `raw bytes`. This may result often in a lot of trial and error (because of typos) or even runtime exceptions. This source generator tries to minimize this weakness.

## Usage

```cs{3}
namespace Acme.Module
{
    [Xenial.XenialImageNames]
    internal partial class ImageNames { }
}
```

```xml{3,4}
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <XenialImageNames Include="Images/*.png" />
    <XenialImageNames Include="Images/*.svg" />
  </ItemGroup>
</Project>
```

## Generated-Code

Given you have a project structure like:

```txt
Acme.Module
Acme.Module/Images
Acme.Module/Images/MyImage.png
```

This will output:

```cs
namespace Acme.Module
{
    [CompilerGenerated]
    partial class ImageNames
    {
        internal const string MyImage = "MyImage";
    }
}
```

## Advantage

* Usage of *strongly* typed image names
* Centralized usage via common class name `ImageNames` inside the assembly
* Usage in attributes `BusinessObjects`, `[ActionAttribute]` and `Controllers` (`Action.ImageName`) etc.
* Intellisense for `ImageNames`
* Preview of actual image when using the `CodeRush Rich Comments` feature
* Use Images directly as Resources, Bytes and Stream without the need of `ImageLoader` (SVG support is experimental)

## Drawbacks/Issues

* No Grouping by Size, yet
* No Nested Folder support, yet
* No support for mixing SVG & PNG, yet
* No support for mapping multiple image sources to multiple classes, yet
* No support for different subdirectories (only `Images` will be treated), yet
* Assembly size will increase by a couple of bytes even if some constants are not used, yet

## API-surface

A partial class marked with the `Xenial.XenialImageNames` will follow the rules:

* All MSBuild `Includes` that define the `XenialImageNames` inside the `Images` subdirectory will be treated as image
  * `<ItemGroup><XenialImageNames Include="Images/*.*" /></ItemGroup>`

* It will respect the visibility of the target class

## Options

### MSBuild

* `<GenerateXenialImageNamesAttribute>` - Control if `XenialImageNamesAttribute` will be emitted
* `<XenialAttributesVisibility>` (global) - Control's `XenialImageNamesAttribute` visibility modifier (`public`/`internal`)
* `<EmitCompilerGeneratedFiles>` (global) - Code will be flushed to disk (debug)
* `<XenialDebugSourceGenerators>` (global) - Debugger will launch on code generation (debug)

### Code

Generator will follow the visibility of the Target Class:

```cs{6-7,15-16}
/* SourceGenerator will follow the visibility of the class */

namespace Acme.Module
{
    [Xenial.XenialImageNames]
    // Declared as public so...
    public partial class ImageNames { }
}

namespace Acme.Module
{
    [CompilerGenerated]
    partial class ImageNames
    {
        // ...the output will be public constants
        public const string MyImage = "MyImage";
    }
}

```

| Property            | Type      | Behavior                                                                |
|---------------------|-----------|-------------------------------------------------------------------------|
| `SmartComments`     | `Boolean` | Will generate a comment that allows CodeRush to display an inline image |
| `ResourceAccessors` | `Boolean` | Will emit additional subclasses to access Strongly typed resources      |
| `Sizes`             | `Boolean` | (experimental) Will group by [Size Suffixes](https://docs.devexpress.com/eXpressAppFramework/112792/application-shell-and-base-infrasctructure/icons/add-and-replace-icons#rules-for-image-files) |

## Tips and Tricks

Use the [C# static using](https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/using-directive#static-modifier) feature to remove the `ImageNames` prefix:

```cs
using static Acme.Module.ImageNames;

//Know you have all constants in instant scope like:
string myImage = MyImage;
```

Mark the the target class public and name according to your module so external modules can use them:

```cs{6}
using Xenial.Framework.Base;
namespace Acme.Accounting.Module
{
    [Xenial.XenialImageNames]
    /* this will make sure you can access the image names across assembly boundaries and avoid conflicting names later */
    public partial class AccountingImageNames { }
}
```

Because those are partial, you can add custom code and mark them static

```cs{6,8}
using Xenial.Framework.Base;
namespace Acme.Accounting.Module
{
    [Xenial.XenialImageNames]
    public static partial class AccountingImageNames
    {
        public const string CustomConstant = "Whatever constant you need";
    }
}
```

When using the SmartComments and ResourceAccessors you can safe a lot of boiler plate code. Given you have one image in 4 sizes ImageNamesGenerator will produce the following code:

![XenialImageNamesGenerator with SmartComments and ResourceAccessors](/images/guide/source-generators/imagenames-generator.png)

::: danger CAUTION
When using image names, make sure they are valid C# identifiers (don't start with numbers, special characters, etc.), otherwise the source generation will break
:::


::: warning CAUTION
When updating from an older Xenial to a newer Xenial version, it's necessary to restart VisualStudio/VSCode after the upgrade, so Intellisense can reload the new SourceGenerator. So it may come to false positive warnings if they don't match.
:::

## Diagnostics

|ID            | Severity | Message                                                                        | Reason                                                                                 |
|:------------:|:--------:|--------------------------------------------------------------------------------|----------------------------------------------------------------------------------------|
|XENGEN0010    | Error    | Could not parse boolean MSBUILD variable `<GenerateXenialImageNamesAttribute>` | MsBuild variable needs to be in boolean parsable format: `true`/`false`/`True`/`False` |
|XENGEN0010    | Error    | Could not parse boolean MSBUILD variable `<XenialImageNames>`                  | MsBuild variable needs to be in boolean parsable format: `true`/`false`/`True`/`False` |
|XENGEN0100    | Error    | The class using the `[XenialImageNamesAttribute]` needs to be partial          | We can not generate code for non partial classes                                       |
|XENGEN0101    | Error    | The class using the `[XenialImageNamesAttribute]` needs to be in a namespace   | We can not generate code in the global namespace                                       |

## Demo-Source

<!-- markdownlint-disable MD033 -->
You can find demo sources in the <a target="_blank" :href=" $var['gitHubUrl'] + '/tree/' + $var['gitBranch'] + '/demos/SourceGenerators/' + $frontmatter.demoName">Xenial.Framework repository</a> for in depth usage information.
<!-- markdownlint-enable MD033 -->
