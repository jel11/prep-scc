namespace CollegeLms.Domain.Entities;

public class TestQuestion
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public int OrderIndex { get; set; }

    public Test Test { get; set; } = null!;
    public ICollection<TestAnswer> Answers { get; set; } = new List<TestAnswer>();
}
