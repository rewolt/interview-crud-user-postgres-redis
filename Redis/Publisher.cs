using CrudUsers.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CrudUsers.Redis
{
    public class Publisher
    {
        private readonly IConfiguration _configuration;
        private static ILogger<Subscriber>? _logger;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly ISubscriber _publisher;
        private readonly string _channelName;

        public Publisher(IConfiguration configuration, ILogger<Subscriber> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis")!);
            _publisher = _connectionMultiplexer.GetSubscriber();
            _channelName = _configuration["Redis:ChannelName"]!;
        }

        public async Task<bool> PublishAsync(User user, CancellationToken cancellation)
        {
            var msg = System.Text.Json.JsonSerializer.Serialize(user);
            
            if (!cancellation.IsCancellationRequested)
            {
                await _publisher.PublishAsync(_channelName, msg);
                _logger!.LogInformation($"Sent message:\n{msg}");
            }
            
            return true;
        }
    }
}
