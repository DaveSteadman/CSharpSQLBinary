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

        // Prepare the content for readme.txt
        //string readmeContent = "This is a nominal readme file for the database.";
        //byte[] readmeBytes = Encoding.UTF8.GetBytes(readmeContent);

        // Get the readme.txt content from the local file, to add to the database.
        if (!File.Exists("readme.txt"))
        {
            Console.WriteLine("The file readme.txt does not exist in the current directory.");
            return;
        }
        byte[] readmeBytes = File.ReadAllBytes("readme.txt");

        // Add readme.txt to the database.
        bool success = dbManager.Add("readme.txt", readmeBytes);
        Console.WriteLine(success ? "readme.txt added successfully." : "Failed to add readme.txt.");

        // Optional: Retrieve and display the file content.
        byte[] retrieved = dbManager.Get("readme.txt");
        if (retrieved != null)
        {
            string content = Encoding.UTF8.GetString(retrieved);
            Console.WriteLine("Retrieved content: " + content);
        }

        // write an arbitrary data structure to the database.
        {
            ByteArrayWriter writer = new ByteArrayWriter();
            writer.WriteInt(42);
            writer.WriteFloat(3.14f);
            writer.WriteDouble(0.123456789);
            writer.WriteBool(true);
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
                Console.WriteLine($"i: {i}, f: {f}, d: {d}, b: {bool2}");
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
        List<string> files = dbManager.List();
        foreach (string file in files)
            Console.WriteLine($"- {file}");

        // var dbManager = new AsyncBinaryDataManager("path/to/your.db");

        // // For example, to add data:
        // bool success = await dbManager.Add("tile_001", someByteData);

        // // Or to check if data exists:
        // bool exists = await dbManager.DataExists("tile_001");
    }
}
