-- Test Accounts (negative IDs to avoid collisions)
INSERT INTO stakeholders."Accounts"(
    "Id", "Username", "Password", "Email", "Role", "Status")
VALUES
    (-1, 'test_admin', 'Test123!', 'admin.test@explorer.com', 0, 0),  -- Admin, Active
    (-2, 'test_author', 'Test123!', 'author.test@explorer.com', 1, 0), -- Author, Active
    (-3, 'test_tourist', 'Test123!', 'tourist.test@explorer.com', 2, 0); -- Tourist, Active

