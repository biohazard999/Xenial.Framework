---
title: SourceGenerators - ViewIdsGenerator
sidebarDepth: 5
demoName: Acme.ViewIdsDemo
---

# ViewIdsGenerator - Introduction

A generator that helps you avoid mistakes when dealing with ViewId's.

## Intent

When writing XAF code we often need to deal with View-Ids. Most of the time those are `DefaultDetailView` `DefaultListView` and `DefaultLookupListView` of a particular business class. With the help of `ModelNodeIdHelper` we can eliminate most string magic in regular code (like when defining an `Controller` with a `TargetViewId`). But a lot of code in XAF needs to be defined in `Attributes` hence we can not use `ModelNodeIdHelper` in that case. Another case that can't be solved are [custom views](TODO: Define Custom Views). To avoid this you need to define constants that need a lot of maintenance. This source generator tries to minimize this weakness.

## Usage

```cs{3}
namespace Acme.Module
{
    [Xenial.XenialViewIds]
    internal partial class ViewIds { }
}
```

## Generated-Code

Given you have business objects like:

```cs{4,5,13,17}

namespace Acme.Module.BusinessObjects
{
    [Persistent] //We need to flag the class with either [Persistent] [NonPersistent] or [DomainComponent] to be collected by this source generator
    [Xenial.Framework.Base.DeclareDetailView("Person_Custom_DetailView")] //Only place this magic string ever appears, afterwards we are able to use the generated code.
    public class Person : XPObject
    {
        public Person(Session session) : base(session) { }

        [Association("Person-Addresses")]
        [Aggregated]
        // public ICollection members are considered as nested views, regardless of association or aggregation status
        public XPCollection<Address> Addresses
            => GetCollection<Address>(nameof(Addresses));
    }

    [Persistent]
    public class Address : XPObject
    {
        public Address(Session session) : base(session) { }

        [Persistent]
        [Association("Person-Addresses")]
        public Person Person
        {
            get => person;
            set => SetPropertyValue(nameof(Person), ref person, value);
        }
    }
}

```

This will output:

```cs
namespace Acme.Module
{
    [CompilerGenerated]
    partial class ViewIds
    {
        internal const string Person_DetailView = "Person_DetailView";
        internal const string Person_ListView = "Person_ListView";
        internal const string Person_LookupListView = "Person_LookupListView";
        internal const string Person_Addresses_ListView = "Person_Addresses_ListView";
        internal const string Person_Custom_DetailView = "Person_Custom_DetailView";
        internal const string Address_DetailView = "Address_DetailView";
        internal const string Address_ListView = "Address_ListView";
        internal const string Address_LookupListView = "Address_LookupListView";
    }
}
```

## Advantage

* Usage of *strongly* typed id's
* Centralized usage via common class name `ViewIds` inside the assembly
* Usage in attributes (`Validation`, `ConditionalAppearance`,...) and controllers (`TargetViewId`) etc.
* Compile time refactoring safe for renames

## Drawbacks/Issues

* We need attributes on the target classes to work, yet
* There is no way yet to use ModelBuilders to define custom views, yet
* Assembly size will increase by a couple of bytes even if some constants are not used, yet
* No Grouping by Type, yet
* No customization of default ViewId generation

## API-surface

A partial class marked with the `Xenial.XenialViewIdsAttribute` will follow the rules:

* All classes inside the assembly marked with the following attributes will generate the default views (`XXX_DetailView`, `XXX_ListView`, `XXX_LookupListView`):

  * `DevExpress.ExpressApp.DC.DomainComponentAttribute`
  * `DevExpress.Xpo.PersistentAttribute`
  * `DevExpress.Xpo.NonPersistentAttribute`

* All `public ICollection` interface assignable members will generate a nested collection that follows:
  * `XXX_NameOfTheCollectionProperty_ListView`

* All classes inside the assembly marked with the following attributes will be added as defined if they are marked with the previous attributes:

  * `Xenial.Framework.Base.DeclareDetailViewAttribute`
  * `Xenial.Framework.Base.DeclareListViewAttribute`
  * `Xenial.Framework.Base.DeclareDashboardViewAttribute`

