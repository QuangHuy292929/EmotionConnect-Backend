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
    public DbSet<Community> Communities => Set<Community>();
    public DbSet<CommunityMember> CommunityMembers => Set<CommunityMember>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomMember> RoomMembers => Set<RoomMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<EmotionEntry> EmotionEntries => Set<EmotionEntry>();
    public DbSet<EmotionScore> EmotionScores => Set<EmotionScore>();
    public DbSet<TextEmbedding> TextEmbeddings => Set<TextEmbedding>();
    public DbSet<MatchingRequest> MatchingRequests => Set<MatchingRequest>();
    public DbSet<MatchingCandidate> MatchingCandidates => Set<MatchingCandidate>();
    public DbSet<Reflection> Reflections => Set<Reflection>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
