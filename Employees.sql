
CREATE DATABASE Employee-time-tracking;
GO


USE Employee-time-tracking;
GO


CREATE TABLE dbo.Employees (
      EmployeeNo INT NOT NULL PRIMARY KEY,
      FirstName VARCHAR(50) NOT NULL,
      LastName VARCHAR(50) NOT NULL,
      Username VARCHAR(50) NOT NULL,
      PasswordHash VARCHAR(255) NOT NULL,
      CellPhoneNumber VARCHAR(15) NULL,
      Position VARCHAR(50) NULL,
      IsDisabled BIT NULL,
      CreatedAt DATETIME NULL,
      UpdatedAt DATETIME NULL,
      IsManager BIT NULL,
      ManagerId INT NULL,
      IsFirstLogin BIT NOT NULL
);
GO
