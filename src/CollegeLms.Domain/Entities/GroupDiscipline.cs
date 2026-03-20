namespace CollegeLms.Domain.Entities;

public class GroupDiscipline
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid DisciplineId { get; set; }

    public Group Group { get; set; } = null!;
    public Discipline Discipline { get; set; } = null!;
}
