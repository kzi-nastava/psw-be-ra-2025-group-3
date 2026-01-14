-- Obriši stare podatke
DELETE FROM encounters."EncounterActivations";
DELETE FROM encounters."Encounters";
DELETE FROM tours."Positions" WHERE "TouristId" IN (-21, -22, -23, -99);