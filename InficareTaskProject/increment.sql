--Permissions
CREATE TABLE Permissions (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) UNIQUE NOT NULL
);
--RolePermission
CREATE TABLE RolePermissions (
    RoleId NVARCHAR(450),
    PermissionId INT,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id), 
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id),
    PRIMARY KEY (RoleId, PermissionId)
);

--Insert Query for Permission
INSERT INTO Permissions (Name) VALUES
('CreateStudent'),
('AddStudent'),
('UpdateStudent'),
('DeleteStudent'),
('GetStudent'),
('CreateRole'),
('UpdateRole'),
('DeleteRole'),
('GetRole');


--Insert Query for role permission
INSERT INTO RolePermissions (RoleId, PermissionId)
VALUES
('171f8f6b-df63-4249-9f98-01fc6ca398d7', 1),
('171f8f6b-df63-4249-9f98-01fc6ca398d7', 2),
('171f8f6b-df63-4249-9f98-01fc6ca398d7', 3),
('171f8f6b-df63-4249-9f98-01fc6ca398d7', 4),
('171f8f6b-df63-4249-9f98-01fc6ca398d7', 5),
('7cd318d3-b146-4f69-9b77-7b71ef48e952', 5);



CREATE TABLE StudentSubjects (
    Id INT PRIMARY KEY IDENTITY,
    SubjectId INT NOT NULL,
    StudentId NVARCHAR(450) NOT NULL,
    CONSTRAINT FK_StudentSubjects_Subject FOREIGN KEY (SubjectId) REFERENCES Subjects(Id),
    CONSTRAINT FK_StudentSubjects_Student FOREIGN KEY (StudentId) REFERENCES AspNetRoles(Id)
);


CREATE TABLE Subjects (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(MAX)
);



--INSERT INTO Subjects (Name) VALUES
--    ('Mathematics'),
--    ('Physics'),
--    ('Biology'),
--    ('Chemistry'),
--    ('Computer Science'),
--    ('History'),
--    ('English'),
--    ('Geography');

CREATE TABLE StudentSubjects (
    StudentId NVARCHAR(450),
    SubjectId INT,
    FOREIGN KEY (StudentId) REFERENCES AspNetUsers(Id), 
    FOREIGN KEY (SubjectId) REFERENCES Subjects(Id),
    PRIMARY KEY (StudentId, SubjectId)
);

CREATE TABLE StudentSubjects (
    Id INT PRIMARY KEY IDENTITY,
    StudentId NVARCHAR(450),
    SubjectId INT,
    FOREIGN KEY (StudentId) REFERENCES AspNetUsers(Id), 
    FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
);