INSERT INTO stakeholders."AppRatings"
    ("Id", "UserId", "Rating", "Comment", "CreatedAt", "UpdatedAt")
VALUES
    -- Autori
    (-301, -11, 5, 'Odlicna aplikacija, sve radi!', '2025-01-01 10:00:00+00', NULL),
    (-302, -12, 4, 'Vrlo korisna i stabilna.', '2025-01-02 12:00:00+00', NULL),
    (-303, -13, 3, 'Moze bolje, ali OK.', '2025-01-03 15:00:00+00', NULL),

    -- Turisti
    (-304, -21, 5, 'Sjajno iskustvo!', '2025-01-04 09:30:00+00', NULL),
    (-305, -22, 2, 'Nisam previse zadovoljan.', '2025-01-05 13:45:00+00', NULL),
    (-306, -23, 4, 'Dobro izgleda i lako se koristi.', '2025-01-06 16:20:00+00', NULL);