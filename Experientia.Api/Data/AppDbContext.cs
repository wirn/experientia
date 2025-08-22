using Experientia.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Technique> Techniques => Set<Technique>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Job>()
         .HasOne(j => j.User)
         .WithMany(u => u.Jobs)
         .HasForeignKey(j => j.UserId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
