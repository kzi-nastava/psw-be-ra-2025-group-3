namespace Explorer.Tours.Core.Domain;
public enum TourProblemStatus
{
    Open = 0, // Otvoren problem
    Resolved = 1, // Turista oznacio kao resen
    Unresolved = 2, // Turista oznacio kao neresen
    Closed = 3 // Administrator zatvorio problem
}
