---
title: 'DetailViewLayoutBuilders - Record Syntax'
---

# DetailViewLayoutBuilders - `Record` Syntax

C#9 [introduced a new record syntax](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records#:~:text=C%23%209%20introduces%20records%2C%20a,types%20use%20value%2Dbased%20equality.) which has been implemented within Xenial.Framework LayoutBuilders ensuring that layouts can be built using `with` expressions. Although not very different from initializers they make it possible to create a copy of a given record, which is particularly beneficial to a clean fluent syntax in combination with a **functional style** API.

## Setting the compiler options

Using this feature requires the compiler version to be set in the project. By default the framework will choose the compiler level based on the .NET version being used but it can be overridden  by setting the [`LangVersion`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version) property in the`*.csproj` files.

By far the best way to use this feature is to create a `Directory.Build.props` file in the same location as the application `*.sln` file:

```xml
<Project>
  <PropertyGroup>
    <LangVersion>9.0</LangVersion>
    <!--<LangVersion>latest</LangVersion> Alternative: just use the latest version, if you want the latest and greatest -->
  </PropertyGroup>
</Project>
```

For more information on this topic [please look at the Microsoft Documentation](https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/configure-language-version#configure-multiple-projects)

::: tip
To ensure that the compiler is picked up correctly close VisualStudio, delete all `bin` and `obj` folders and then restart `VisualStudio`.
:::

::: warning CAUTION
Whilst it is possible to use this in projects targeting .net frameworks below `net5` (by adding a class called `IsExternalInit` in the project) it is not officially supported by Microsoft:

```cs
#if !NET5

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}
#endif
```

:::

::: danger
All code should be thoroughly tested after changing the compiler version.  

Microsoft has done a great job trying not to break any existing client project, but because it is not supported on the old full framework (.NET4xx) officially, use this technique at your own risk. 
:::

## Registration

Registration is exactly the same as in the previous examples, override the `AddGeneratorUpdaters` in the platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Build the Layout

As this uses C#9 it is now possible to use the [`Target-typed new expressions feature`](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#fit-and-finish-features) which removes a little bit of redundancy in the code as shown below:

<<< @/guide/samples/layout-builders-records/RecordLayout.cs

Although the syntax is a lot longer than the fluent builder syntax, it is a little more structured. It's combining both the power of `expression trees` to specify type safe layouts, as well as a familiar syntax comparable to initializers. However it is more verbose (language limitations require the need to specify the `Children` directly, which is not needed with normal initializer syntax) so it's use is only recommended when there is a need to specify properties and children at the same time, or when using leaf node types (for example property editors).

## A mixed sample

<<< @/guide/samples/layout-builders-records/RecordLayoutMixed.cs

::: warning
Whilst this sample works without issue mixing syntax styles is not recommended. It may work from a technical standpoint but it adds complexity and harms readability. 
 
Wherever possible coding styles and conventions should be clearly defined and adhered to. 
:::
