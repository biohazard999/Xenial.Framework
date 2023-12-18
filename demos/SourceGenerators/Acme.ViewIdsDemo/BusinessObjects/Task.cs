using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.Xpo;

namespace Acme.Module;

[Persistent]
public class Task : XPObject
{
    public Task(Session session) : base(session) { }

    [Association]
    public Person Person { get; set; }

    public string Description { get; set; }

    public TimeSpan Duration { get; set; }
}
