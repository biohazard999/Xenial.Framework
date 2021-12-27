using System;

using DevExpress.Xpo;

using Xenial;

namespace Acme.Module;

[Persistent]
[XenialXpoBuilder]
public class Person : XPObject
{
    /* Build to look at Person.Person.Builder.g.cs */
    public Person(Session session) : base(session) { }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Person Manager { get; set; }

    [Association]
    public XPCollection<Task> Tasks => GetCollection<Task>(nameof(Tasks));
}

