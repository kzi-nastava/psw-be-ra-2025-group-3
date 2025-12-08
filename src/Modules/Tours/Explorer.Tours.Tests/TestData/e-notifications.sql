DELETE FROM tours."Notifications";

INSERT INTO tours."Notifications" 
("Id", "RecipientId", "Type", "RelatedEntityId", "Message", "IsRead", "CreatedAt", "ReadAt")
VALUES 
(-1, -11, 0, -99, 'Tourist sent a new message on problem: Bus was 30 minutes late at pickup point', false, '2024-12-06 10:30:00', NULL),
(-2, -21, 0, -99, 'Tour author responded to your problem on tour: Test Notification Tour Draft', true, '2024-12-06 11:00:00', '2024-12-06 11:05:00'),
(-3, -11, 1, -99, 'Tourist marked problem as RESOLVED on tour: Test Notification Tour Draft', true, '2024-12-05 18:30:00', '2024-12-05 19:00:00'),
(-4, -11, 2, -101, 'Tourist marked problem as UNRESOLVED on tour: Test Notification Tour Published. Immediate attention required!', false, '2024-12-05 21:15:00', NULL),
(-5, -22, 0, -101, 'Tour author responded to your problem on tour: Test Notification Tour Published', false, '2024-12-06 15:30:00', NULL),
(-6, -11, 0, -101, 'Tourist sent a new message on problem: Main attraction was closed without prior notice', false, '2024-12-06 18:45:00', NULL),
(-7, -21, 0, -100, 'Tour author responded to your problem on tour: Test Notification Tour Draft', true, '2024-12-05 19:00:00', '2024-12-05 19:10:00');
