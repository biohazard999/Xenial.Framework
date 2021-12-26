using System;
using System.Data;
using System.Data.SqlClient;
using DevExpress.ExpressApp.Xpo;

namespace Demos.Data {
    public static class DemoXPODatabaseHelper {
        public static string InMemoryDatabaseUsageMessage = "This may cause performance issues. All data modifications will be lost when you close the application.";
        public static string AlternativeName = "XPO InMemoryDataStore";
        public static IXpoDataStoreProvider GetDataStoreProvider(string connectionString, IDbConnection connection, bool enablePoolingInConnectionString = true) {
            if(UseSQLAlternativeInfoSingleton.Instance.UseAlternative) {
                connectionString = InMemoryDataStoreProvider.ConnectionString;
            }
            if(connection != null) {
                connectionString = null;
            }
            else {
                if((connectionString != InMemoryDataStoreProvider.ConnectionString) && !string.IsNullOrEmpty(connectionString) && !DemoDbEngineDetectorHelper.IsSqlServerAccessible(connectionString)) {
                    string patchedConnectionString = DemoDbEngineDetectorHelper.PatchSQLConnectionString(connectionString);
                    if((patchedConnectionString == DemoDbEngineDetectorHelper.AlternativeConnectionString) || !DemoDbEngineDetectorHelper.IsSqlServerAccessible(patchedConnectionString)) {
                        UseSQLAlternativeInfoSingleton.Instance.FillFields(DemoDbEngineDetectorHelper.GetIssueMessage(patchedConnectionString), DemoXPODatabaseHelper.AlternativeName, DemoXPODatabaseHelper.InMemoryDatabaseUsageMessage);
                        patchedConnectionString = InMemoryDataStoreProvider.ConnectionString;
                    }
                    connectionString = patchedConnectionString;
                }
            }
            return XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, enablePoolingInConnectionString);
        }
    }
}
