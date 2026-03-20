namespace CollegeLms.Domain.Entities;

public class Discipline
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? Semester { get; set; }

    public ICollection<TeacherDiscipline> TeacherDisciplines { get; set; } = new List<TeacherDiscipline>();
    public ICollection<GroupDiscipline> GroupDisciplines { get; set; } = new List<GroupDiscipline>();
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
