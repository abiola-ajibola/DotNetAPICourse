// Dominic installed microsoft.entityframeworkcore and microsoft.entityframeworkcore.sqlserver separately
// but at https://learn.microsoft.com/en-us/ef/core/providers/sql-server/?tabs=dotnet-core-cli it is claimed
// that simply installing Microsoft.EntityFrameworkCore.SqlServer adds both.
// see https://learn.microsoft.com/en-us/ef/core/providers/sql-server/?tabs=dotnet-core-cli#install

using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Dapper;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;

namespace HelloWorld
{
    public class Program
    {
        public static void Main(/* string[] args */)
        {

            /* 
            For more on configurations, see the following;
            - https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration
            - https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration#alternative-hosting-approach
            - https://www.nuget.org/packages/Microsoft.Extensions.Hosting
            - https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createapplicationbuilder?view=net-9.0-pp#microsoft-extensions-hosting-host-createapplicationbuilder(system-string())
            - https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers
            - https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration?view=net-9.0-pp
             */

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


            // 1 and 2 at ./Data/DataContextDapper.cs
            DataContextDapper dapper = new(config);
            // 3. Query the database using the database connection
            // string response = dbCOnnection.QuerySingle<string>("SELECT GETDATE()"); // dbConnection has been moved to the DapperContext.
            string response = dapper.LoadSingle<string>("SELECT GETDATE()");
            // dbCOnnection.QuerySingle<string> means convert the response to a string.
            // It could also be dbCOnnection.QuerySingle<DateTime>, in which case response would be of type DateTime.
            Console.WriteLine(response);
            CultureInfo culture = new("en-US");
            // CultureInfo culture = new("fr-FR"); // This would not work because of the culture/locale format
            // MSSQL will break because the decimal point is represented with a comma


            // Setup entity framework too. We definitely don't need to use both entity framework and dapper in a real project
            // but this is just to show that we can use both to achieve the same goals (sometimes).
            DataContextEF entityFramework = new(config);

            Computer computer = new() // Can also be Computer computer = new Computer() 
            {
                CPUCores = 16,
                HasLTE = false,
                HasWifi = true,
                Motherboard = "MEG Z890",
                Price = 1500.55m,
                ReleaseDate = DateTime.Parse("2025-05-02 03:30:00 PM"),
                // ReleaseDate = DateTime.Now,
                VideoCard = "RTX 4060"
            };

            // Insert with dapper...
            // Multiline verbatim string starts
            string insertQuery = @"INSERT INTO TutorialAppSchema.Computers (
                Motherboard,
                CPUCores,
                HasWifi,
                HasLTE,
                Price,
                VideoCard,
                ReleaseDate
            ) VALUES 
            (
                '"
                // Multiline verbatim string ends

                // Use single quotes because of values that may have spaces and other special characters
                // like video card, release date, and motherboard.
                // Always try to use single quotes to wrap your variables, this can prevent some errors.
                + computer.Motherboard
                + "'," + computer.CPUCores
                + ",'" + computer.HasWifi
                + "','" + computer.HasLTE
                // See https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#standard-format-specifiers
                // to read about format specifiers.
                + "','" + computer.Price.ToString("F2", culture)
                + "','" + computer.VideoCard
                // ReleaseDate can also be converted to a date string, This prevents errors due to locales.
                // + "','" + computer.ReleaseDate.ToUniversalTime() // Convert to UTC time.
                + "','" + computer.ReleaseDate.ToString("G", culture) // To culture/locale specific date string.
            // see https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
                + "')";

            // Console.WriteLine(insertQuery);
            // int insertResponse = dbCOnnection.Execute(insertQuery);
            // int insertResponse = dapper.ExecuteSqlWithRowCount(insertQuery);
            // Console.WriteLine(insertResponse);


            //Insert with entity framework...
            var changeTracker = entityFramework.Add(computer); // to learn more about change tracking: https://learn.microsoft.com/en-us/ef/core/change-tracking/
            Console.WriteLine("before: " + changeTracker);
            // try to complete all operations before calling SaveChanges(), so that yuo only make a single requests to the database with
            // all the queries at once.
            int insertResponse = entityFramework.SaveChanges(); // You can also do `await entityFramework.SaveChangesAsync()`
            Console.WriteLine(insertResponse);
            Console.WriteLine("After: " + changeTracker);

            // Dominic said it's good practice to add the table names to the columns...
            string selectQuery = @"SELECT
                Computers.ComputerId,
                Computers.Motherboard,
                Computers.CPUCores,
                Computers.HasWifi,
                Computers.HasLTE,
                Computers.Price,
                Computers.VideoCard,
                Computers.ReleaseDate
             FROM TutorialAppSchema.Computers";

            Console.WriteLine(selectQuery);
            // Select with dapper...
            // IEnumerable<Computer> selectResponse = dbCOnnection.Query<Computer>(selectQuery);
            // IEnumerable<Computer> selectResponse = dapper.LoadData<Computer>(selectQuery);

            // Select with entity framework...
            IEnumerable<Computer> selectResponse = entityFramework.Computers;

            Console.WriteLine($"{"ID",10}\t{"Board",15}\t{"Cores",5}{"Wifi",7}{"LTE",7}{"Price",15}\t{"Video Card",20}\t{"Release Date",20}");
            foreach (Computer computer_data in selectResponse)
            {
                Console.WriteLine($"{computer_data.ComputerId,10}\t{computer_data.Motherboard,15}{computer_data.CPUCores,5}{computer_data.HasWifi,7}{computer_data.HasLTE,7}{computer_data.Price,15}\t{computer_data.VideoCard,20}\t{computer_data.ReleaseDate,20}");
            }

        }
    }
}