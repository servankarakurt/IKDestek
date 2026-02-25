-- Test personel (çalışan) hesabı - DB'ye şifre ile ekleme
-- E-posta: personel@hepiyi.com
-- Şifre: Personel123!
-- Rol: Çalışan (4)
-- Bu script'i bir kez çalıştırın (SSMS veya sqlcmd ile).

IF NOT EXISTS (SELECT 1 FROM Employees WHERE Email = N'personel@hepiyi.com')
BEGIN
    INSERT INTO Employees (
        FirstName, LastName, Email, Phone, CardID, BirthDate, StartDate,
        Department, Roles, PasswordHash, MustChangePassword,
        CreatedTime, Isactive, IsDeleted
    )
    VALUES (
        N'Test',
        N'Personel',
        N'personel@hepiyi.com',
        N'5551234567',
        9001,
        '1995-05-15',
        '2024-01-01',
        1,   -- Yazılım
        4,   -- Çalışan (Roles enum)
        N'$2a$11$yyIJoUxQfNxp6itxlMDLjeSC/pbP5eYw3iqlKz867fATxLQAOaIO2',  -- Personel123!
        0,
        GETUTCDATE(),
        1,
        0
    );
    PRINT 'Personel hesabı oluşturuldu: personel@hepiyi.com / Personel123!';
END
ELSE
    PRINT 'Bu e-posta zaten kayıtlı.';
