USE [MinorProject5_GameLibrary];
GO
SELECT 'Franchises' AS TableName, COUNT(*) AS RecordCount FROM dbo.Franchises
UNION ALL SELECT 'Platforms', COUNT(*) FROM dbo.Platforms
UNION ALL SELECT 'Games', COUNT(*) FROM dbo.Games;
SELECT g.Id, g.Title, g.Notes, g.ReleaseYear, f.Name AS Franchise, p.Name AS Platform
FROM dbo.Games g
INNER JOIN dbo.Franchises f ON f.Id = g.FranchiseId
INNER JOIN dbo.Platforms p ON p.Id = g.PlatformId
ORDER BY g.Title;
GO
