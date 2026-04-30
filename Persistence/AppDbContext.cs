using System;
using Microsoft.EntityFrameworkCore;
using Domain;
public class AppDbContext : DbContext
{
    public required DbSet<Activity> Activities { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
