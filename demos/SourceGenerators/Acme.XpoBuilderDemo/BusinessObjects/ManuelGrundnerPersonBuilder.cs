using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Xpo;

namespace Acme.Module.BusinessObjects;

internal class ManuelGrundnerPersonBuilder : PersonBuilder
{
    public ManuelGrundnerPersonBuilder(Session session)
    {
        WithSession(session);
        WithFirstName("Manuel");
        WithLastName("Grundner");
        WithTasks(b => b
            .WithDescription("Implement SourceGenerators")
            .WithDuration(TimeSpan.MaxValue)
        );
        WithTasks(b => b
            .WithDescription("Test SourceGenerators")
            .WithDuration(TimeSpan.MaxValue)
        );
        WithTasks(b => b
            .WithDescription("Deploy SourceGenerators")
            .WithDuration(TimeSpan.MaxValue)
        );
    }
}
