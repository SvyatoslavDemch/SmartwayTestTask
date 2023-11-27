
INSERT INTO Companies (Name) VALUES ('CompanyA');
INSERT INTO Companies (Name) VALUES ('CompanyB');

INSERT INTO Departments (Name, Phone, CompanyID) VALUES ('DepartmentA', '123-456-7890', 1);
INSERT INTO Departments (Name, Phone, CompanyID) VALUES ('DepartmentB', '987-654-3210', 2);

INSERT INTO Employees (Name, Surname, Phone, DepartmentID) VALUES ('John', 'Doe', '111-222-3333', 1);
INSERT INTO Employees (Name, Surname, Phone, DepartmentID) VALUES ('Jane', 'Smith', '444-555-6666', 2);

INSERT INTO Passports (Type, Number, EmployeeId) VALUES ('Passport', 'ABC123', 1);
INSERT INTO Passports (Type, Number, EmployeeId) VALUES ('Driver License', 'XYZ789', 2);
	