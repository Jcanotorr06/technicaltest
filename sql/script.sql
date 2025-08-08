IF NOT EXISTS (
  SELECT * FROM sys.tables t
  JOIN sys.schemas s ON (t.schema_id = s.schema_id)
  WHERE s.name = 'dbo' AND t.name = 'TaskStatus'
)
BEGIN
  CREATE TABLE TaskStatus (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL
  )

  INSERT INTO [TaskManagement].[dbo].[TaskStatus] (StatusName, Description) VALUES
  ('Pending', 'Task is pending'),
  ('In Progress', 'Task is currently being worked on'),
  ('Done', 'Task has been completed')

END

IF NOT EXISTS (
  SELECT * FROM sys.tables t
  JOIN sys.schemas s ON (t.schema_id = s.schema_id)
  WHERE s.name = 'dbo' AND t.name = 'Tasks'
)
BEGIN
  CREATE TABLE Tasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    DueDate DATETIME NULL,
    [Status] INT NOT NULL,
    CreatedBy NVARCHAR(100) NOT NULL,
    AssignedTo NVARCHAR(100) NULL,

    FOREIGN KEY ([Status]) REFERENCES [TaskManagement].[dbo].[TaskStatus](Id)
    ON DELETE CASCADE
    ON UPDATE CASCADE
  );
END

