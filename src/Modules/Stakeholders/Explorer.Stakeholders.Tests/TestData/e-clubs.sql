
INSERT INTO stakeholders."Clubs"("Id", "Name", "Description", "OwnerId", "FeaturedImageId")
VALUES
    (-1, 'Hiking Club', 'Club for mountain hiking', -21, NULL),
    (-2, 'Urbex Club', 'Urban exploration club', -22, NULL),
    (-3, 'Delete Club', 'This club will be deleted', -23, NULL);


INSERT INTO stakeholders."ClubImages"("Id", "ClubId", "ImageUrl", "UploadedAt")
VALUES
	(-1, -1, 'https://example.com/images/hiking_featured.jpg', '2025-01-01 10:00:00'),
	(-2, -2, 'https://example.com/images/urbex_featured.jpg', '2025-01-01 11:00:00'),
	(-3, -3, 'https://example.com/images/delete_featured.jpg', '2025-01-01 12:00:00'),
	(-4, -1, 'https://example.com/hiking_gallery1.jpg', '2025-01-01 10:05:00'),
	(-5, -2, 'https://example.com/hiking_gallery2.jpg', '2025-01-01 11:05:00'),
    (-6, -2, 'https://example.com/hiking_gallery3.jpg', '2025-01-01 11:10:00');


UPDATE stakeholders."Clubs" SET "FeaturedImageId" = -1 WHERE "Id" = -1;
UPDATE stakeholders."Clubs" SET "FeaturedImageId" = -2 WHERE "Id" = -2;
UPDATE stakeholders."Clubs" SET "FeaturedImageId" = -3 WHERE "Id" = -3;