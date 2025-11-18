INSERT INTO tours."AwardEvents"(
    "Id", "Name", "Description", "Year", "Status", "VotingStartDate", "VotingEndDate", "CreatedAt", "UpdatedAt")
VALUES 
    (-1, 'Test Nagrada 2024', 'Opis za testiranje', 2024, 'Draft', '2024-10-01 10:00:00+00', '2024-10-10 10:00:00+00', CURRENT_TIMESTAMP, NULL),
    (-2, 'Test Nagrada 2023', 'Stara nagrada', 2023, 'VotingClosed', '2023-10-01 10:00:00+00', '2023-10-10 10:00:00+00', CURRENT_TIMESTAMP, NULL),
    (-3, 'Test Nagrada 2025', 'Buduca nagrada', 2025, 'Draft', '2025-10-01 10:00:00+00', '2025-10-10 10:00:00+00', CURRENT_TIMESTAMP, NULL);