namespace CollegeLms.Domain.Entities;

public class TeacherDiscipline
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DisciplineId { get; set; }

    public User User { get; set; } = null!;
    public Discipline Discipline { get; set; } = null!;
}
