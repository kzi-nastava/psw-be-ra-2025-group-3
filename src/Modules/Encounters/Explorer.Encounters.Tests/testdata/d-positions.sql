-- Obriši stare pozicije
DELETE FROM tours."Positions" WHERE "TouristId" IN (-21, -22, -23, -99);

-- Dodaj početne pozicije
INSERT INTO tours."Positions" ("TouristId", "Latitude", "Longitude")
VALUES
(-21, 45.2517, 19.8658),  -- Pera na Petrovaradinu (Encounter -1)
(-22, 45.2517, 19.8658),  -- Mika na Petrovaradinu (Encounter -1)
(-23, 45.2517, 19.8658);  -- Steva na Petrovaradinu (Encounter -1)