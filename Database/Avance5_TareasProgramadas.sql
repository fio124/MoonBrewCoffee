USE MoonBrewCoffee;
GO

IF OBJECT_ID('dbo.ScheduledTaskExecutions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ScheduledTaskExecutions
    (
        IdScheduledTaskExecution INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_ScheduledTaskExecutions PRIMARY KEY,
        TaskName NVARCHAR(80) NOT NULL,
        ExecutionKey NVARCHAR(120) NOT NULL,
        StartedAtUtc DATETIME2 NOT NULL,
        CompletedAtUtc DATETIME2 NULL,
        Status NVARCHAR(20) NOT NULL,
        Details NVARCHAR(500) NULL
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.ScheduledTaskExecutions')
      AND name = 'UX_ScheduledTaskExecutions_Task_Key'
)
BEGIN
    CREATE UNIQUE INDEX UX_ScheduledTaskExecutions_Task_Key
        ON dbo.ScheduledTaskExecutions(TaskName, ExecutionKey);
END;
GO

SELECT TOP (20)
    TaskName,
    ExecutionKey,
    StartedAtUtc,
    CompletedAtUtc,
    Status,
    Details
FROM dbo.ScheduledTaskExecutions
ORDER BY IdScheduledTaskExecution DESC;
GO
