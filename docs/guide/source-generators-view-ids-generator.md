---
title: SourceGenerators - ViewIdsGenerator
sidebarDepth: 5
---

# ViewIdsGenerator - Introduction

A generator that helps you avoid mistakes when dealing with ViewId's.

## Intent

When writing XAF code we often need to deal with View-Ids. Most of the time those are `DefaultDetailView` `DefaultListView` and `DefaultLookupListView` of a particular business class. With the help of `ModelNodeIdHelper` we can eliminate most string magic in regular code (like when defining an `Controller` with a `TargetViewId`). But a lot of code in XAF needs to be defined in `Attributes` hence we can not use `ModelNodeIdHelper` in that case. Another case that can't be solved are [custom views](TODO: Define Custom Views). To avoid this you need to define constants that need a lot of maintenance. This source generator tries to minimize this weakness.

## Usage

```cs
namespace Acme.Module
{
    [Xenial.XenialViewIds]
    internal partial class ViewIds { }
}
```

## Generated-Code

Given you have business objects like:

```cs

namespace Acme.Module.BusinessObjects
{
    [Persistent]    
    [Xenial.Framework.Base.DeclareDetailView("Person_Custom_DetailView")]
    public class Person : XPObject
    {
        public Person(Session session) : base(session) { }

        [Association("Person-Addresses")]
        [Aggregated]
        public XPCollection<Address> Addresses
        {
            get
            {
                return GetCollection<Address>(nameof(Addresses));
            }
        }
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

## API-surface

## Diagnostics

## Options
