using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Hosting;
using Dapper;

namespace Breast_For_Baby.Data
{
    public class Store
    {
        private static readonly string DbName = HostingEnvironment.MapPath("~/App_Data/b4b.sqlite");
        private static readonly string SetupScriptsDirectory = HostingEnvironment.MapPath("~/Data/Scripts");

        private static readonly string ConnectionString = "Data Source=" + DbName + ";Version=3;";

        static Store()
        {
            InitialiseDb();
        }

        private static void InitialiseDb()
        {
            if (!File.Exists(DbName) || GetExistingSchemaHash() != CalulateScriptsHash())
                CreateDb();
        }

        private static void CreateDb()
        {
            SQLiteConnection.CreateFile(DbName);

            using (var connection = CreateOpenConnection())
            {
                var up = BuildUpScripts();
                connection.Execute(up);
            }

            SaveSchemaHash(CalulateScriptsHash());

            SeedDb();
        }

        public static bool Alive()
        {
            using (CreateOpenConnection())
            {
                return true;
            }
        }

        internal static DbConnection CreateOpenConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        private static string CalulateScriptsHash()
        {
            var scriptContents = BuildUpScripts();

            using (var provider = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(scriptContents)))
                    .Replace("-", string.Empty);
            }
        }

        private static string BuildUpScripts()
        {
            return string.Join("\n", Directory.GetFiles(SetupScriptsDirectory, "*.sql").Select(File.ReadAllText));
        }

        private static string GetExistingSchemaHash()
        {
            try
            {
                using (var connection = CreateOpenConnection())
                {
                    return connection.ExecuteScalar<string>("select Hash from SchemaVersion limit 1") ?? string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void SaveSchemaHash(string hash)
        {
            using (var connection = CreateOpenConnection())
            {
                connection.Execute("INSERT INTO SchemaVersion(Hash) VALUES(@hash)", new { hash });
            }
        }

        private static void SeedDb()
        {
            using (var conn = CreateOpenConnection())
            {
                conn.Execute(
                    $"INSERT INTO Event (Name, Date, Time, StartDateTime, Deleted) VALUES ('{EventName.SupportGroup}', 'Wednesday, April 6, 2016', '10:00am - 11:00am', '{new DateTime(2016, 4, 6, 10, 0, 0).ToString("O")}', 0)");
                conn.Execute(
                    $"INSERT INTO Event (Name, Date, Time, StartDateTime, Deleted) VALUES ('{EventName.EducationalClass1}', 'Saturday, July 23, 2016', '10:00am - 11:00am', '{new DateTime(2016, 7, 23, 10, 0, 0).ToString("O")}', 0)");
                conn.Execute(
                    $"INSERT INTO Event (Name, Date, Time, StartDateTime, Deleted) VALUES ('{EventName.EducationalClass2}', 'Saturday, July 30, 2016', '10:00am - 11:00pm', '{new DateTime(2016, 7, 30, 10, 0, 0).ToString("O")}', 0)");
            }
        }
    }
}