namespace CollegeLms.Domain.Entities;

public class Test
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitMinutes { get; set; }
    public int ThresholdExcellent { get; set; } = 90;
    public int ThresholdGood { get; set; } = 75;
    public int ThresholdSatisfactory { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Topic Topic { get; set; } = null!;
    public ICollection<TestQuestion> Questions { get; set; } = new List<TestQuestion>();
    public ICollection<TestAttempt> Attempts { get; set; } = new List<TestAttempt>();
}
