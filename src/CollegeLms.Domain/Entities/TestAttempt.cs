namespace CollegeLms.Domain.Entities;

public class TestAttempt
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int? Score { get; set; }
    public int? Grade { get; set; }
    public string? IpAddress { get; set; }

    public Test Test { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<TestAttemptAnswer> Answers { get; set; } = new List<TestAttemptAnswer>();
}
