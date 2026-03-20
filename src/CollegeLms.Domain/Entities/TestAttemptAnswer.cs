namespace CollegeLms.Domain.Entities;

public class TestAttemptAnswer
{
    public Guid Id { get; set; }
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid? SelectedAnswerId { get; set; }

    public TestAttempt Attempt { get; set; } = null!;
    public TestQuestion Question { get; set; } = null!;
    public TestAnswer? SelectedAnswer { get; set; }
}
