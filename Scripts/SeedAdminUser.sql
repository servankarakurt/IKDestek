-- Test girişi: admin@hepiyi.com / Admin123!
-- Bu script'i sadece bu e-posta ile kayıt yoksa çalıştırın.
IF NOT EXISTS (SELECT 1 FROM Employees WHERE Email = N'admin@hepiyi.com')
BEGIN
    SET IDENTITY_INSERT Employees ON;
    INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, CardID, BirthDate, StartDate, Department, Roles, PasswordHash, MustChangePassword, CreatedTime, Isactive, IsDeleted)
    VALUES (1, N'Sistem', N'Admin', N'admin@hepiyi.com', N'', 0, '1990-01-01', '2024-01-01', 1, 1, N'PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=', 0, '2024-01-01', 1, 0);
    SET IDENTITY_INSERT Employees OFF;
END
