using System.Text.Json;
using AutoMapper;
using HelloWorld.Models;

string computersJson = File.ReadAllText("ComputersSnake.json");

// see https://docs.automapper.org/en/stable/Getting-started.html
Mapper mapper = new Mapper(new MapperConfiguration((cfg) =>
{
    cfg.CreateMap<ComputerSnake, Computer>()
        .ForMember(destination => destination.ComputerId, options =>
            options.MapFrom(source => source.computer_id))
        .ForMember(destination => destination.CPUCores, options =>
            options.MapFrom(source => source.cpu_cores))
        .ForMember(destination => destination.HasLTE, options =>
            options.MapFrom(source => source.has_lte))
        .ForMember(destination => destination.HasWifi, options =>
            options.MapFrom(source => source.has_wifi))
        .ForMember(destination => destination.Motherboard, options =>
            options.MapFrom(source => source.motherboard))
        .ForMember(destination => destination.VideoCard, options =>
            options.MapFrom(source => source.video_card))
        .ForMember(destination => destination.ReleaseDate, options =>
            options.MapFrom(source => source.release_date))
        .ForMember(destination => destination.Price, options =>
            options.MapFrom(source => source.price));
}));

IEnumerable<ComputerSnake>? computersSystem = JsonSerializer.Deserialize<IEnumerable<ComputerSnake>>(computersJson);

if (computersSystem != null)
{
    IEnumerable<Computer> computerResult = mapper.Map<IEnumerable<Computer>>(computersSystem);
    Console.WriteLine("Automapper Count: " + computerResult.Count());
    // foreach (Computer computer in computerResult)
    // {
    //     Console.WriteLine(computer.Motherboard);
    // }
}

IEnumerable<Computer>? computersJsonPropertyMapping = JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson);
if (computersJsonPropertyMapping != null)
{
    Console.WriteLine("JSON Property Count: " + computersJsonPropertyMapping.Count());
    // foreach (Computer computer in computersJsonPropertyMapping)
    // {
    //     Console.WriteLine(computer.Motherboard);
    // }
}
