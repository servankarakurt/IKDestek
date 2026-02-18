CREATE TABLE Employeess(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    CardID INT NOT NULL,          
    Department INT NOT NULL,       
    BirthDate DATE NOT NULL,
    StartDate DATE NOT NULL,
    ROLES INT NOT NULL,          
    Createdate DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0
);