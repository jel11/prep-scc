namespace CollegeLms.Api.Authorization;

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string TeacherOrAdmin = "TeacherOrAdmin";
    public const string StudentOnly = "StudentOnly";
}