* All generated constants will be removed if they fit the criteria of:

  * `Xenial.Framework.Base.GenerateNoDetailViewAttribute`
  * `Xenial.Framework.Base.GenerateNoListViewAttribute`
  * `Xenial.Framework.Base.GenerateNoLookupListViewAttribute`
  * `Xenial.Framework.Base.GenerateNoNestedListViewAttribute`

* It will respect the visibility of the target class

## Options

### MSBuild

* `<GenerateXenialViewIdsAttribute>` - Control if `XenialViewIdsAttribute` will be emitted
* `<XenialAttributesVisibility>` (global) - Control's `XenialViewIdsAttribute` visibility modifier (`public`/`internal`)
* `<EmitCompilerGeneratedFiles>` (global) - Code will be flushed to disk (debug)
* `<XenialDebugSourceGenerators>` (global) - Debugger will launch on code generation (debug)

### Code

Generator will follow the visibility of the Target Class:

```cs{6-7,15-16}
/* SourceGenerator will follow the visibility of the class */

namespace Acme.Module
{
    [Xenial.XenialViewIds]
    // Declared as public so...
    public partial class ViewIds { }
}

namespace Acme.Module
{
    [CompilerGenerated]
    partial class ViewIds
    {
        // ...the output will be public constants
        public const string Person_DetailView = "Person_DetailView";
    }
}

```

## Tips and Tricks

Use the [C# static using](https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/using-directive#static-modifier) feature to remove the `ViewIds` prefix:

```cs
using static Acme.Module.ViewIds;

//Know you have all constants in instant scope like:
string personViewId = Person_DetailView;
```

You directly can use custom views when declared even on the same class (so the magic string only appears once in the source base):

```cs{3-4}
using Xenial.Framework.Base;

[DeclareDetailView("CustomPersonView")]
[CustomAttribute(ViewIds.CustomPersonView)]
public class Person { /* ... */ }

```

You can use [constant interpolated strings](https://docs.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-10.0/constant_interpolated_strings) to eliminate string magic in Attributes:

```cs
[MyAttribute($"{ViewIds.Person_ListView};{ViewIds.Person_LookupListView}")]
```

Mark the the target class public and name according to your module so external modules can use them:

```cs{6}
using Xenial.Framework.Base;
namespace Acme.Accounting.Module
{
    [Xenial.XenialViewIds]
    /* this will make sure you can access the view id's across assembly boundaries and avoid conflicting names later */
    public partial class AccountingViewIds { }
}
```

Because those are partial, you can add custom code and mark them static

```cs{6,8}
using Xenial.Framework.Base;
namespace Acme.Accounting.Module
{
    [Xenial.XenialViewIds]
    public static partial class AccountingViewIds
    {
        public const string CustomConstant = "Whatever constant you need";
    }
}
```

::: warning CAUTION
When updating from an older Xenial to a newer Xenial version, it's necessary to restart VisualStudio/VSCode after the upgrade, so Intellisense can reload the new SourceGenerator. So it may come to false positive warnings if they don't match.
:::

## Diagnostics

|ID            | Severity | Message                                                                     | Reason                                                                                 |
|:------------:|:--------:|-----------------------------------------------------------------------------|----------------------------------------------------------------------------------------|
|XENGEN0010    | Error    | Could not parse boolean MSBUILD variable `<GenerateXenialViewIdsAttribute>` | MsBuild variable needs to be in boolean parsable format: `true`/`false`/`True`/`False` |
|XENGEN0100    | Error    | The class using the `[XenialViewIdsAttribute]` needs to be partial          | We can not generate code for non partial classes                                       |
|XENGEN0101    | Error    | The class using the `[XenialViewIdsAttribute]` needs to be in a namespace   | We can not generate code in the global namespace                                       |

## Demo-Source

<!-- markdownlint-disable MD033 -->
You can find demo sources in the <a target="_blank" :href=" $var['gitHubUrl'] + '/tree/' + $var['gitBranch'] + '/demos/SourceGenerators/' + $frontmatter.demoName">Xenial.Framework repository</a> for in depth usage information.
<!-- markdownlint-enable MD033 -->
