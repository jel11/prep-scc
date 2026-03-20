namespace CollegeLms.Domain.Entities;

public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EnrollmentYear { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
    public ICollection<GroupDiscipline> GroupDisciplines { get; set; } = new List<GroupDiscipline>();
    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>();
}
