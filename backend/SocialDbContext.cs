using Microsoft.EntityFrameworkCore;
using SocialBackend.Models;
namespace SocialBackend.Data;
public class SocialDbContext : DbContext
{
public SocialDbContext(DbContextOptions<SocialDbContext> options) : base(options) { }
public DbSet<User> Users => Set<User>();
public DbSet<Connection> Connections => Set<Connection>();
public DbSet<DirectMessage> Messages => Set<DirectMessage>();
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
base.OnModelCreating(modelBuilder);
modelBuilder.Entity<Connection>()
.HasOne(c => c.Requester)
.WithMany(u => u.SentConnections)
.HasForeignKey(c => c.RequesterId)
.OnDelete(DeleteBehavior.Restrict);
modelBuilder.Entity<Connection>()
.HasOne(c => c.Receiver)
.WithMany(u => u.ReceivedConnections)
.HasForeignKey(c => c.ReceiverId)
.OnDelete(DeleteBehavior.Restrict);
}
}