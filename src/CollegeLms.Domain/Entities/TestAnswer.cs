namespace CollegeLms.Domain.Entities;

public class TestAnswer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }

    public TestQuestion Question { get; set; } = null!;
}
