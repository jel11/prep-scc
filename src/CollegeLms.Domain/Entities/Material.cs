using CollegeLms.Domain.Enums;

namespace CollegeLms.Domain.Entities;

public class Material
{
    public Guid Id { get; set; }
    public Guid? TopicId { get; set; }
    public Guid DisciplineId { get; set; }
    public MaterialType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string? ExternalUrl { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Topic? Topic { get; set; }
    public Discipline Discipline { get; set; } = null!;
}
