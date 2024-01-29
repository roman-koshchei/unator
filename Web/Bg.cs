using System.Threading.Channels;

public static class Bg
{
    private static readonly Channel<Func<Task>> queue;
    private static bool run = false;

    static Bg()
    {
        var options = new UnboundedChannelOptions
        {
            // Queue is supposed to be singleton, otherwith change it to false
            SingleReader = true
        };
        queue = Channel.CreateUnbounded<Func<Task>>(options);
    }

    public static async Task Enqueue(Func<Task> task)
    {
        Console.WriteLine("Job enqueued");
        await queue.Writer.WriteAsync(task);
    }

    public static async Task Run()
    {
        // can start only once
        if (run) return;
        run = true;

        Console.WriteLine("Bg queue started");

        while (run)
        {
            var job = await queue.Reader.ReadAsync();
            try
            {
                await job();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, $"Exception appeared during execution of Background queue job: {nameof(job)}.");
            }
        }
    }
};