-- Testni podaci za Preferences tabelu (koristimo negativne ID-jeve)
INSERT INTO tours."Preferences" ("Id", "TouristId", "Difficulty", "WalkingRating", "BicycleRating", "CarRating", "BoatRating", "Tags") VALUES
(-1, -101, 1, 3, 2, 1, 0, '["Nature", "Culture"]'::jsonb),
(-2, -102, 2, 2, 3, 2, 1, '["Adventure", "History"]'::jsonb),
(-3, -103, 0, 3, 1, 0, 2, '["Relaxation", "Beach"]'::jsonb);