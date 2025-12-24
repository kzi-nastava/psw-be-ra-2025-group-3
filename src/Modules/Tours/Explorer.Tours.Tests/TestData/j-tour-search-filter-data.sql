
DELETE FROM tours."TourReviews" WHERE "TourId" IN (-100, -101, -102, -103, -104);
DELETE FROM tours."KeyPoints" WHERE "TourId" IN (-100, -101, -102, -103, -104);
DELETE FROM tours."Tours" WHERE "Id" IN (-100, -101, -102, -103, -104);

-------------------------------------------------------
-- TEST TURE SA RAZLICITIM KRITERIJUMIMA
-------------------------------------------------------

-- Tura 1: Mountain Adventure (Hard, skuplja, visok rating)
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty", "Status", "Price", "DistanceInKm", 
    "AuthorId", "CreatedAt", "Tags", "TourDurations"
)
VALUES (
    -100,
    'Mountain Adventure',
    'Exciting mountain hiking tour with breathtaking views',
    2, -- Hard
    1, -- Published
    150.00,
    25.5,
    -13, 
    '2025-01-15 10:00:00',
    '["mountain", "hiking", "adventure"]',
    '[{{"TimeInMinutes": 180, "TransportType": 0}}]'
);

-- Tura 2: City Tour Belgrade (Easy, jeftinija, srednji rating)
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty", "Status", "Price", "DistanceInKm", 
    "AuthorId", "CreatedAt", "Tags", "TourDurations"
)
VALUES (
    -101,
    'City Tour Belgrade',
    'Discover the hidden gems of Belgrade',
    0, -- Easy
    1, -- Published
    50.00,
    8.0,
    -13,
    '2025-02-10 09:00:00',
    '["city", "culture", "history"]',
    '[{{"TimeInMinutes": 120, "TransportType": 0}}]'
);

-- Tura 3: Beach Relax (Easy, srednja cena, bez review-a)
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty", "Status", "Price", "DistanceInKm", 
    "AuthorId", "CreatedAt", "Tags", "TourDurations"
)
VALUES (
    -102,
    'Beach Relax Paradise',
    'Relax on pristine beaches with crystal clear water',
    0, -- Easy
    1, -- Published
    80.00,
    5.0,
    -12, -- Drugi autor
    '2025-03-05 11:00:00',
    '["beach", "relax", "summer"]',
    '[{{"TimeInMinutes": 240, "TransportType": 1}}]'
);

-- Tura 4: Extreme Sports (Hard, najskuplja, nizak rating)
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty", "Status", "Price", "DistanceInKm", 
    "AuthorId", "CreatedAt", "Tags", "TourDurations"
)
VALUES (
    -103,
    'Extreme Sports Challenge',
    'For adrenaline junkies only - bungee, rafting, and more',
    2, -- Hard
    1, -- Published
    200.00,
    30.0,
    -12,
    '2025-04-01 08:00:00',
    '["sport", "adventure", "extreme"]',
    '[{{"TimeInMinutes": 300, "TransportType": 2}}]'
);

-- Tura 5: Wine Tasting Tour (Medium, srednja cena, dobar rating)
INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty", "Status", "Price", "DistanceInKm", 
    "AuthorId", "CreatedAt", "Tags", "TourDurations"
)
VALUES (
    -104,
    'Wine Tasting Tour',
    'Visit the best wineries and taste premium wines',
    1, -- Medium
    1, -- Published
    120.00,
    15.0,
    -13, 
    '2025-05-20 10:00:00',
    '["wine", "culture", "food"]',
    '[{{"TimeInMinutes": 180, "TransportType": 2}}]'
);
------------------------------------------------------
-- KEY POINTS ZA TEST TURE (minimum 2 po turi)
------------------------------------------------------

