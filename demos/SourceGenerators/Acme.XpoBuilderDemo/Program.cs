using System;

using Acme.Module.BusinessObjects;

using DevExpress.Xpo;

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
/*
{
  "FirstName": "John",
  "LastName": "Doe",
  "Manager": {
    "FirstName": "Mary",
    "LastName": "Major",
    "Manager": null,
    "Tasks": [],
    "Oid": 1
  },
  "Tasks": [
    {
      "Description": "Use Xenial.Framework.Generators",
      "Duration": "00:00:10",
      "Oid": 2
    },
    {
      "Description": "Install Xenial.Framework",
      "Duration": "00:00:30",
      "Oid": 1
    }
  ],
  "Oid": 2
}
*/

var manuel = new ManuelGrundnerPersonBuilder(session)
    .WithManager(person)
    .Build();

session.Save(manuel);

WriteXPObjectToConsole(manuel);
/*
{
  "FirstName": "Manuel",
  "LastName": "Grundner",
  "Manager": {
    "FirstName": "John",
    "LastName": "Doe",
    "Manager": {
      "FirstName": "Mary",
      "LastName": "Major",
      "Manager": null,
      "Tasks": [],
      "Oid": 1
    },
    "Tasks": [
      {
        "Description": "Use Xenial.Framework.Generators",
        "Duration": "00:00:10",
        "Oid": 2
      },
      {
        "Description": "Install Xenial.Framework",
        "Duration": "00:00:30",
        "Oid": 1
      }
    ],
    "Oid": 2
  },
  "Tasks": [
    {
      "Description": "Implement SourceGenerators",
      "Duration": "10675199.02:48:05.4775807",
      "Oid": 3
    },
    {
      "Description": "Test SourceGenerators",
      "Duration": "10675199.02:48:05.4775807",
      "Oid": 4
    },
    {
      "Description": "Deploy SourceGenerators",
      "Duration": "10675199.02:48:05.4775807",
      "Oid": 5
    }
  ],
  "Oid": 3
}
*/

Console.ReadLine();

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
