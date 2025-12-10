-- =====================================================
-- ADMIN TOUR PROBLEM TEST DATA
-- NAPOMENA: Ažuriraj datume pre pokretanja u pgAdmin!
-- =====================================================

DELETE FROM tours."Messages";
DELETE FROM tours."TourProblems";

-- =====================================================
-- OVERDUE PROBLEMI (stariji od 5 dana, Status=Open)
-- =====================================================
INSERT INTO tours."TourProblems"(
    "Id", "TourId", "TouristId", "AuthorId", "Category", "Priority", 
    "Description", "Time", "CreatedAt", "UpdatedAt", 
    "Status", "ResolvedByTouristComment", "IsHighlighted", "AdminDeadline"
)
VALUES 
(-101, -1, -21, -11, 0, 3, 
 'Bus never arrived at pickup location - had to find alternative transport', 
 '2025-11-29 08:00:00', '2025-11-29 08:30:00', NULL, 
 0, NULL, false, NULL),

(-102, -1, -21, -11, 1, 2, 
 'Hotel room was not available despite confirmed booking', 
 '2025-12-02 18:00:00', '2025-12-02 18:30:00', NULL, 
 0, NULL, false, NULL),

(-103, -2, -22, -12, 2, 1, 
 'Tour guide did not show up at meeting point', 
 '2025-12-03 09:00:00', '2025-12-03 09:30:00', NULL, 
 0, NULL, false, NULL),

(-104, -1, -22, -11, 3, 3, 
 'Main attraction was closed for renovation - no refund offered', 
 '2025-11-19 14:00:00', '2025-11-19 14:30:00', NULL, 
 0, NULL, false, NULL);

-- FRESH PROBLEMI (mlađi od 5 dana)
INSERT INTO tours."TourProblems"(
    "Id", "TourId", "TouristId", "AuthorId", "Category", "Priority", 
    "Description", "Time", "CreatedAt", "UpdatedAt", 
    "Status", "ResolvedByTouristComment", "IsHighlighted", "AdminDeadline"
)
VALUES 
(-105, -1, -21, -11, 4, 1, 
 'Lunch portion was smaller than advertised', 
 '2025-12-07 12:30:00', '2025-12-07 13:00:00', NULL, 
 0, NULL, false, NULL),

(-106, -2, -22, -12, 0, 2, 
 'Bus had no air conditioning despite 35°C temperature', 
 '2025-12-08 10:00:00', '2025-12-08 10:30:00', NULL, 
 0, NULL, false, NULL);

-- RESOLVED PROBLEM
INSERT INTO tours."TourProblems"(
    "Id", "TourId", "TouristId", "AuthorId", "Category", "Priority", 
    "Description", "Time", "CreatedAt", "UpdatedAt", 
    "Status", "ResolvedByTouristComment", "IsHighlighted", "AdminDeadline"
)
VALUES 
(-108, -1, -21, -11, 1, 2, 
 'Hotel changed our room after complaint about noise', 
 '2025-12-01 20:00:00', '2025-12-01 20:30:00', '2025-12-03 10:00:00', 
 1, 'Thank you for resolving this quickly!', false, NULL);

-- UNRESOLVED PROBLEM
INSERT INTO tours."TourProblems"(
    "Id", "TourId", "TouristId", "AuthorId", "Category", "Priority", 
    "Description", "Time", "CreatedAt", "UpdatedAt", 
    "Status", "ResolvedByTouristComment", "IsHighlighted", "AdminDeadline"
)
VALUES 
(-110, -1, -21, -11, 3, 3, 
 'Promised refund for closed attraction never received', 
 '2025-11-20 14:00:00', '2025-11-20 14:30:00', '2025-11-28 12:00:00', 
 2, 'Author promised refund but I still have not received it.', false, NULL);

-- PROBLEM SA PORUKAMA
INSERT INTO tours."TourProblems"(
    "Id", "TourId", "TouristId", "AuthorId", "Category", "Priority", 
    "Description", "Time", "CreatedAt", "UpdatedAt", 
    "Status", "ResolvedByTouristComment", "IsHighlighted", "AdminDeadline"
)
VALUES 
(-111, -1, -21, -11, 0, 2, 
 'Bus driver was rude and unprofessional', 
 '2025-11-30 16:00:00', '2025-11-30 16:30:00', NULL, 
 0, NULL, false, NULL);

INSERT INTO tours."Messages"("Id", "TourProblemId", "AuthorId", "Content", "Timestamp", "AuthorType")
VALUES 
(-201, -111, -21, 'The bus driver was extremely rude and yelled at passengers.', '2025-11-30 17:00:00', 0),
(-202, -111, -11, 'We sincerely apologize. Can you provide more details?', '2025-12-01 09:00:00', 1),
(-203, -111, -21, 'It happened around 3 PM. Other passengers witnessed this.', '2025-12-01 14:00:00', 0),
(-204, -111, -11, 'We are investigating this incident.', '2025-12-02 10:00:00', 1);