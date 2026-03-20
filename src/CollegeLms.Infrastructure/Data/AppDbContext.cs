using CollegeLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<StudentGroup> StudentGroups => Set<StudentGroup>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();
    public DbSet<TeacherDiscipline> TeacherDisciplines => Set<TeacherDiscipline>();
    public DbSet<GroupDiscipline> GroupDisciplines => Set<GroupDiscipline>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<TestQuestion> TestQuestions => Set<TestQuestion>();
    public DbSet<TestAnswer> TestAnswers => Set<TestAnswer>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<TestAttemptAnswer> TestAttemptAnswers => Set<TestAttemptAnswer>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<RatingEntry> RatingEntries => Set<RatingEntry>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseBlock> CourseBlocks => Set<CourseBlock>();
    public DbSet<CourseProgress> CourseProgress => Set<CourseProgress>();
    public DbSet<ScheduleEntry> ScheduleEntries => Set<ScheduleEntry>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256);
            e.Property(u => u.FullName).HasMaxLength(256);
            e.Property(u => u.PasswordHash).HasMaxLength(512);
            e.Property(u => u.PhotoPath).HasMaxLength(512);
            e.Property(u => u.Role).HasConversion<string>().HasMaxLength(32);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(rt => rt.Token).HasMaxLength(512);
        });

        // Group
        modelBuilder.Entity<Group>(e =>
        {
            e.Property(g => g.Name).HasMaxLength(64);
        });

        // StudentGroup
        modelBuilder.Entity<StudentGroup>(e =>
        {
            e.HasOne(sg => sg.User)
                .WithMany(u => u.StudentGroups)
                .HasForeignKey(sg => sg.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(sg => sg.Group)
                .WithMany(g => g.StudentGroups)
                .HasForeignKey(sg => sg.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(sg => new { sg.UserId, sg.GroupId })
                .HasFilter("\"RemovedAt\" IS NULL")
                .IsUnique();
        });

        // Discipline
        modelBuilder.Entity<Discipline>(e =>
        {
            e.Property(d => d.Name).HasMaxLength(256);
            e.Property(d => d.Code).HasMaxLength(32);
        });

        // TeacherDiscipline
        modelBuilder.Entity<TeacherDiscipline>(e =>
        {
            e.HasOne(td => td.User)
                .WithMany(u => u.TeacherDisciplines)
                .HasForeignKey(td => td.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(td => td.Discipline)
                .WithMany(d => d.TeacherDisciplines)
                .HasForeignKey(td => td.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(td => new { td.UserId, td.DisciplineId }).IsUnique();
        });

        // GroupDiscipline
        modelBuilder.Entity<GroupDiscipline>(e =>
        {
            e.HasOne(gd => gd.Group)
                .WithMany(g => g.GroupDisciplines)
                .HasForeignKey(gd => gd.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(gd => gd.Discipline)
                .WithMany(d => d.GroupDisciplines)
                .HasForeignKey(gd => gd.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(gd => new { gd.GroupId, gd.DisciplineId }).IsUnique();
        });

        // Topic
        modelBuilder.Entity<Topic>(e =>
        {
            e.HasOne(t => t.Discipline)
                .WithMany(d => d.Topics)
                .HasForeignKey(t => t.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(t => t.Title).HasMaxLength(512);
        });

        // Material
        modelBuilder.Entity<Material>(e =>
        {
            e.HasOne(m => m.Topic)
                .WithMany(t => t.Materials)
                .HasForeignKey(m => m.TopicId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(m => m.Discipline)
                .WithMany(d => d.Materials)
                .HasForeignKey(m => m.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(m => m.Title).HasMaxLength(256);
            e.Property(m => m.FilePath).HasMaxLength(512);
            e.Property(m => m.ExternalUrl).HasMaxLength(1024);
            e.Property(m => m.Type).HasConversion<string>().HasMaxLength(32);
        });

        // Test
        modelBuilder.Entity<Test>(e =>
        {
            e.HasOne(t => t.Topic)
                .WithMany(tp => tp.Tests)
                .HasForeignKey(t => t.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(t => t.Code).HasMaxLength(32);
            e.Property(t => t.Title).HasMaxLength(256);
        });

        // TestQuestion
        modelBuilder.Entity<TestQuestion>(e =>
        {
            e.HasOne(tq => tq.Test)
                .WithMany(t => t.Questions)
                .HasForeignKey(tq => tq.TestId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(tq => tq.Text).HasMaxLength(2048);
            e.Property(tq => tq.ImagePath).HasMaxLength(512);
        });

        // TestAnswer
        modelBuilder.Entity<TestAnswer>(e =>
        {
            e.HasOne(ta => ta.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(ta => ta.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(ta => ta.Text).HasMaxLength(1024);
        });

        // TestAttempt
        modelBuilder.Entity<TestAttempt>(e =>
        {
            e.HasOne(ta => ta.Test)
                .WithMany(t => t.Attempts)
                .HasForeignKey(ta => ta.TestId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ta => ta.User)
                .WithMany()
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(ta => new { ta.UserId, ta.TestId });
            e.Property(ta => ta.IpAddress).HasMaxLength(64);
        });

        // TestAttemptAnswer
        modelBuilder.Entity<TestAttemptAnswer>(e =>
        {
            e.HasOne(taa => taa.Attempt)
                .WithMany(a => a.Answers)
                .HasForeignKey(taa => taa.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(taa => taa.Question)
                .WithMany()
                .HasForeignKey(taa => taa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(taa => taa.SelectedAnswer)
                .WithMany()
                .HasForeignKey(taa => taa.SelectedAnswerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // JournalEntry
        modelBuilder.Entity<JournalEntry>(e =>
        {
            e.HasOne(je => je.User)
                .WithMany()
                .HasForeignKey(je => je.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(je => je.Discipline)
                .WithMany()
                .HasForeignKey(je => je.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(je => je.Group)
                .WithMany()
                .HasForeignKey(je => je.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(je => je.CreatedByUser)
                .WithMany()
                .HasForeignKey(je => je.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(je => new { je.UserId, je.DisciplineId, je.GroupId, je.Date });
        });

        // RatingEntry
        modelBuilder.Entity<RatingEntry>(e =>
        {
            e.HasOne(re => re.User)
                .WithMany()
                .HasForeignKey(re => re.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(re => re.Discipline)
                .WithMany()
                .HasForeignKey(re => re.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(re => re.Group)
                .WithMany()
                .HasForeignKey(re => re.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(re => re.CreatedByUser)
                .WithMany()
                .HasForeignKey(re => re.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(re => new { re.UserId, re.DisciplineId, re.GroupId, re.Date });
        });

        // Course
        modelBuilder.Entity<Course>(e =>
        {
            e.HasOne(c => c.Discipline)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.Title).HasMaxLength(256);
        });

        // CourseBlock
        modelBuilder.Entity<CourseBlock>(e =>
        {
            e.HasOne(cb => cb.Course)
                .WithMany(c => c.Blocks)
                .HasForeignKey(cb => cb.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(cb => cb.Type).HasConversion<string>().HasMaxLength(32);
            e.Property(cb => cb.Content).HasColumnType("jsonb");
        });

        // CourseProgress
        modelBuilder.Entity<CourseProgress>(e =>
        {
            e.HasOne(cp => cp.User)
                .WithMany()
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(cp => cp.CourseBlock)
                .WithMany(cb => cb.Progress)
                .HasForeignKey(cp => cp.CourseBlockId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(cp => new { cp.UserId, cp.CourseBlockId }).IsUnique();
        });

        // ScheduleEntry
        modelBuilder.Entity<ScheduleEntry>(e =>
        {
            e.HasOne(se => se.Group)
                .WithMany(g => g.ScheduleEntries)
                .HasForeignKey(se => se.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(se => se.Discipline)
                .WithMany()
                .HasForeignKey(se => se.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(se => se.Teacher)
                .WithMany()
                .HasForeignKey(se => se.TeacherUserId)
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(se => se.Room).HasMaxLength(32);
            e.Property(se => se.WeekType).HasConversion<string>().HasMaxLength(16);
            e.HasIndex(se => new { se.GroupId, se.DayOfWeek });
        });

        // Announcement
        modelBuilder.Entity<Announcement>(e =>
        {
            e.HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(a => a.Title).HasMaxLength(256);
        });
    }
}
