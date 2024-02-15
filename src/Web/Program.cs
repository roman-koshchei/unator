using Unator;

var now = DateTime.UtcNow;

//Stopwatch stopwatch = new();
//stopwatch.Start();

//var channel = Channel.CreateUnbounded<string>();
//var tasks = new List<Task>();

//for (int i = 1; i < 10; i++)
//{
//    tasks.Add(DoWork(i * 1000, channel));
//}

//// Channel ballence automatically
//// So we can spin up any amount of readers without problems
//for (int i = 0; i < 3; i++)
//{
//    _ = Task.Factory.StartNew(async (num) =>
//    {
//        await foreach (var result in channel.Reader.ReadAllAsync())
//        {
//            Console.WriteLine($"r{num}: {result}");
//        }
//        Console.WriteLine($"Reader {num} is complete.");
//    }, state: i);
//}

//// Come together to stop reader
//Task.WaitAll([.. tasks]);
//channel.Writer.Complete();

//stopwatch.Stop();
//Console.WriteLine($"Overall time: {stopwatch.ElapsedMilliseconds} ms");

//static async Task DoWork(int duration, Channel<string> channel)
//{
//    Console.WriteLine("Doing work...");
//    await Task.Delay(duration);
//    Console.WriteLine("Work is done!");
//    await channel.Writer.WriteAsync($"Work duration was: {duration} ms");
//}