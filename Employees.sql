CREATE DATABASE Employee-time-tracking;
 

USE [Employee-time-tracking]
GO

/****** Object:  Table [dbo].[Employees]    Script Date: 2025/01/21 15:01:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employees](
    [EmployeeNo] [int] IDENTITY(1,1) NOT NULL,
    [FirstName] [varchar](50) NOT NULL,
    [LastName] [varchar](50) NOT NULL,
    [Username] [varchar](50) NOT NULL,
    [PasswordHash] [varchar](255) NOT NULL,
    [CellPhoneNumber] [varchar](15) NULL,
    [Position] [varchar](50) NULL,
    [IsDisabled] [bit] NULL,
    [CreatedAt] [datetime] NULL,
    [UpdatedAt] [datetime] NULL,
    [IsManager] [bit] NULL,
    [ManagerId] [int] NULL,
    [IsFirstLogin] [bit] NOT NULL,
    [IsClockedIn] [bit] NULL,
    PRIMARY KEY CLUSTERED
(
[EmployeeNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
    UNIQUE NONCLUSTERED
(
[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

ALTER TABLE [dbo].[Employees] ADD  DEFAULT ((0)) FOR [IsDisabled]
    GO

ALTER TABLE [dbo].[Employees] ADD  DEFAULT (getdate()) FOR [CreatedAt]
    GO

ALTER TABLE [dbo].[Employees] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
    GO

ALTER TABLE [dbo].[Employees] ADD  DEFAULT ((1)) FOR [IsFirstLogin]
    GO
