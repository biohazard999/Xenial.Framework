using System;

using DevExpress.Xpo;

namespace Acme.Module;

[Persistent]
public class Person : XPObject
{
    public Person(Session session) : base(session) { }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Person Manager { get; set; }

    [Association]
    public XPCollection<Task> Tasks => GetCollection<Task>(nameof(Tasks));
}

