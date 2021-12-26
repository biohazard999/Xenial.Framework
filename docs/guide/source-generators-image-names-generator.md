---
title: SourceGenerators - ImageNamesGenerator
sidebarDepth: 5
---

# ImageNamesGenerator - Introduction

A generator that helps you avoid mistakes when dealing with ImageNames's.

## Intent

When writing XAF code we often need to deal with `ImageNames`. Most of the time those come in different sizes if we use `PNG` files or as `SVG` files. Regardless of the format, in XAF, we often need to refer them as string constants. If we write a more complex module (like for example a custom editor module) we might need those images as `Embedded Resources`, instances of `System.Drawing.Image` or even `raw bytes`. This may result often in a lot of trial and error (because of typos) or even runtime exceptions. This source generator tries to minimize this weakness.

## Usage

```cs
namespace Acme.Module
{
    [Xenial.XenialImageNames]
    internal partial class ImageNames { }
}
```

```xml
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

## Drawbacks/Issues

* No Grouping by Size, yet
* No Nested Folder support, yet
* No support for mixing SVG & PNG, yet
* No support for mapping multiple image sources to multiple classes, yet
* Assembly size will increase by a couple of bytes even if some constants are not used, yet
* Use Images directly as Resources, Bytes and Stream without the need of `ImageLoader` (SVG support is experimental)

## API-surface

A partial class marked with the `Xenial.XenialImageNames` will follow the rules:

* All MSBuild `Includes` that follow either/or
  * `<XenialImageNames Include="Images/*.*" />`
  * `<AdditionalFiles Include="Images/*.*" XenialImageNames="true" />`

* It will respect the visibility of the target class
