namespace CollegeLms.Domain.Entities;

public class CourseProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseBlockId { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? QuizScore { get; set; }

    public User User { get; set; } = null!;
    public CourseBlock CourseBlock { get; set; } = null!;
}
