INSERT INTO tours."TourExecutions"("TouristId", "TourId", "StartTime", "Status", "StartLatitude", "StartLongitude", "LastActivity", "ProgressPercentage", "CompletedKeyPoints", "CompletionTime", "AbandonTime")
VALUES 
(-25, -2, NOW() - INTERVAL '2 days', 0, 45.2500, 19.8300, NOW() - INTERVAL '2 hours', 50.0, '[]', NULL, NULL),
 (-26, -2, NOW() - INTERVAL '3 days', 0, 45.2500, 19.8300, NOW() - INTERVAL '3 hours', 20.0, '[]', NULL, NULL),
(-27, -2, NOW() - INTERVAL '15 days', 0, 45.2500, 19.8300, NOW() - INTERVAL '14 days', 60.0, '[]', NULL, NULL),
(-28, -2, NOW() - INTERVAL '1 day', 0, 45.2500, 19.8300, NOW() - INTERVAL '1 hour', 40.0, '[]', NULL, NULL);