---
title: 'DetailViewLayoutBuilders - Record Syntax'
---

# DetailViewLayoutBuilders - `Record` Syntax

C#9 [introduced a new syntax for record's](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records#:~:text=C%23%209%20introduces%20records%2C%20a,types%20use%20value%2Dbased%20equality.). Because LayoutBuilders where implemented using this new feature, we are able to build layouts using the `with` expressions. They are not very different from initalizers, but they allow us to create a copy of a given record, which is really helpful to create a clean fluent syntax especially in combination with a **functional style** API.

## Setting the compiler options

First we need to set the compiler version in our project. You have different options here, by default the framework will choose the compiler level based on your .NET version. You can override this behavior by setting the [`LangVersion`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version) property in your `*.csproj` files.

But the best way to use this feature is by creating a `Directory.Build.props` file at the same location as your `*.sln` file:

```xml
<Project>
  <PropertyGroup>
    <LangVersion>9.0</LangVersion>
    <!--<LangVersion>latest</LangVersion> Alternative: just use the latest version, if you want the latest and greatest -->
  </PropertyGroup>
</Project>
```

For more information on this topic [please look at the Microsoft Documentation](https://docs.microsoft.com/de-de/dotnet/csharp/language-reference/configure-language-version#configure-multiple-projects)

::: warning
You can also use records in projects < `net5` (.NET4xxx, .NETStandard2.0, etc...). Although it is not officially supported by Microsoft, you can enable this feature by adding a class called `IsExternalInit` in your project:

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
You need to make sure to test your code after changing the compiler version.  
Microsoft did a great job trying not to break any existing client project, but because it is not supported on the old full framework (.NET4xx) officially, use this technique at your own risk. 
:::

## Registration

The registration in the module is exactly the same as in the last chapter, we need to tell XAF to use the `LayoutBuilders`.  
For this we need to override the `AddGeneratorUpdaters` in our platform agnostic module and call the `updaters.UseDetailViewLayoutBuilders()` extension method.  
Nothing additional is needed to use the record syntax.

<<< @/guide/samples/layout-builders-simple/RegisterInModule.cs{8,12}

## Build the Layout

Because we use C#9 we now also can use the [`Target-typed new expressions feature`](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#fit-and-finish-features) to remove a little bit of redundancy in our code, let's look at this:

<<< @/guide/samples/layout-builders-records/RecordLayout.cs

Although the syntax is a lot longer than the fluent builder syntax, it is a little bit more structured. It's combining both the power of `expression trees` to specify type safe layouts, as well as a familiar syntax comparable to initializers. On the other had it is a little bit more verbose (cause of language limitations, we need to specify the `Children` directly, which is not needed with normal initializer syntax), so we recommend only using it when you need to specify properties and children at the same, or use it on leaf node types only (like for example property editors).

## A mixed sample

<<< @/guide/samples/layout-builders-records/RecordLayoutMixed.cs

::: warning
This sample works just fine, we do not recommend to mix syntax styles too much. Although technically working, it adds complexity and harms readability.  
Make sure you and your team agree on certain styles and conventions and try to stick to them as much as possible.
:::