using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.Internal;

namespace Demos.Data {
    public static class DemoDbEngineDetectorHelper {
        public static string AlternativeConnectionString = "DataSource=Alternative";
        public static string SQLServerIsNotFoundMessage = "Could not find a SQL database server on your computer.";
        public static string DBServerIsNotAccessibleMessage = "This XAF Demo application failed to access your SQL database server.";
        public static string DBIsNotAccessibleMessage = "This XAF Demo application failed to access a database.";
        public static string PatchSQLConnectionString(string connectionString) {
#if !NET5_0 || WINDOWS
            if(DbEngineDetector.IsSqlExpressInstalled || DbEngineDetector.IsLocalDbInstalled) {
                return DbEngineDetector.PatchConnectionString(connectionString);
            }
            else {
#endif
            return DemoDbEngineDetectorHelper.AlternativeConnectionString;
#if !NET5_0 || WINDOWS
            }
#endif
        }
        private static string GetSQLServerConnectionString(string connectionString, out string databaseName) {
            string result = connectionString;
            databaseName = "";
            List<string> connectionStringParts = new List<string>();
            connectionStringParts.AddRange(connectionString.Split(';'));
            string databaseNamePart = connectionStringParts.FirstOrDefault(x => x.StartsWith("initial catalog", StringComparison.InvariantCultureIgnoreCase));
            if(!string.IsNullOrEmpty(databaseNamePart)) {
                connectionStringParts.Remove(databaseNamePart);
                result = string.Join(";", connectionStringParts);
                databaseName = databaseNamePart.Substring(databaseNamePart.IndexOf('=') + 1);
            }
            return result;
        }
        public static string GetIssueMessage(string connectionString) {
            return connectionString == AlternativeConnectionString ? SQLServerIsNotFoundMessage : DBServerIsNotAccessibleMessage;
        }
        public static bool IsSqlServerAccessible(string connectionString) {
            if(string.IsNullOrEmpty(connectionString)) {
                return false;
            }
            bool result = true;
            string databaseName = "";
            string sqlServerConnectionString = GetSQLServerConnectionString(connectionString, out databaseName);
            SqlConnection sqlConnection = new SqlConnection(sqlServerConnectionString);
            SqlConnection sqlConnection1 = new SqlConnection(sqlServerConnectionString);
            try {
                sqlConnection.Open();
                string accessQueryString = string.Format("SELECT HAS_DBACCESS('{0}')", databaseName);
                SqlCommand accessCommand = new SqlCommand(accessQueryString, sqlConnection);
                object canAccess = accessCommand.ExecuteScalar();
                if(canAccess is DBNull) {
                    string createQueryString = "SELECT has_perms_by_name(null, null, 'CREATE ANY DATABASE');";
                    SqlCommand createCommand = new SqlCommand(createQueryString, sqlConnection);
                    int canCreate = (int)createCommand.ExecuteScalar();
                    if(canCreate == 0) {
                        result = false;
                    }
                }
                else if((int)canAccess == 0) {
                    result = false;
                }
            }
            catch(Exception) {
                result = false;
            }
            finally {
                if(sqlConnection != null) {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if(sqlConnection1 != null) {
                    sqlConnection1.Close();
                    sqlConnection1.Dispose();
                }
            }
            return result;
        }
    }

    public class UseSQLAlternativeInfoSingleton {
        private static UseSQLAlternativeInfoSingleton instance;
        private UseSQLAlternativeInfo useSqlAlternativeInfo;
        private UseSQLAlternativeInfoSingleton() {
            UseAlternative = false;
        }
        public static UseSQLAlternativeInfoSingleton Instance {
            get {
                if(instance == null) {
                    instance = new UseSQLAlternativeInfoSingleton();
                    instance.useSqlAlternativeInfo = new UseSQLAlternativeInfo();
                }
                return instance;
            }
        }
        public bool UseAlternative { get; set; }
        public UseSQLAlternativeInfo Info { get { return useSqlAlternativeInfo; } }
        public void FillFields(string sqlIssue, string alternativeName, string restrictions) {
            if(!this.UseAlternative) {
                this.UseAlternative = true;
                this.Info.SQLIssue = sqlIssue;
                this.Info.Alternative = alternativeName;
                this.Info.Restrictions = restrictions;
            }
            else if(!this.Info.Alternative.Contains(alternativeName)) {
                AddAlternative(alternativeName, restrictions);
            }
        }
        public void AddAlternative(string alternativeName, string restrictions) {
            this.Info.Alternative += " and " + alternativeName;
            this.Info.Restrictions += Environment.NewLine + restrictions;
        }
        public void Clear() {
            UseAlternative = false;
            Info.SQLIssue = null;
            Info.Alternative = null;
            Info.Restrictions = null;
        }
    }
}
