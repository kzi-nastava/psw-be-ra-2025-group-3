INSERT INTO encounters."Encounters"(
  "Id", "Name", "Description", "Latitude", "Longitude", "XP", "Status", "Type",
  "ActionDescription", "RequiredPeopleCount", "RangeInMeters", "ImageUrl"
)
VALUES 
-- Misc Encounter (Type = 'Misc', Status = 'Draft')
(-5, 'Draft Encounter Example', 'This encounter is still being prepared', 
 45.2671, 19.8335, 200, 'Draft', 'Misc',
 'Complete simple task', NULL, NULL, NULL),

-- Hidden Location Encounters (Type = 'HiddenLocation', Status = 'Active')
(-1, 'Petrovaradin Fortress Challenge', 'Find the hidden spot using the image hint', 
 45.2517, 19.8658, 100, 'Active', 'HiddenLocation',
 NULL, NULL, NULL, 'https://picsum.photos/id/10/2500/1667'),

(-2, 'Danube River Mystery', 'Use the image to find the secret location', 
 45.2551, 19.8451, 75, 'Active', 'HiddenLocation',
 NULL, NULL, NULL, 'https://picsum.photos/id/20/3670/2462'),

(-4, 'Fruška Gora Monastery Hunt', 'Discover the hidden monastery', 
 45.1514, 19.7208, 150, 'Active', 'HiddenLocation',
 NULL, NULL, NULL, 'https://picsum.photos/id/30/1280/901'),

-- Social Encounter (Type = 'Social', Status = 'Active')
(-3, 'Meet a Local Guide', 'Gather with other tourists at this location', 
 45.2517, 19.8658, 50, 'Active', 'Social',
 NULL, 3, 15.0, NULL),

-- Archived Hidden Location (Type = 'HiddenLocation', Status = 'Archived')
(-6, 'Archived Old Challenge', 'This challenge is no longer available', 
 45.2671, 19.8335, 300, 'Archived', 'HiddenLocation',
 NULL, NULL, NULL, 'https://picsum.photos/id/40/4106/2806');