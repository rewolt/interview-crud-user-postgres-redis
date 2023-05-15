using CrudUsers.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudUsers.Context;

public class BartekContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public BartekContext(DbContextOptions<BartekContext> options) : base(options)
    {
    }
}