-- KeyPoints za Mountain Adventure (-100)
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "ImageUrl", "Secret", "Latitude", "Longitude")
VALUES 
    (-200, -100, 'Mountain Base Camp', 'Starting point', 'http://img.com/base.jpg', 'secret1', 43.3209, 21.8958),
    (-201, -100, 'Summit Peak', 'Highest point', 'http://img.com/summit.jpg', 'secret2', 43.3309, 21.9058);

-- KeyPoints za City Tour Belgrade (-101)
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "ImageUrl", "Secret", "Latitude", "Longitude")
VALUES 
    (-202, -101, 'Kalemegdan Fortress', 'Historic fortress', 'http://img.com/kalemegdan.jpg', 'secret3', 44.8230, 20.4503),
    (-203, -101, 'Skadarlija Street', 'Bohemian quarter', 'http://img.com/skadarlija.jpg', 'secret4', 44.8172, 20.4639);

-- KeyPoints za Beach Relax (-102)
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "ImageUrl", "Secret", "Latitude", "Longitude")
VALUES 
    (-204, -102, 'Golden Beach', 'Main beach area', 'http://img.com/golden.jpg', 'secret5', 40.6401, 22.9444),
    (-205, -102, 'Beach Bar', 'Refreshments', 'http://img.com/bar.jpg', 'secret6', 40.6501, 22.9544);

-- KeyPoints za Extreme Sports (-103)
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "ImageUrl", "Secret", "Latitude", "Longitude")
VALUES 
    (-206, -103, 'Rafting Start', 'River entry', 'http://img.com/rafting.jpg', 'secret7', 43.1486, 20.5219),
    (-207, -103, 'Bungee Platform', 'Jump zone', 'http://img.com/bungee.jpg', 'secret8', 43.1586, 20.5319);

-- KeyPoints za Wine Tasting (-104)
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "ImageUrl", "Secret", "Latitude", "Longitude")
VALUES 
    (-208, -104, 'First Winery', 'Premium wines', 'http://img.com/winery1.jpg', 'secret9', 44.0165, 21.2686),
    (-209, -104, 'Second Winery', 'Vintage collection', 'http://img.com/winery2.jpg', 'secret10', 44.0265, 21.2786);

-----------------------------------------------------
-- TOUR REVIEWS ZA TESTIRANJE RATING FILTERA
-----------------------------------------------------

-- Mountain Adventure: 2 review-a (rating 5 i 4) → avg: 4.5
INSERT INTO tours."TourReviews" ("Id", "TourId", "TouristId", "Rating", "Comment", "CreatedAt", "ProgressPercentage", "IsEdited")
VALUES 
    (-300, -100, -21, 5, 'Amazing experience!', '2025-06-01 12:00:00', 100.0, false),
    (-301, -100, -22, 4, 'Great tour, recommend!', '2025-06-02 14:00:00', 100.0, false);

-- City Tour Belgrade: 1 review (rating 3) → avg: 3.0
INSERT INTO tours."TourReviews" ("Id", "TourId", "TouristId", "Rating", "Comment", "CreatedAt", "ProgressPercentage", "IsEdited")
VALUES 
    (-302, -101, -21, 3, 'Nice, but could be better', '2025-06-05 10:00:00', 100.0, false);

-- Beach Relax: BEZ review-a → avg: 0

-- Extreme Sports: 1 review (rating 2) → avg: 2.0
INSERT INTO tours."TourReviews" ("Id", "TourId", "TouristId", "Rating", "Comment", "CreatedAt", "ProgressPercentage", "IsEdited")
VALUES 
    (-303, -103, -22, 2, 'Too dangerous for me', '2025-06-10 16:00:00', 50.0, false);

-- Wine Tasting: 2 review-a (rating 4 i 5) → avg: 4.5
INSERT INTO tours."TourReviews" ("Id", "TourId", "TouristId", "Rating", "Comment", "CreatedAt", "ProgressPercentage", "IsEdited")
VALUES 
    (-304, -104, -21, 4, 'Excellent wines!', '2025-06-15 13:00:00', 100.0, false),
    (-305, -104, -22, 5, 'Perfect tour!', '2025-06-16 15:00:00', 100.0, false);