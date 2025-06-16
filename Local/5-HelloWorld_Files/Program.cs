using System.Globalization;
using System.Text.Json;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

            // 3. Query the database using the database connection

            CultureInfo culture = new("en-US");
            // CultureInfo culture = new("fr-FR"); // This would not work because of the culture/locale format
            // MSSQL will break because the decimal point is represented with a comma

            DataContextEF entityFramework = new(config);

            Computer computer = new() // Can also be Computer computer = new Computer() 
            {
                CPUCores = 16,
                HasLTE = false,
                HasWifi = true,
                Motherboard = "MEG Z890",
                Price = 1500.55m,
                ReleaseDate = DateTime.Parse($"2025-05-02 03:{formatInt(DateTime.Now.Minute)}:{formatInt(DateTime.Now.Second)} PM"),
                VideoCard = "RTX 4060"
            };

            IQueryable<Computer> data = entityFramework.Computers.Where(d => d.CPUCores > 4 && d.CPUCores < 13);

            foreach (Computer computer_data in data)
            {
                Console.WriteLine($"{computer_data.ComputerId,10}\t{computer_data.Motherboard,15}{computer_data.CPUCores,5}{computer_data.HasWifi,7}{computer_data.HasLTE,7}{computer_data.Price,15}\t{computer_data.VideoCard,20}\t{computer_data.ReleaseDate,20}");
            }

            // entityFramework.Add(computer);
            // int insertResponse = entityFramework.SaveChanges();
            // Console.WriteLine(insertResponse);

            // https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer?view=net-9.0
            // https://learn.microsoft.com/en-us/dotnet/api/system.text.json?view=net-9.0
            // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to
            // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties
            var jsonSerializeroptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            // string jsonText = File.ReadAllText("Computers2.json"); // requires CamelCase option for deserialization
            string jsonText = File.ReadAllText("Computers.json");
            Computer[]? computers = System.Text.Json.JsonSerializer.Deserialize<Computer[]>(jsonText, jsonSerializeroptions);
            string computerText = System.Text.Json.JsonSerializer.Serialize(computer, jsonSerializeroptions);
            Console.WriteLine("computerText: " + computerText);

            /* 
            if (computers != null)
            {
                foreach (Computer computer_data in computers)
                {
                    Console.WriteLine($"{computer_data.ComputerId,10}\t{computer_data.Motherboard,15}{computer_data.CPUCores,5}{computer_data.HasWifi,7}{computer_data.HasLTE,7}{computer_data.Price,15}\t{computer_data.VideoCard,20}\t{computer_data.ReleaseDate,20}");
                }
            }
             */

            // see: https://www.nuget.org/packages/newtonsoft.json/
            // see: https://www.newtonsoft.com/json/help/html/Introduction.htm
            Computer[]? computersNewton = JsonConvert.DeserializeObject<Computer[]>(jsonText);
            // does not need an option to desialize in cammel case or another case.

            if (computersNewton != null)
            {
                foreach (Computer computer_data in computersNewton)
                {
                    Console.WriteLine($"{computer_data.ComputerId,10}\t{computer_data.Motherboard,15}{computer_data.CPUCores,5}{computer_data.HasWifi,7}{computer_data.HasLTE,7}{computer_data.Price,15}\t{computer_data.VideoCard,20}\t{computer_data.ReleaseDate,20}");
                }
            }
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver() // Without this, it will not use camel case; even if input string is camel case.
            };
            string newtonJson = JsonConvert.SerializeObject(computer, settings);

            Console.WriteLine("newtonJson: " + newtonJson);

            Computer? singleComputer = entityFramework.Computers.FirstOrDefault(c => c.CPUCores < 4);
            if (singleComputer != null)
            {
                File.WriteAllText("temp.log.txt", $"{singleComputer.ComputerId,10}\t{singleComputer.Motherboard,15}{singleComputer.CPUCores,5}{singleComputer.HasWifi,7}{singleComputer.HasLTE,7}{singleComputer.Price,15}\t{singleComputer.VideoCard,20}\t{singleComputer.ReleaseDate,20}");
            }
            else
            {
                Console.WriteLine("No element");
            }

            // Select with entity framework...
            IEnumerable<Computer> selectResponse = entityFramework.Computers;


            // Console.WriteLine($"{"ID",10}\t{"Board",15}\t{"Cores",5}{"Wifi",7}{"LTE",7}{"Price",15}\t{"Video Card",20}\t{"Release Date",20}");
            using StreamWriter fileStream = new("log.txt", true);
            fileStream.WriteLine($"{"ID",10}\t{"Board",15}\t{"Cores",5}{"Wifi",7}{"LTE",7}{"Price",15}\t{"Video Card",20}\t{"Release Date",20}");

            foreach (Computer computer_data in selectResponse)
            {
                // Console.WriteLine($"{computer_data.ComputerId,10}\t{computer_data.Motherboard,15}{computer_data.CPUCores,5}{computer_data.HasWifi,7}{computer_data.HasLTE,7}{computer_data.Price,15}\t{computer_data.VideoCard,20}\t{computer_data.ReleaseDate,20}");
                fileStream.WriteLine($"{computer_data.ComputerId,10}\t{computer_data.Motherboard,15}{computer_data.CPUCores,5}{computer_data.HasWifi,7}{computer_data.HasLTE,7}{computer_data.Price,15}\t{computer_data.VideoCard,20}\t{computer_data.ReleaseDate,20}");
            }
            fileStream.Close();

            string content = File.ReadAllText("log.txt");
            // Console.WriteLine(content);

            static string formatInt(int number)
            {
                if (number < 10)
                {
                    return "0" + number.ToString();
                }
                return number.ToString();
            }
        }
    }
}