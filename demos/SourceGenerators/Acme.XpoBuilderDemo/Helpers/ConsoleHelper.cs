using System;

using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

using Newtonsoft.Json;

namespace Acme.Module.Helpers;

public static class DemoHelper
{
    internal static Session CreateSession()
    {

        XpoDefault.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
        XpoDefault.Dictionary = new ReflectionDictionary();
        XpoDefault.Dictionary.CollectClassInfos(typeof(Person).Assembly);
        return XpoDefault.Session;
    }

    internal static void WriteXPObjectToConsole(XPObject xpObject)
        => Console.WriteLine(JsonConvert.SerializeObject(xpObject, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DemoModelJsonSerializationContractResolver()
        }));
}
