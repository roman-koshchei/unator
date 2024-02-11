using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Channels;
using Unator;

Stopwatch stopwatch = new();
stopwatch.Start();

var channel = Channel.CreateUnbounded<string>();
var tasks = new List<Task>();

for (int i = 1; i < 9; i++)
{
    tasks.Add(DoWork(i * 1000, channel));
}

_ = Task.Run(async () =>
{
    await foreach (var result in channel.Reader.ReadAllAsync())
    {
        Console.WriteLine($"r1: {result}");
    }
    Console.WriteLine("Reader 1 is complete.");
});

_ = Task.Run(async () =>
{
    await foreach (var result in channel.Reader.ReadAllAsync())
    {
        Console.WriteLine($"r2: {result}");
    }
    Console.WriteLine("Reader 2 is complete.");
});

// Come together to stop reader
Task.WaitAll([.. tasks]);
channel.Writer.Complete();

stopwatch.Stop();
Console.WriteLine($"Overall time: {stopwatch.ElapsedMilliseconds} ms");

static async Task DoWork(int duration, Channel<string> channel)
{
    Console.WriteLine("Doing work...");
    await Task.Delay(duration);
    Console.WriteLine("Work is done!");
    await channel.Writer.WriteAsync($"Work duration was: {duration} ms");
}