-- Test Autor User (koristimo ID = -100 da ne kolidira sa postojećim -11, -12, -13)
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES (-100, 'testauthor@tours.com', 'Test123!', 1, true);

-- Test Autor Person
INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email")
VALUES (-100, -100, 'Test', 'Author', 'testauthor@tours.com');

-- Test Ture
INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "CreatedAt", "Tags")
VALUES 
    (-1, 'Test Tour Draft', 'Draft tour for testing', 0, 0, 0, -100, '2025-01-01 10:00:00', CAST('["test", "draft"]' AS jsonb)),
    (-2, 'Test Tour Published', 'Published tour for testing', 1, 1, 500, -100, '2025-01-01 11:00:00', CAST('["test", "published"]' AS jsonb)),
    (-3, 'Test Tour Hard', 'Hard difficulty tour', 2, 0, 1000, -100, '2025-01-01 12:00:00', CAST('["test", "hard"]' AS jsonb));