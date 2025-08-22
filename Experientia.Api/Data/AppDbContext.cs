using Experientia.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Technique> Techniques => Set<Technique>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobTechnique> JobTechniques => Set<JobTechnique>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);


        b.Entity<Technique>()
       .HasIndex(t => t.Name)
       .IsUnique();

        b.Entity<Job>()
         .HasOne(j => j.User)
         .WithMany(u => u.Jobs)
         .HasForeignKey(j => j.UserId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Entity<JobTechnique>().HasKey(jt => new { jt.JobId, jt.TechniqueId });

        b.Entity<JobTechnique>()
         .HasOne(jt => jt.Job)
         .WithMany(j => j.JobTechniques)
         .HasForeignKey(jt => jt.JobId)
         .OnDelete(DeleteBehavior.Cascade);

        b.Entity<JobTechnique>()
         .HasOne(jt => jt.Technique)
         .WithMany() // ingen back-navigation på Technique
         .HasForeignKey(jt => jt.TechniqueId)
         .OnDelete(DeleteBehavior.Restrict);

    }
}
