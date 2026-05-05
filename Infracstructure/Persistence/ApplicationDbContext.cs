using Application.DTOs.Matching;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomMember> RoomMembers => Set<RoomMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<EmotionEntry> EmotionEntries => Set<EmotionEntry>();
    public DbSet<EmotionScore> EmotionScores => Set<EmotionScore>();
    public DbSet<TextEmbedding> TextEmbeddings => Set<TextEmbedding>();
    public DbSet<MatchingRequest> MatchingRequests => Set<MatchingRequest>();
    public DbSet<MatchingCandidate> MatchingCandidates => Set<MatchingCandidate>();
    public DbSet<Reflection> Reflections => Set<Reflection>();
    public DbSet<CheckInSession> CheckInSessions => Set<CheckInSession>();
    public DbSet<MatchingCandidateSeed> MatchingCandidateSeeds => Set<MatchingCandidateSeed>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OutBoxMessage> OutBoxMessages => Set<OutBoxMessage>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<MatchingCandidateSeed>().HasNoKey();
        modelBuilder.Entity<MatchingCandidateSeed>().ToView(null);

        base.OnModelCreating(modelBuilder);
    }
}
