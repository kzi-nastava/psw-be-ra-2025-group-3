INSERT INTO encounters."EncounterActivations"(
  "Id", "EncounterId", "TouristId", "Status", "ActivatedAt", "CompletedAt",
  "LastLocationUpdateAt", "CurrentLatitude", "CurrentLongitude"
)
VALUES 
-- COMPLETED (Status = 'Completed')
(-1, -1, -21, 'Completed', '2025-12-20 10:00:00', '2025-12-20 10:30:00', NULL, NULL, NULL),
(-4, -4, -22, 'Completed', '2025-12-25 08:00:00', '2025-12-25 12:00:00', NULL, NULL, NULL),
(-10, -1, -22, 'Completed', '2025-12-26 10:00:00', '2025-12-26 10:30:00', NULL, NULL, NULL),

-- FAILED (Status = 'Failed')
(-3, -3, -22, 'Failed', '2025-12-27 14:00:00', '2025-12-27 14:15:00', NULL, NULL, NULL),

-- IN_PROGRESS (Status = 'InProgress')
(-2, -2, -21, 'InProgress', '2025-12-28 09:00:00', NULL, '2025-12-28 09:05:00', 45.2551, 19.8451),
(-5, -3, -23, 'InProgress', '2025-12-28 11:00:00', NULL, '2025-12-28 11:10:00', 45.2517, 19.8658),
(-6, -4, -21, 'InProgress', '2025-12-28 12:00:00', NULL, '2025-12-28 12:05:00', 45.1514, 19.7208),
(-7, -2, -22, 'InProgress', '2025-12-28 13:00:00', NULL, '2025-12-28 13:05:00', 45.2551, 19.8451);