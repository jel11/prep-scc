namespace CollegeLms.Domain.Entities;

public class Course
{
    public Guid Id { get; set; }
    public Guid DisciplineId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Discipline Discipline { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<CourseBlock> Blocks { get; set; } = new List<CourseBlock>();
}
