USE [employee-time-tracking]
GO

/****** Object:  Table [dbo].[Employees]    Script Date: 2025/02/10 08:38:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employees](
    [EmployeeNo] [int] IDENTITY(1,1) NOT NULL,
    [FirstName] [nvarchar](max) NOT NULL,
    [LastName] [nvarchar](max) NOT NULL,
    [Username] [nvarchar](max) NOT NULL,
    [CellPhoneNumber] [nvarchar](max) NOT NULL,
    [Position] [nvarchar](max) NOT NULL,
    [IsManager] [bit] NULL,
    [IsDisabled] [bit] NOT NULL,
    [PasswordHash] [nvarchar](max) NOT NULL,
    [ManagerId] [int] NULL,
    [IsClockedIn] [bit] NULL,
    [IsFirstLogin] [bit] NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED
(
[EmployeeNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    GO


