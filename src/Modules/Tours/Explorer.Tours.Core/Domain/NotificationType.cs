namespace Explorer.Tours.Core.Domain;
public enum NotificationType
{
    NewMessage = 0,           // Nova poruka na problemu
    ProblemResolved = 1,      // Problem označen kao rešen (NOVO za Podtask 3)
    ProblemUnresolved = 2,    // Problem označen kao nerešen (NOVO za Podtask 3)
    DeadlineSet = 3,          // Administrator postavio deadline
    WalletTopUp = 4,         // Uplata AC
}
