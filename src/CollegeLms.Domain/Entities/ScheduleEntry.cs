using CollegeLms.Domain.Enums;

namespace CollegeLms.Domain.Entities;

public class ScheduleEntry
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid DisciplineId { get; set; }
    public Guid TeacherUserId { get; set; }
    public int DayOfWeek { get; set; }
    public int TimeSlot { get; set; }
    public string? Room { get; set; }
    public WeekType WeekType { get; set; } = WeekType.Both;

    public Group Group { get; set; } = null!;
    public Discipline Discipline { get; set; } = null!;
    public User Teacher { get; set; } = null!;
}
