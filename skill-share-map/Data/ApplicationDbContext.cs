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
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<UserSkillProgress> UserSkillProgress { get; set; } = null!;
    public DbSet<UserBadge> UserBadges { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<AIConversation> AIConversations { get; set; } = null!;
    public DbSet<AIMessage> AIMessages { get; set; } = null!;
    public DbSet<TaskApplication> TaskApplications { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure SkillTask relationships
        modelBuilder.Entity<SkillTask>()
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SkillTask>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<TaskApplication>()
            .HasOne(a => a.Task)
            .WithMany(t => t.Applications)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Job relationships
        modelBuilder.Entity<Job>()
            .HasOne(j => j.PostedBy)
            .WithMany(u => u.PostedJobs)
            .HasForeignKey(j => j.PostedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Message relationships
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

        // Configure Rating relationships
        modelBuilder.Entity<Rating>()
            .HasOne(r => r.FromUser)
            .WithMany(u => u.GivenRatings)
            .HasForeignKey(r => r.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rating>()
            .HasOne(r => r.ToUser)
            .WithMany(u => u.ReceivedRatings)
            .HasForeignKey(r => r.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Rating-Task relationship (one-to-one)
        modelBuilder.Entity<Rating>()
            .HasOne(r => r.Task)
            .WithOne(t => t.Rating)
            .HasForeignKey<Rating>(r => r.TaskId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure UserSkillProgress relationships
        modelBuilder.Entity<UserSkillProgress>()
            .HasOne(p => p.User)
            .WithMany(u => u.SkillProgress)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure UserBadge relationships
        modelBuilder.Entity<UserBadge>()
            .HasOne(b => b.User)
            .WithMany(u => u.Badges)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure WalletTransaction relationships
        modelBuilder.Entity<WalletTransaction>()
            .HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure precision for decimal fields
        modelBuilder.Entity<SkillTask>()
            .Property(t => t.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SkillTask>()
            .Property(t => t.NegotiatedPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SkillTask>()
            .Property(t => t.DepositAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TaskApplication>()
            .Property(a => a.ProposedPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<WalletTransaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<WalletTransaction>()
            .Property(t => t.BalanceAfter)
            .HasPrecision(18, 2);

        // Configure AIConversation relationships
        modelBuilder.Entity<AIConversation>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AIMessage>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}