using System;
using System.Data.SQLite;
using System.Text;

#nullable enable

// ----------------------------------------------------------------------------------------
// MARK: Setup Notes
// ----------------------------------------------------------------------------------------
// dotnet create console -n DBTestApp
// cd DBTestApp
// dotnet add package System.Data.SQLite
// dotnet workload update
// dotnet run
// ----------------------------------------------------------------------------------------

namespace DBTestApp;

class Program
{
    static void Main(string[] args)
    {
        // Specify the path for the SQLite database file.
        string dbPath = "files.db";
        var dbManager = new BinaryDataManager(dbPath);

        // write an arbitrary data structure to the database.
        {
            ByteArrayWriter writer = new ByteArrayWriter();
            writer.WriteInt(42);
            writer.WriteFloat(3.14f);
            writer.WriteDouble(0.123456789);
            writer.WriteBool(true);
            writer.WriteString("Hello, World!");
            dbManager.Add("adhoc", writer.ToArray());
        }

        // read back the strcture from the database.
        {
            byte[] data = dbManager.Get("adhoc");
            if (data != null)
            {
                ByteArrayReader reader = new ByteArrayReader(data);
                int    i     = reader.ReadInt();
                float  f     = reader.ReadFloat();
                double d     = reader.ReadDouble();
                bool   bool2 = reader.ReadBool();
                string str   = reader.ReadString();
                Console.WriteLine($"i: {i}, f: {f}, d: {d}, b: {bool2}, str: {str}");
            }
            else
            {
                Console.WriteLine("Failed to retrieve 'adhoc' data.");
            }
        }

        // Add some numbers to the database.
        double pi = 3.14159;
        byte[] b  = BytesConversionUtils.DoubleToBytes(pi);
        dbManager.Add("pi", b);

        DateTime now = DateTime.Now;
        byte[] b2 = BytesConversionUtils.DateTimeToBytes(now);
        dbManager.Add("now", b2);

        // List the database contents
        Console.WriteLine("Entries in the database:");
        List<string> dataNameList = dbManager.List();
        foreach (string currName in dataNameList)
            Console.WriteLine($"- {currName}");
    }
}
