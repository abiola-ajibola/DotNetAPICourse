// https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
// https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios

namespace HelloWorld
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Task secondTask = ConsoleAfterDelayAsync("Task 2", 150);
            Task thirdTask = ConsoleAfterDelayAsync("Task 3", 50);
            Task firstTask = new Task(() =>
            {
                Thread.Sleep(100);
                Console.WriteLine("Task 1");

            });
            firstTask.Start();


            ConsoleAfterDelay("Delay", 75);


            Console.WriteLine("After the Task was created");
            await firstTask;
            await secondTask;
            await thirdTask;
        }

        static void ConsoleAfterDelay(string text, int delayTime)
        {
            Thread.Sleep(delayTime);
            Console.WriteLine(text);
        }

        static async Task ConsoleAfterDelayAsync(string text, int delayTime)
        {
            await Task.Delay(delayTime);
            Console.WriteLine(text);
        }

    }
}