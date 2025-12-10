DELETE FROM tours."Messages";
DELETE FROM tours."TourProblems";

INSERT INTO tours."TourProblems"(
    "Id", "TourId", "TouristId", "AuthorId", "Category", "Priority", 
    "Description", "Time", "CreatedAt", "UpdatedAt", 
    "Status", "ResolvedByTouristComment", "IsHighlighted", "AdminDeadline"
)
VALUES 
-- TouristId = -21 (TURISTA iz baze), AuthorId = -11 (AUTOR iz baze)
(-1, -1, -21, -11, 0, 2, 'Bus was 30 minutes late at pickup point', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
(-2, -1, -21, -11, 2, 1, 'Tour guide spoke too quietly, hard to hear explanations', '2024-11-15 10:15:00', '2024-11-15 12:00:00', NULL, 0, NULL, false, NULL),
(-3, -2, -22, -12, 3, 3, 'Main attraction was closed without prior notice', '2024-11-14 14:00:00', '2024-11-14 15:30:00', NULL, 0, NULL, false, NULL),
(-4, -1, -22, -11, 4, 0, 'Lunch portion was smaller than expected', '2024-11-15 12:30:00', '2024-11-15 13:00:00', NULL, 0, NULL, false, NULL),
(-5, -1, -21, -11, 0, 2, 'Problem for resolving test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
(-6, -1, -21, -11, 0, 2, 'Problem for unresolving test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
(-7, -1, -21, -11, 0, 2, 'Problem for message test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL);