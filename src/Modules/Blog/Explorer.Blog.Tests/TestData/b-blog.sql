-- Test data za Blog tabelu
INSERT INTO blog."Blogs"("Id", "Title", "Description", "CreationDate", "AuthorId")
VALUES 
(-1, 'Planinarenje u Alpima', 'Vodi? za planinarenje u Alpima sa savjetima za po?etnike i iskusne planinare.', '2025-01-15 10:00:00', -11),
(-2, 'Kulinarsko putovanje kroz Italiju', 'Otkrijte autenti?ne recepte i restorane širom Italije.', '2025-02-20 14:30:00', -11),
(-3, 'Fotografija pejzaža', 'Tehnike fotografisanja prirode i pejzaža tokom putovanja.', '2025-03-10 09:15:00', -12);

-- Test data za BlogImages tabelu
INSERT INTO blog."BlogImages"("Id", "ImageUrl", "BlogId")
VALUES
(-1, 'https://example.com/alps1.jpg', -1),
(-2, 'https://example.com/alps2.jpg', -1),
(-3, 'https://example.com/italy-food.jpg', -2),
(-4, 'https://example.com/landscape-photo.jpg', -3);