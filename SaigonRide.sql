USE [SaigonRideDB];
GO

DELETE FROM RentingHistories;
DBCC CHECKIDENT ('RentingHistories', RESEED, 0);

DELETE FROM Vehicles;
DBCC CHECKIDENT ('Vehicles', RESEED, 0);

DELETE FROM Stations;
DBCC CHECKIDENT ('Stations', RESEED, 0);

DELETE FROM Users WHERE Role = 'User';

SELECT Id, StationName, Location, Capacity 
FROM Stations;

SELECT v.Id, v.VehicleName, v.Type, v.Status, s.StationName
FROM Vehicles v
LEFT JOIN Stations s ON v.StationId = s.Id;

SELECT Id, FullName, Email, Role 
FROM Users;

SELECT h.Id, u.FullName, v.VehicleName, h.StartTime, h.EndTime, h.TotalPrice, h.Status
FROM RentingHistories h
JOIN Users u ON h.UserId = u.Id
JOIN Vehicles v ON h.VehicleId = v.Id;

CREATE TABLE SupportReports (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    VehicleId INT NULL FOREIGN KEY REFERENCES Vehicles(Id), -- Có thể báo cáo về 1 xe cụ thể
    Message NVARCHAR(MAX) NOT NULL,
    AdminReply NVARCHAR(MAX) NULL, -- Chỗ để Admin trả lời
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Resolved
    CreatedAt DATETIME DEFAULT GETDATE()
);

DROP TABLE SupportReports;

SELECT TOP 100 * 
FROM SupportReports 
ORDER BY CreatedAt DESC;

UPDATE Users 
SET Role = 'Admin' 
WHERE Email = 'gp.admin@gmail.com';