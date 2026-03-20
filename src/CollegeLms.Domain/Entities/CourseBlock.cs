using CollegeLms.Domain.Enums;

namespace CollegeLms.Domain.Entities;

public class CourseBlock
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public CourseBlockType Type { get; set; }
    public string Content { get; set; } = "{}";
    public int OrderIndex { get; set; }

    public Course Course { get; set; } = null!;
    public ICollection<CourseProgress> Progress { get; set; } = new List<CourseProgress>();
}
