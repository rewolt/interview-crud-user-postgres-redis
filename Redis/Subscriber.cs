using CrudUsers.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CrudUsers.Redis
{
    public class Subscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private static ILogger<Subscriber>? _logger;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly ISubscriber _subscriber;

        public Subscriber(IConfiguration configuration, ILogger<Subscriber> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis")!);
            _subscriber = _connectionMultiplexer.GetSubscriber();
            
            _subscriber.Subscribe(_configuration["Redis:ChannelName"]!, Handler);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private Action<RedisChannel, RedisValue> Handler = (channel, message) =>
        {
            var msg = message.HasValue ? System.Text.Json.JsonSerializer.Deserialize<User>(message!) : null;
            _logger!.LogInformation($"Received message:\n{message!}");
        };

        public override void Dispose()
        {
            _subscriber.UnsubscribeAll();
            _connectionMultiplexer.Close();
            _connectionMultiplexer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
