-- =============================================
-- Script de Creación de Base de Datos NPS
-- SQL Server 2019+
-- =============================================

-- Crear Base de Datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NPSApplicationDB')
BEGIN
    CREATE DATABASE NPSApplicationDB;
END
GO

USE NPSApplicationDB;
GO

-- =============================================
-- Tabla: Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role INT NOT NULL, -- 1=Voter, 2=Admin
        IsLocked BIT NOT NULL DEFAULT 0,
        FailedLoginAttempts INT NOT NULL DEFAULT 0,
        LastLoginAttempt DATETIME2 NULL,
        LastSuccessfulLogin DATETIME2 NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE INDEX IX_Users_Username ON Users(Username);
    CREATE INDEX IX_Users_Role ON Users(Role);
END
GO

-- =============================================
-- Tabla: Votes
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Votes')
BEGIN
    CREATE TABLE Votes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Score INT NOT NULL CHECK (Score >= 0 AND Score <= 10),
        Category INT NOT NULL, -- 0=Detractor, 1=Neutral, 2=Promoter
        VotedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Votes_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT UQ_Votes_UserId UNIQUE (UserId)
    );

    CREATE INDEX IX_Votes_UserId ON Votes(UserId);
    CREATE INDEX IX_Votes_Category ON Votes(Category);
END
GO

-- =============================================
-- Tabla: RefreshTokens
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE RefreshTokens (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Token NVARCHAR(500) NOT NULL UNIQUE,
        ExpiresAt DATETIME2 NOT NULL,
        IsRevoked BIT NOT NULL DEFAULT 0,
        RevokedAt DATETIME2 NULL,
        ReplacedByToken NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );

    CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
    CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
END
GO

-- =============================================
-- Stored Procedures
-- =============================================

-- SP: Autenticar Usuario
CREATE OR ALTER PROCEDURE sp_AuthenticateUser
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Username,
        PasswordHash,
        Role,
        IsLocked,
        FailedLoginAttempts,
        LastLoginAttempt,
        IsActive
    FROM Users
    WHERE Username = @Username AND IsActive = 1;
END
GO

-- SP: Registrar Intento de Login Fallido
CREATE OR ALTER PROCEDURE sp_RegisterFailedLogin
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Users
    SET 
        FailedLoginAttempts = FailedLoginAttempts + 1,
        LastLoginAttempt = GETUTCDATE(),
        IsLocked = CASE WHEN FailedLoginAttempts + 1 >= 3 THEN 1 ELSE 0 END,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @UserId;
END
GO

-- SP: Registrar Login Exitoso
CREATE OR ALTER PROCEDURE sp_RegisterSuccessfulLogin
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Users
    SET 
        FailedLoginAttempts = 0,
        LastSuccessfulLogin = GETUTCDATE(),
        LastLoginAttempt = GETUTCDATE(),
        UpdatedAt = GETUTCDATE()
    WHERE Id = @UserId;
END
GO

-- SP: Crear Voto
CREATE OR ALTER PROCEDURE sp_CreateVote
    @UserId INT,
    @Score INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Category INT;
    
    -- Calcular categoría según el score
    SET @Category = CASE 
        WHEN @Score >= 9 THEN 2  -- Promoter
        WHEN @Score >= 7 THEN 1  -- Neutral
        ELSE 0                    -- Detractor
    END;
    
    INSERT INTO Votes (UserId, Score, Category, VotedAt, CreatedAt)
    VALUES (@UserId, @Score, @Category, GETUTCDATE(), GETUTCDATE());
    
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- SP: Verificar si Usuario ya votó
CREATE OR ALTER PROCEDURE sp_HasUserVoted
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS HasVoted
    FROM Votes
    WHERE UserId = @UserId AND IsActive = 1;
END
GO

-- SP: Obtener Resultados NPS
CREATE OR ALTER PROCEDURE sp_GetNPSResults
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalVotes INT;
    DECLARE @Promoters INT;
    DECLARE @Neutrals INT;
    DECLARE @Detractors INT;
    DECLARE @NPS DECIMAL(5, 2);
    
    SELECT @TotalVotes = COUNT(*)
    FROM Votes
    WHERE IsActive = 1;
    
    SELECT @Promoters = COUNT(*)
    FROM Votes
    WHERE Category = 2 AND IsActive = 1;
    
    SELECT @Neutrals = COUNT(*)
    FROM Votes
    WHERE Category = 1 AND IsActive = 1;
    
    SELECT @Detractors = COUNT(*)
    FROM Votes
    WHERE Category = 0 AND IsActive = 1;
    
    -- Calcular NPS
    IF @TotalVotes > 0
    BEGIN
        SET @NPS = (CAST(@Promoters AS DECIMAL) - CAST(@Detractors AS DECIMAL)) / CAST(@TotalVotes AS DECIMAL) * 100;
    END
    ELSE
    BEGIN
        SET @NPS = 0;
    END
    
    SELECT 
        @TotalVotes AS TotalVotes,
        @Promoters AS Promoters,
        @Neutrals AS Neutrals,
        @Detractors AS Detractors,
        @NPS AS NPSScore;
END
GO

-- SP: Guardar Refresh Token
CREATE OR ALTER PROCEDURE sp_SaveRefreshToken
    @UserId INT,
    @Token NVARCHAR(500),
    @ExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, CreatedAt)
    VALUES (@UserId, @Token, @ExpiresAt, GETUTCDATE());
    
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- SP: Obtener Refresh Token
CREATE OR ALTER PROCEDURE sp_GetRefreshToken
    @Token NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Token,
        ExpiresAt,
        IsRevoked,
        RevokedAt,
        ReplacedByToken,
        IsActive,
        CreatedAt
    FROM RefreshTokens
    WHERE Token = @Token AND IsActive = 1;
END
GO

-- SP: Revocar Refresh Token
CREATE OR ALTER PROCEDURE sp_RevokeRefreshToken
    @Token NVARCHAR(500),
    @ReplacedByToken NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE RefreshTokens
    SET 
        IsRevoked = 1,
        RevokedAt = GETUTCDATE(),
        ReplacedByToken = @ReplacedByToken,
        UpdatedAt = GETUTCDATE()
    WHERE Token = @Token;
END
GO

-- =============================================
-- Datos Iniciales
-- =============================================

-- Insertar Usuario Administrador (contraseña: Admin123!)
-- Hash generado con BCrypt
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
    VALUES ('admin', '$2a$11$3eFwe1QE2fNgUmXP3EwRjeLdN4R2QXuZbTZF2Sd6xlgvhXT3yORAS', 2, GETUTCDATE());
    -- Nota: Reemplazar con hash real generado por BCrypt
END
GO

-- Insertar Usuarios Votantes de Prueba (contraseña: Voter123!)
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'voter1')
BEGIN
    INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
    VALUES 
        ('voter1', '$2a$11$Vd1JMdWjwwVbNs3eK68JZOHvsQtNMBdpwAqoZJToYfyrtESH4vVsS', 1, GETUTCDATE());
    -- Nota: Reemplazar con hash real generado por BCrypt
END
GO

PRINT 'Base de datos creada exitosamente';
GO