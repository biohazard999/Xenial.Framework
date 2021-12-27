---
title: SourceGenerators - XpoBuilderGenerator
sidebarDepth: 5
---

# XpoBuilderGenerator - Introduction

A generator that helps you using the builder pattern with XPO Objects when dealing with complicated object models and UnitTests.

## Intent

When writing XPO/XAF code we need to deal object creation all the time. Most of the time this is an easy task by using `ObjectSpace.CreateObject<T>()` or just creating the class manually by using the constructor `new MyBusinessObject(session)`. When the business model grows and the internals of the class structure we can use the [BuilderPattern](//blog.xenial.io/2019/05/26/t-is-for-testing-xaf-xpo-test-data-2.html) to decouple the creation of (complex) objects from the actual domain logic and reduce possible bugs when requirements change over time. This may often result in a works here, but not here scenario. With the flexibility XAF provides, this can lead to hard to grasp bugs. Writing the builder code can be tedious and error prone. This source generator tries to minimize this weakness.

## Usage

```cs
using DevExpress.Xpo;

namespace Acme.Module
{
    [Xenial.XenialXpoBuilder]
    public class Person : XPObject
    {
        public Person(Session session) : base(session) { }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Person Manager { get; set; }

        [Association]
        public XPCollection<Task> Tasks => GetCollection<Task>(nameof(Tasks));
    }

    [Xenial.XenialXpoBuilder]
    public class Task : XPObject
    {
        
    }
}
```
