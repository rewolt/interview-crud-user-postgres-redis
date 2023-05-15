using CrudUsers.Context;
using CrudUsers.Models;
using CrudUsers.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudUsers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly Publisher _publisher;
        private readonly IDbContextFactory<BartekContext> _contextFactory;

        public UsersController(IDbContextFactory<BartekContext> contextFactory, ILogger<UsersController> logger, Publisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAllPaged([FromQuery] int take, [FromQuery] int skip, CancellationToken cancellation)
        {
            _logger.LogInformation("Getting all users");
            
            using var context = await _contextFactory.CreateDbContextAsync(cancellation);
            
            return await context.Users
                .OrderBy(u => u.Email).ThenBy(u => u.LastName).ThenBy(u => u.FirstName)
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellation);
        }

        [HttpGet("user")]
        public async Task<User?> GetSingle([FromQuery] Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("Getting all users");

            using var context = await _contextFactory.CreateDbContextAsync(cancellation);

            return await context.Users
                .FirstOrDefaultAsync(x => x.Id == userId, cancellation);
        }

        [HttpPost("user")]
        public async Task<Guid> CreateUser([FromBody] User user, CancellationToken cancellation)
        {
            _logger.LogInformation("Creating user");

            using var context = await _contextFactory.CreateDbContextAsync(cancellation);
            await context.Users.AddAsync(user, cancellation);
            await context.SaveChangesAsync(cancellation);

            await _publisher.PublishAsync(user, cancellation);

            return user.Id;
        }

        [HttpPatch("user")]
        public async Task UpdateUser([FromQuery] Guid UserId, [FromBody] User user, CancellationToken cancellation)
        {
            _logger.LogInformation("Updatating user");

            using var context = await _contextFactory.CreateDbContextAsync(cancellation);
            var dbUser = context.Users.First(u => u.Id == UserId);

            if (!string.IsNullOrEmpty(user.FirstName))
                dbUser.FirstName = user.FirstName;

            if (!string.IsNullOrEmpty(user.LastName))
                dbUser.LastName = user.LastName;

            if (!string.IsNullOrEmpty(user.Email))
                dbUser.Email = user.Email;

            await context.SaveChangesAsync(cancellation);

        }

        [HttpDelete("user")]
        public async Task DeleteUser([FromQuery] Guid userId, CancellationToken cancellation)
        {
            _logger.LogInformation("Deleting user");

            using var context = await _contextFactory.CreateDbContextAsync(cancellation);
            var user = new User { Id = userId };
            context.Users.Attach(user);
            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellation);
        }
    }
}