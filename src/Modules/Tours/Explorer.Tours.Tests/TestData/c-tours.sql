-- Test Autor (negativan ID da ne kolidira sa realnim podacima)
INSERT INTO stakeholders."People"(
    "Id", "Name", "Surname", "Email")
VALUES (-1, 'Test', 'Author', 'test.author@tours.com');

INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive", "PersonId")
VALUES (-1, 'testauthor', 'Test123!', 1, true, -1);

-- Test Ture
INSERT INTO tours."Tours"(
    "Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "CreatedAt", "Tags")
VALUES 
    (-1, 'Test Tour Draft', 'Draft tour for testing', 0, 0, 0, -1, '2025-01-01 10:00:00', CAST('["test", "draft"]' AS jsonb)),
    (-2, 'Test Tour Published', 'Published tour for testing', 1, 1, 500, -1, '2025-01-01 11:00:00', CAST('["test", "published"]' AS jsonb)),
    (-3, 'Test Tour Hard', 'Hard difficulty tour', 2, 0, 1000, -1, '2025-01-01 12:00:00', CAST('["test", "hard"]' AS jsonb));