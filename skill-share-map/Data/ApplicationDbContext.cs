using Microsoft.EntityFrameworkCore;
using SkillShareMap.Models;

namespace SkillShareMap.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SkillTask> SkillTasks { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the relationship between User and the tasks they created
        modelBuilder.Entity<SkillTask>()
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship between User and the tasks assigned to them
        modelBuilder.Entity<SkillTask>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Configure the relationship between Message and its sender and receiver
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Other configurations...
        base.OnModelCreating(modelBuilder);
    }
}