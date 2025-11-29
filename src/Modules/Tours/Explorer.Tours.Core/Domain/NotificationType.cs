namespace Explorer.Tours.Core.Domain;
public enum NotificationType
{
    NewMessage = 0, // Nova poruka na problemu
    DeadlineSet = 1, // Administrator postavio deadline
    DeadlineExpiring = 2, // Deadline istice uskoro (opciono)
    ProblemClosed = 3 // Problem zatvoren (opciono)
}
