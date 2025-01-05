using Azure.Messaging.ServiceBus;
using Comments.API.Interfaces;
using System.Text.Json;

namespace Ratbags.Comments.API.Services;

public class ServiceBusBackgroundService : BackgroundService
{
    private readonly string _connectionString;
    private readonly string _topicName;
    private readonly string _subscriptionName;

    // scoped comments service
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    // service bus
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    private readonly ILogger<ServiceBusBackgroundService> _logger;


    public ServiceBusBackgroundService(
        string connectionString,
        string topicName,
        string subscriptionName,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ServiceBusBackgroundService> logger)
    {
        _connectionString = connectionString;
        _topicName = topicName;
        _subscriptionName = subscriptionName;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client = new ServiceBusClient(_connectionString);

        _processor = _client.CreateProcessor(
            _topicName,
            _subscriptionName,
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 5
            });

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        _logger.LogInformation("Starting Comments Service Bus processor...");
        await _processor.StartProcessingAsync(stoppingToken);

        // When stopping, close everything
        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Stopping Service Bus processor...");
            _processor.CloseAsync().GetAwaiter().GetResult();
        });
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
        {
            _logger.LogInformation("Stopping Comments Service Bus processor...");
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            _logger.LogInformation($"Received message: {args.Message.MessageId}");
            _logger.LogInformation($"Body: {args.Message.Body}");

            var articleId = Guid.Parse(args.Message.Body.ToString());

            // set up comments service through service scope factory and get comments for article
            using var scope = _serviceScopeFactory.CreateScope();
            var commentsService = scope.ServiceProvider.GetRequiredService<ICommentsService>();
            var comments = await commentsService.GetByArticleIdAsync(articleId);

            // return to sender... yeah yeah yeah
            if (!string.IsNullOrEmpty(args.Message.ReplyTo))
            {
                var responseBody = JsonSerializer.Serialize(comments);

                // response with same correlationId
                var responseMessage = new ServiceBusMessage(responseBody)
                {
                    CorrelationId = args.Message.CorrelationId
                };

                // create sender for 'ReplyTo' topic
                var sender = _client!.CreateSender(args.Message.ReplyTo);
                await sender.SendMessageAsync(responseMessage);

                _logger.LogInformation($"Sent response to: {args.Message.ReplyTo}, CorrelationId: {args.Message.CorrelationId}");
            }

            // complete message to remove it from the subscription
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Message processing failed.");
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error in Service Bus Processor");
        return Task.CompletedTask;
    }
}
