DELETE FROM tours."Messages";
DELETE FROM tours."TourProblems";

INSERT INTO tours."TourProblems"(
    "Id", "TourId", "TouristId", "AuthorId", "Category", "Priority", 
    "Description", "Time", "CreatedAt", "UpdatedAt", 
    "Status", "ResolvedByTouristComment", "IsHighlighted", "AdminDeadline"
)
VALUES 
(-99, -1, -11, -21, 0, 2, 'Bus was 30 minutes late at pickup point', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
(-100, -1, -11, -21, 2, 1, 'Tour guide spoke too quietly, hard to hear explanations', '2024-11-15 10:15:00', '2024-11-15 12:00:00', NULL, 0, NULL, false, NULL),
(-101, -2, -12, -22, 3, 3, 'Main attraction was closed without prior notice', '2024-11-14 14:00:00', '2024-11-14 15:30:00', NULL, 0, NULL, false, NULL),
(-102, -1, -12, -21, 4, 0, 'Lunch portion was smaller than expected', '2024-11-15 12:30:00', '2024-11-15 13:00:00', NULL, 0, NULL, false, NULL),
(-103, -1, -11, -21, 0, 2, 'Problem for resolving test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
(-104, -1, -11, -21, 0, 2, 'Problem for unresolving test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL),
(-105, -1, -11, -21, 0, 2, 'Problem for message test', '2024-11-15 08:30:00', '2024-11-15 09:00:00', NULL, 0, NULL, false, NULL);