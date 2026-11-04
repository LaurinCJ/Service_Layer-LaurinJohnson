USE [MinorProject5_GameLibrary];
GO
SET XACT_ABORT ON;

-- Seed only a completely empty set of tables. Never erase existing work.
IF EXISTS (SELECT 1 FROM dbo.Franchises) OR EXISTS (SELECT 1 FROM dbo.Platforms) OR EXISTS (SELECT 1 FROM dbo.Games)
BEGIN
    PRINT 'Seed skipped: at least one table already contains records.';
END
ELSE
BEGIN
    BEGIN TRANSACTION;
    INSERT INTO dbo.Franchises (Name, Developer) VALUES
        (N'Super Mario', N'Nintendo'),
        (N'The Legend of Zelda', N'Nintendo'),
        (N'Pokemon', N'Game Freak'),
        (N'Metroid', N'Nintendo'),
        (N'Halo', N'Bungie'),
        (N'Forza', N'Playground Games'),
        (N'Final Fantasy', N'Square Enix'),
        (N'Stardew Valley', N'ConcernedApe');
    INSERT INTO dbo.Platforms (Name, Manufacturer) VALUES
        (N'Nintendo Switch', N'Nintendo'),
        (N'Nintendo 3DS', N'Nintendo'),
        (N'Game Boy Advance', N'Nintendo'),
        (N'Xbox Series X', N'Microsoft'),
        (N'PlayStation 5', N'Sony'),
        (N'PC', N'Various');
    INSERT INTO dbo.Games (Title, Notes, ReleaseYear, FranchiseId, PlatformId)
    SELECT seed.Title, seed.Notes, seed.ReleaseYear, f.Id, p.Id
    FROM (VALUES
        (N'Super Mario Odyssey', N'3D platforming adventure', 2017, N'Super Mario', N'Nintendo Switch'),
        (N'Super Mario Bros. Wonder', N'Side scrolling platforming', 2023, N'Super Mario', N'Nintendo Switch'),
        (N'Mario Kart 8 Deluxe', N'Kart racing with friends', 2017, N'Super Mario', N'Nintendo Switch'),
        (N'Super Mario 3D World + Bowser''s Fury', N'Cooperative platforming adventure', 2021, N'Super Mario', N'Nintendo Switch'),
        (N'The Legend of Zelda: Breath of the Wild', N'Open world exploration', 2017, N'The Legend of Zelda', N'Nintendo Switch'),
        (N'The Legend of Zelda: Tears of the Kingdom', N'Building and open world exploration', 2023, N'The Legend of Zelda', N'Nintendo Switch'),
        (N'The Legend of Zelda: Link''s Awakening', N'Top down island adventure', 2019, N'The Legend of Zelda', N'Nintendo Switch'),
        (N'The Legend of Zelda: A Link Between Worlds', N'Top down puzzle adventure', 2013, N'The Legend of Zelda', N'Nintendo 3DS'),
        (N'Pokemon FireRed', N'Turn based monster collecting', 2004, N'Pokemon', N'Game Boy Advance'),
        (N'Pokemon Emerald', N'Turn based adventure in Hoenn', 2004, N'Pokemon', N'Game Boy Advance'),
        (N'Pokemon X', N'Turn based adventure in Kalos', 2013, N'Pokemon', N'Nintendo 3DS'),
        (N'Pokemon Legends: Arceus', N'Monster collecting and exploration', 2022, N'Pokemon', N'Nintendo Switch'),
        (N'Metroid Dread', N'Side scrolling exploration', 2021, N'Metroid', N'Nintendo Switch'),
        (N'Metroid Prime Remastered', N'First person exploration', 2023, N'Metroid', N'Nintendo Switch'),
        (N'Halo Infinite', N'First person science fiction action', 2021, N'Halo', N'Xbox Series X'),
        (N'Halo: The Master Chief Collection', N'Collection of first person campaigns', 2014, N'Halo', N'PC'),
        (N'Forza Horizon 5', N'Open world racing', 2021, N'Forza', N'Xbox Series X'),
        (N'Final Fantasy VII Rebirth', N'Action role playing adventure', 2024, N'Final Fantasy', N'PlayStation 5'),
        (N'Final Fantasy XIV', N'Online role playing adventure', 2013, N'Final Fantasy', N'PC'),
        (N'Stardew Valley', N'Farming and village life', 2016, N'Stardew Valley', N'PC')
    ) AS seed(Title, Notes, ReleaseYear, FranchiseName, PlatformName)
    INNER JOIN dbo.Franchises f ON f.Name = seed.FranchiseName
    INNER JOIN dbo.Platforms p ON p.Name = seed.PlatformName;
    COMMIT TRANSACTION;
END;
GO
SELECT 'Franchises' AS TableName, COUNT(*) AS RecordCount FROM dbo.Franchises
UNION ALL SELECT 'Platforms', COUNT(*) FROM dbo.Platforms
UNION ALL SELECT 'Games', COUNT(*) FROM dbo.Games;
GO
