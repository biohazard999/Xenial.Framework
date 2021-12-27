using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.Xpo;

using Xenial;

namespace Acme.Module;

[Persistent]
[XenialXpoBuilder]
public class Task : XPObject
{
    /* Build to look at Task.Task.Builder.g.cs */
    public Task(Session session) : base(session) { }

    [Association]
    public Person Person { get; set; }

    public string Description { get; set; }

    public TimeSpan Duration { get; set; }
}
