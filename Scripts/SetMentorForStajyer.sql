-- Stajyere mentor atamak için: Interns.MentorId = bir çalışanın (Employees) Id değeri olmalı.
-- Önce çalışanların Id'lerini görmek için: SELECT Id, FirstName, LastName, Email FROM Employees WHERE IsDeleted = 0;
-- Aşağıda stajyer@hepiyi.com için MentorId = 1 atanıyor; gerçek çalışan Id'niz ile değiştirin.

DECLARE @StajyerEmail NVARCHAR(256) = N'stajyer@hepiyi.com';
DECLARE @MentorEmployeeId INT = 1;  -- Employees tablosundaki mentor çalışanın Id'si

UPDATE Interns
SET MentorId = @MentorEmployeeId
WHERE Email = @StajyerEmail;

IF @@ROWCOUNT > 0
    PRINT 'Mentor atandı. Stajyer panelinde "Mentor Bilgisi" kutusu artık dolu görünmeli.';
ELSE
    PRINT 'Bu e-posta ile stajyer bulunamadı veya güncelleme yapılmadı.';
