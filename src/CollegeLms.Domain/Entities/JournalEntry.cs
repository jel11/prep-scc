namespace CollegeLms.Domain.Entities;

public class JournalEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DisciplineId { get; set; }
    public Guid GroupId { get; set; }
    public DateOnly Date { get; set; }
    public int? Grade { get; set; }
    public string? Comment { get; set; }
    public Guid CreatedByUserId { get; set; }

    public User User { get; set; } = null!;
    public Discipline Discipline { get; set; } = null!;
    public Group Group { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
}
