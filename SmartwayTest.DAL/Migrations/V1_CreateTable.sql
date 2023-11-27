CREATE TABLE Companies (
    ID INT PRIMARY KEY Identity,
    Name VARCHAR(50) NOT NULL
);
CREATE TABLE Departments (
    ID INT PRIMARY KEY Identity,
    Name VARCHAR(50) NOT NULL,
    Phone VARCHAR(20),
    CompanyID INT,
    FOREIGN KEY (CompanyID) REFERENCES Companies(ID) ON DELETE CASCADE
);
CREATE TABLE Employees (
    ID INT PRIMARY KEY Identity,
    Name VARCHAR(50) NOT NULL,
    Surname VARCHAR(20) NOT NULL,
    Phone VARCHAR(20),
    DepartmentID INT,
	FOREIGN KEY (DepartmentId) REFERENCES Departments(Id) ON DELETE CASCADE
);
CREATE TABLE Passports (
	ID INT PRIMARY KEY Identity,
    Type VARCHAR(20) NOT NULL,
    Number VARCHAR(20) NOT NULL,
	EmployeeId INT not null,	
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
);