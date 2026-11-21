CREATE DATABASE ConstructionManagementDB;
GO

USE ConstructionManagementDB;
GO

-- 1. Users: Builder, Supervisor, Contractor, Admin
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(20) NOT NULL UNIQUE,
    Email NVARCHAR(150) NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(30) NOT NULL
        CHECK (Role IN ('Admin','Builder','Supervisor','Contractor')),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 2. Construction sites
CREATE TABLE ConstructionSites (
    SiteId INT IDENTITY(1,1) PRIMARY KEY,
    BuilderId INT NOT NULL,
    SiteName NVARCHAR(150) NOT NULL,
    Address NVARCHAR(500) NULL,
    Latitude DECIMAL(9,6) NULL,
    Longitude DECIMAL(9,6) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    FOREIGN KEY (BuilderId) REFERENCES Users(UserId)
);
GO

-- 3. Site access for supervisors and other authorized users
CREATE TABLE SiteUserAccess (
    SiteId INT NOT NULL,
    UserId INT NOT NULL,
    AssignedBy INT NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PRIMARY KEY (SiteId, UserId),
    FOREIGN KEY (SiteId) REFERENCES ConstructionSites(SiteId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (AssignedBy) REFERENCES Users(UserId)
);
GO

-- 4. Contractor details
CREATE TABLE Contractors (
    ContractorId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    ContractorCompany NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

-- 5. Daily attendance and work submissions
CREATE TABLE DailyWorkSubmissions (
    SubmissionId BIGINT IDENTITY(1,1) PRIMARY KEY,
    SiteId INT NOT NULL,
    ContractorId INT NOT NULL,
    WorkDate DATE NOT NULL,
    AttendanceSelfiePath NVARCHAR(500) NOT NULL,
    WorkPhotoPath NVARCHAR(500) NOT NULL,
    DeclaredLabourCount INT NOT NULL
        CHECK (DeclaredLabourCount > 0),
    VerifiedLabourCount INT NULL
        CHECK (VerifiedLabourCount IS NULL OR VerifiedLabourCount >= 0),
    WorkDescription NVARCHAR(1000) NULL,
    Latitude DECIMAL(9,6) NULL,
    Longitude DECIMAL(9,6) NULL,
    SubmittedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Status NVARCHAR(40) NOT NULL DEFAULT 'PendingSupervisor'
        CHECK (Status IN (
            'PendingSupervisor',
            'PendingBuilder',
            'Approved',
            'Rejected'
        )),
    UNIQUE (SiteId, ContractorId, WorkDate),
    FOREIGN KEY (SiteId) REFERENCES ConstructionSites(SiteId),
    FOREIGN KEY (ContractorId) REFERENCES Contractors(ContractorId)
);
GO

-- 6. Approval history
CREATE TABLE WorkApprovals (
    ApprovalId BIGINT IDENTITY(1,1) PRIMARY KEY,
    SubmissionId BIGINT NOT NULL,
    ApproverId INT NOT NULL,
    ApprovalLevel INT NOT NULL
        CHECK (ApprovalLevel IN (1,2)),
    Decision NVARCHAR(20) NOT NULL
        CHECK (Decision IN ('Approved','Rejected')),
    Remarks NVARCHAR(1000) NULL,
    DecisionAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    FOREIGN KEY (SubmissionId)
        REFERENCES DailyWorkSubmissions(SubmissionId),
    FOREIGN KEY (ApproverId) REFERENCES Users(UserId),
    UNIQUE (SubmissionId, ApprovalLevel)
);
GO

-- 7. Material demands
CREATE TABLE MaterialDemands (
    DemandId BIGINT IDENTITY(1,1) PRIMARY KEY,
    SiteId INT NOT NULL,
    RequestedBy INT NOT NULL,
    DemandDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    RequiredDate DATE NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Pending'
        CHECK (Status IN (
            'Pending','Approved','Rejected',
            'PartiallyDelivered','Completed'
        )),
    Remarks NVARCHAR(1000) NULL,
    FOREIGN KEY (SiteId) REFERENCES ConstructionSites(SiteId),
    FOREIGN KEY (RequestedBy) REFERENCES Users(UserId)
);
GO

-- 8. Individual materials in a demand
CREATE TABLE MaterialDemandItems (
    DemandItemId BIGINT IDENTITY(1,1) PRIMARY KEY,
    DemandId BIGINT NOT NULL,
    MaterialName NVARCHAR(150) NOT NULL,
    RequestedQuantity DECIMAL(12,2) NOT NULL
        CHECK (RequestedQuantity > 0),
    Unit NVARCHAR(30) NOT NULL,
    ApprovedQuantity DECIMAL(12,2) NULL,
    UnitRate DECIMAL(12,2) NULL,
    FOREIGN KEY (DemandId) REFERENCES MaterialDemands(DemandId)
);
GO

-- 9. Material delivery and challan records
CREATE TABLE MaterialDeliveries (
    DeliveryId BIGINT IDENTITY(1,1) PRIMARY KEY,
    DemandItemId BIGINT NOT NULL,
    ReceivedBy INT NOT NULL,
    ReceivedQuantity DECIMAL(12,2) NOT NULL
        CHECK (ReceivedQuantity > 0),
    ChallanNumber NVARCHAR(100) NULL,
    ChallanPhotoPath NVARCHAR(500) NOT NULL,
    InvoicePhotoPath NVARCHAR(500) NULL,
    InvoiceNumber NVARCHAR(100) NULL,
    DeliveryDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UnitRate DECIMAL(12,2) NULL,
    TotalAmount AS (
        CASE WHEN UnitRate IS NOT NULL
        THEN ReceivedQuantity * UnitRate
        ELSE NULL END
    ) PERSISTED,
    Remarks NVARCHAR(1000) NULL,
    FOREIGN KEY (DemandItemId)
        REFERENCES MaterialDemandItems(DemandItemId),
    FOREIGN KEY (ReceivedBy) REFERENCES Users(UserId)
);
GO

CREATE INDEX IX_DailyWork_SiteDate
ON DailyWorkSubmissions(SiteId, WorkDate);

CREATE INDEX IX_Demands_SiteStatus
ON MaterialDemands(SiteId, Status);

CREATE INDEX IX_Approvals_Submission
ON WorkApprovals(SubmissionId, ApprovalLevel);
GO
