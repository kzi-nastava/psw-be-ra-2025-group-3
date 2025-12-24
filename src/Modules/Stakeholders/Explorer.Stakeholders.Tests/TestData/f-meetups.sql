DELETE FROM stakeholders."Meetups";

INSERT INTO stakeholders."Meetups" 
("Id", "Title", "Description", "DateTime", "Address", "Latitude", "Longitude", "CreatorId", "TourId")
VALUES 
(-1, 'PSW Networking Event', 'Dogadjaj za studente PSW kursa koji �ele da se povezu i razmene iskustva.', 
 '2026-06-15 18:00:00', 'Bulevar oslobođenja 1', 45.2671, 19.8335, -21, -2),

(-2, 'Travel Enthusiasts Meetup', 'Meetup za sve ljubitelje putovanja. Donesite svoja iskustva!', 
 '2026-06-20 19:00:00', 'Trg slobode 3', 45.2550, 19.8450, -22, 1),

(-3, 'Photography & Travel', 'Kako fotografisati tokom putovanja? Saznajte od profesionalaca.', 
 '2026-06-25 17:00:00', 'Bulevar oslobođenja 1', 45.2400, 19.8500, -11, NULL),

(-4, 'Author Meetup for Deletion', 'This meetup will be deleted in tests.', 
 '2026-06-30 20:00:00', 'Trg slobode 3', 45.3000, 19.9000, -11, NULL);

