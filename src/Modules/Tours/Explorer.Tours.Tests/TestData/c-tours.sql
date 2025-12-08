INSERT INTO tours."Tours"(
    "Id", "Name", "Description", "Difficulty", "Status", "Price", "DistanceInKm", "AuthorId", "CreatedAt", "Tags", "TourDurations") 
VALUES 
    (-1, 'Test Tour Draft', 'Draft tour for testing', 0, 0, 0, 0, -11, '2025-01-01 10:00:00', CAST('["test", "draft"]' AS jsonb), '{{"TimeInMinutes": 30, "Transportation": 0}}' ), 

    (-2, 'Test Tour Published', 'Published tour for testing', 1, 1, 500, 0, -11, '2025-01-01 11:00:00', CAST('["test", "published"]' AS jsonb), '{{"TimeInMinutes": 30, "Transportation": 0}}'),

    (-3, 'Test Tour Hard', 'Hard difficulty tour', 2, 0, 1000, 0, -11, '2025-01-01 12:00:00', CAST('["test", "hard"]' AS jsonb), '{{"TimeInMinutes": 30, "Transportation": 0}}'),

    (-4, 'Test Tour Published 2', 'Another published tour for purchase tests', 1, 1, 700, 0, -11, '2025-01-01 13:00:00', CAST('["test","purchase"]' AS jsonb),'{{"TimeInMinutes": 30, "Transportation": 0}}');
