namespace CollegeLms.Domain.Entities;

public class StudentGroup
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RemovedAt { get; set; }

    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}
