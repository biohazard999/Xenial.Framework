using System;

using Acme.Module;
using Acme.Module.Helpers;

using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

using Newtonsoft.Json;

using static Acme.Module.Helpers.DemoHelper;

var session = CreateSession();

var person = new PersonBuilder()
    .WithSession(session)
    .WithFirstName("John")
    .WithLastName("Doe")

    .WithManager(new PersonBuilder()
            .WithFirstName("Mary")
            .WithLastName("Major")

    ).WithTasks(new TaskBuilder()
        .WithDescription("Install Xenial.Framework")
        .WithDuration(TimeSpan.FromSeconds(30))

    ).WithTasks(new TaskBuilder()
        .WithDescription("Use Xenial.Framework.Generators")
        .WithDuration(TimeSpan.FromSeconds(10))

    ).Build();

session.Save(person);

WriteXPObjectToConsole(person);

Console.ReadLine();
