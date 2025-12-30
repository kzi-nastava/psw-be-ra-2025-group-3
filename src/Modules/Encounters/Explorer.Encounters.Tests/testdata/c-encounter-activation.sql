-- Tourist -21: Kompletirao Petrovaradin Fortress Challenge
INSERT INTO encounters."EncounterActivations"("Id", "EncounterId", "TouristId", "Status", "ActivatedAt", "CompletedAt")
VALUES (-1, -1, -21, 'Completed', '2025-12-20 10:00:00', '2025-12-20 10:30:00');

-- Tourist -21: Ima aktivan Danube River Walk (za GetActiveEncounters test)
INSERT INTO encounters."EncounterActivations"("Id", "EncounterId", "TouristId", "Status", "ActivatedAt", "CompletedAt")
VALUES (-2, -2, -21, 'InProgress', '2025-12-28 09:00:00', NULL);

-- Tourist -22: Odustao od Meet a Local Guide
INSERT INTO encounters."EncounterActivations"("Id", "EncounterId", "TouristId", "Status", "ActivatedAt", "CompletedAt")
VALUES (-3, -3, -22, 'Failed', '2025-12-27 14:00:00', '2025-12-27 14:15:00');

-- Tourist -22: Kompletirao Fruska Gora Monastery Tour
INSERT INTO encounters."EncounterActivations"("Id", "EncounterId", "TouristId", "Status", "ActivatedAt", "CompletedAt")
VALUES (-4, -4, -22, 'Completed', '2025-12-25 08:00:00', '2025-12-25 12:00:00');

-- Tourist -23: Ima aktivan Meet a Local Guide (za AbandonEncounter test)
INSERT INTO encounters."EncounterActivations"("Id", "EncounterId", "TouristId", "Status", "ActivatedAt", "CompletedAt")
VALUES (-5, -3, -23, 'InProgress', '2025-12-28 11:00:00', NULL);

-- Tourist -23: Ima aktivan Danube River Walk (za CompleteEncounter test)
INSERT INTO encounters."EncounterActivations"("Id", "EncounterId", "TouristId", "Status", "ActivatedAt", "CompletedAt")
VALUES (-6, -2, -23, 'InProgress', '2025-12-28 12:00:00', NULL);