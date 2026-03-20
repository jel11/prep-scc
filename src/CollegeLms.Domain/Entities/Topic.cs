namespace CollegeLms.Domain.Entities;

public class Topic
{
    public Guid Id { get; set; }
    public Guid DisciplineId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public int? Hours { get; set; }

    public Discipline Discipline { get; set; } = null!;
    public ICollection<Test> Tests { get; set; } = new List<Test>();
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
