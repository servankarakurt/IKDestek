-- Stajyer test hesabı
-- E-posta: stajyer@hepiyi.com
-- Şifre: Stajyer123!
-- Bu script'i bir kez çalıştırın (SSMS veya sqlcmd ile).

IF NOT EXISTS (SELECT 1 FROM Interns WHERE Email = N'stajyer@hepiyi.com')
BEGIN
    INSERT INTO Interns (FirstName, LastName, Email, Phone, University, Major, Grade, StartDate, EndDate, MentorId, CreatedTime, Isactive, IsDeleted, MustChangePassword, PasswordHash)
    VALUES (
        N'Test',
        N'Stajyer',
        N'stajyer@hepiyi.com',
        N'',
        N'Test Üniversite',
        N'Bilgisayar Mühendisliği',
        3,
        '2025-01-01',
        '2025-06-30',
        NULL,
        GETUTCDATE(),
        1,
        0,
        0,
        N'$2a$11$RBdUXGFg71.3TI4Pzd0K9uAXo4FemJ/dZAev9WZ05rc0KrYQHyYpK'
    );
    PRINT 'Stajyer hesabı oluşturuldu: stajyer@hepiyi.com / Stajyer123!';
END
ELSE
    PRINT 'Bu e-posta zaten kayıtlı.';
