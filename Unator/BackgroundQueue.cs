using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Unator;

/// <summary>
/// Background queue implemented with channels. Perform tasks in background.
/// <example>
/// Oriented to use with ASP NET Core:
/// <code>
/// builder.Services.AddSingleton&lt;BackgroundQueue&gt;();
/// builder.Services.AddHostedService&lt;BackgroundQueue.HostedService&gt;();
/// </code>
/// </example>
/// </summary>
public class BackgroundQueue
{
    private readonly Channel<Func<IServiceProvider, Task>> queue;

    public BackgroundQueue()
    {
        var options = new UnboundedChannelOptions
        {
            // Because service is singleton.
            SingleReader = true
        };
        queue = Channel.CreateUnbounded<Func<IServiceProvider, Task>>(options);
    }

    /// <summary>
    /// Add new task to background queue.
    /// <example>
    /// For example:
    /// <code>
    /// await background.Enqueue(async (provider) =>
    /// {
    ///     using IServiceScope scope = provider.CreateScope();
    ///     var service = scope.ServiceProvider.GetRequiredService&lt;IEmailSender&gt;();
    ///     await service.Send(
    ///         fromEmail: "roman@flurium.com",
    ///         fromName: "Roman Koshchei",
    ///         to: new List&lt;string&gt; { "you@example.com" },
    ///         subject: "Showing example of Background queue",
    ///         text: "Check it on GitHub repo: roman-koshchei/unator",
    ///         html: "Check it on GitHub repo: roman-koshchei/unator"
    ///     )
    /// });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="task">
    /// Action to perform in background queuqe.
    /// If action is sync then just return <c>Task.CompletedTask</c>
    /// </param>
    /// <returns></returns>
    public async Task Enqueue(Func<IServiceProvider, Task> task)
    {
        await queue.Writer.WriteAsync(task);
    }

    public class HostedService : BackgroundService
    {
        private readonly ILogger<HostedService> logger;
        private readonly BackgroundQueue queue;
        private readonly IServiceProvider serviceProvider;

        public HostedService(
            ILogger<HostedService> logger,
            BackgroundQueue queue,
            IServiceProvider serviceProvider
        )
        {
            this.logger = logger;
            this.queue = queue;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Background queue Hostend service is started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await queue.queue.Reader.ReadAsync(stoppingToken);
                try
                {
                    await job(serviceProvider);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Exception appeared during execution of Background queue job: {nameof(job)}.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Background queue Hosted Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}