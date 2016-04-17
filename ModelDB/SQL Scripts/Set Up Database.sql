/* Creat Database*/

CREATE DATABASE ModelDb
GO

/*add Tables*/

use ModelDb
-- 
-- Creating a table dbo.Priorities 
-- 
CREATE TABLE dbo.Priorities (
   Id BIGINT NOT NULL IDENTITY,
   Name VARCHAR(8000) NOT NULL,
   Description VARCHAR(8000),
   CONSTRAINT PK_Priorities PRIMARY KEY (Id)
)
GO

-- 
-- Creating a table dbo.Baselines 
-- 
CREATE TABLE dbo.Baselines (
   Id BIGINT NOT NULL IDENTITY,
   ImpactLow BIT NOT NULL,
   ImpactModerate BIT NOT NULL,
   ImpactHigh BIT NOT NULL,
   Description VARCHAR(8000) NOT NULL,
   CONSTRAINT PK_Baselines PRIMARY KEY (Id)
)
GO

-- 
-- Creating a table dbo.MapTypes 
-- 
CREATE TABLE dbo.MapTypes (
   Id BIGINT NOT NULL IDENTITY,
   Name VARCHAR(8000) NOT NULL,
   Category BIGINT NOT NULL,
   CategoryOrder BIGINT NOT NULL,
   CONSTRAINT PK_MapTypes PRIMARY KEY (Id)
)
GO

-- 
-- Creating a table dbo.Families 
-- 
CREATE TABLE dbo.Families (
   Id BIGINT NOT NULL IDENTITY,
   Name VARCHAR(8000) NOT NULL,
   Description VARCHAR(8000),
   CONSTRAINT PK_Families PRIMARY KEY (Id)
)
GO

-- 
-- Creating a table dbo.Capabilities 
-- 
CREATE TABLE dbo.Capabilities (
   Id BIGINT NOT NULL IDENTITY,
   Domain VARCHAR(8000) NOT NULL,
   Container VARCHAR(8000) NOT NULL,
   Capability VARCHAR(8000) NOT NULL,
   Capability2 VARCHAR(8000),
   FamilyId BIGINT NOT NULL,
   UniqueId VARCHAR(8000) NOT NULL,
   Description VARCHAR(8000) NOT NULL,
   Notes VARCHAR(8000),
   Scopes VARCHAR(8000) NOT NULL,
   CONSTRAINT PK_Capabilities PRIMARY KEY (Id)
)
GO

-- 
-- Commenting a table dbo.Capabilities 
-- 
EXECUTE sp_addextendedproperty 'MS_Description', 'Teh primary key of the  capability', 'schema', 'dbo', 'table', 'Capabilities'
GO

-- 
-- Creating a table dbo.TICMappings 
-- 
CREATE TABLE dbo.TICMappings (
   Id BIGINT NOT NULL IDENTITY,
   CapabilityId BIGINT NOT NULL,
   TICName VARCHAR(8000) NOT NULL,
   CONSTRAINT PK_TICMappings PRIMARY KEY (Id),
   CONSTRAINT FK_TICMappings_Capabilities1 FOREIGN KEY (CapabilityId) REFERENCES dbo.Capabilities (Id)
)
GO

-- 
-- Creating a table dbo.Controls 
-- 
CREATE TABLE dbo.Controls (
   Id BIGINT NOT NULL IDENTITY,
   Name VARCHAR(8000) NOT NULL,
   BaselineID BIGINT NOT NULL,
   FamilyId BIGINT NOT NULL,
   PriorityId BIGINT NOT NULL,
   Description VARCHAR(8000) NOT NULL,
   Guidance VARCHAR(8000) NOT NULL,
   CONSTRAINT PK_Controls PRIMARY KEY (Id),
   CONSTRAINT FK_Controls_Families FOREIGN KEY (FamilyId) REFERENCES dbo.Families (Id),
   CONSTRAINT FK_Controls_Priorities FOREIGN KEY (PriorityId) REFERENCES dbo.Priorities (Id),
   CONSTRAINT FK_Controls_Baselines FOREIGN KEY (BaselineID) REFERENCES dbo.Baselines (Id)
)
GO

-- 
-- Creating a table dbo.Relateds 
-- 
CREATE TABLE dbo.Relateds (
   Id BIGINT NOT NULL IDENTITY,
   ParentId BIGINT NOT NULL,
   ChildId BIGINT NOT NULL,
   CONSTRAINT PK_Relateds PRIMARY KEY (Id),
   CONSTRAINT FK_Relateds_RelatedParent FOREIGN KEY (ParentId) REFERENCES dbo.Controls (Id),
   CONSTRAINT FK_Relateds_IsChild FOREIGN KEY (ChildId) REFERENCES dbo.Controls (Id)
)
GO

-- 
-- Creating a table dbo.Specs 
-- 
CREATE TABLE dbo.Specs (
   Id BIGINT NOT NULL IDENTITY,
   ControId BIGINT NOT NULL,
   SpecificationlName VARCHAR(8000) NOT NULL,
   Description VARCHAR(8000) NOT NULL,
   Guidance VARCHAR(8000) NOT NULL,
   CONSTRAINT PK_Specs PRIMARY KEY (Id),
   CONSTRAINT FK_Specs_Controls FOREIGN KEY (ControId) REFERENCES dbo.Controls (Id)
)
GO

-- 
-- Creating a table dbo.MapTypesCapabilitiesControls 
-- 
CREATE TABLE dbo.MapTypesCapabilitiesControls (
   Id BIGINT NOT NULL IDENTITY,
   CapabilitiesId BIGINT NOT NULL,
   ControlsId BIGINT NOT NULL,
   MapTypesId BIGINT NOT NULL,
   specId BIGINT NOT NULL,
   isControlMap BIT NOT NULL,
   CONSTRAINT PK_MapTypesCapabilitiesControls PRIMARY KEY (Id),
   CONSTRAINT FK_MapTypesCapabilitiesControls_CapabilitiesImplementation FOREIGN KEY (CapabilitiesId) REFERENCES dbo.Capabilities (Id) ON DELETE CASCADE,
   CONSTRAINT FK_MapTypesCapabilitiesControls_ControlsImplementation FOREIGN KEY (ControlsId) REFERENCES dbo.Controls (Id) ON DELETE CASCADE,
   CONSTRAINT FK_MapTypesCapabilitiesControls_MapTypes FOREIGN KEY (MapTypesId) REFERENCES dbo.MapTypes (Id) ON DELETE CASCADE,
   CONSTRAINT FK_MapTypesCapabilitiesControls_Specs FOREIGN KEY (specId) REFERENCES dbo.Specs (Id)
)
GO


use ModelDb

/* Intert Baselines */

Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (0,0,0,'NONE');
Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (0,0,1,'HIGH');
Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (0,1,0,'MODERATE');
Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (0,1,1,'MODERATE,HIGH');
Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (1,0,0,'LOW');
Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (1,0,1,'LOW,HIGH');
Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (1,1,0,'LOW,MODERATE');
Insert into Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description]) VALUES (1,1,1,'LOW,MODERATE,HIGH');

/* Isert Families */
insert into Families ([Name],[Description]) VALUES ('AC','ACCESS CONTROL');
insert into Families ([Name],[Description]) VALUES ('AT','AWARENESS AND TRAINING');
insert into Families ([Name],[Description]) VALUES ('AU','AUDIT AND ACCOUNTABILITY');
insert into Families ([Name],[Description]) VALUES ('CA','SECURITY ASSESSMENT AND AUTHORIZATION');
insert into Families ([Name],[Description]) VALUES ('CM','CONFIGURATION MANAGEMENT');
insert into Families ([Name],[Description]) VALUES ('CP','CONTINGENCY PLANNING');
insert into Families ([Name],[Description]) VALUES ('IA','IDENTIFICATION AND AUTHENTICATION');
insert into Families ([Name],[Description]) VALUES ('IR','INCIDENT RESPONSE');
insert into Families ([Name],[Description]) VALUES ('MA','MAINTENANCE');
insert into Families ([Name],[Description]) VALUES ('MP','MEDIA PROTECTION');
insert into Families ([Name],[Description]) VALUES ('PE','PHYSICAL AND ENVIRONMENTAL PROTECTION');
insert into Families ([Name],[Description]) VALUES ('PL','PLANNING');
insert into Families ([Name],[Description]) VALUES ('PS','PERSONNEL SECURITY');
insert into Families ([Name],[Description]) VALUES ('RA','RISK ASSESSMENT');
insert into Families ([Name],[Description]) VALUES ('SA','SYSTEM AND SERVICES ACQUISITION');
insert into Families ([Name],[Description]) VALUES ('SC','SYSTEM AND COMMUNICATIONS PROTECTION');
insert into Families ([Name],[Description]) VALUES ('SI','SYSTEM AND INFORMATION INTEGRITY');
insert into Families ([Name],[Description]) VALUES ('PM','PROGRAM MANAGEMENT');


/* iNSERT Magtypes*/
INSERT INTO [ModelDb].[dbo].[MapTypes] ([Name], [Category], [CategoryOrder])
VALUES ('Low (Capability Implementation)', 1, 1);
INSERT INTO [ModelDb].[dbo].[MapTypes] ([Name], [Category], [CategoryOrder])
VALUES ('Moderate (Capability Implementation)', 1, 2);
INSERT INTO [ModelDb].[dbo].[MapTypes] ([Name], [Category], [CategoryOrder])
VALUES ('High (Capability Implementation)', 1, 3);

INSERT INTO [ModelDb].[dbo].[MapTypes] ([Name], [Category], [CategoryOrder])
VALUES ('PM Controls', 2, 1);

INSERT INTO [ModelDb].[dbo].[MapTypes] ([Name], [Category], [CategoryOrder])
VALUES ('Low (Info Protection)', 3, 1);
INSERT INTO MapTypes ([Name], [Category], [CategoryOrder])
VALUES ('Moderate (Info Protection)', 3, 2);
INSERT INTO MapTypes ([Name], [Category], [CategoryOrder])
VALUES ('High (Info Protection)', 3, 3);

/* INSERT Priorities */
INSERT INTO Priorities ([Name],[Description]) VALUES ('P0','Priority 0');
INSERT INTO Priorities ([Name],[Description]) VALUES ('P1','Priority 1');
INSERT INTO Priorities ([Name],[Description]) VALUES ('P2','Priority 2');
INSERT INTO Priorities ([Name],[Description]) VALUES ('P3','Priority 3');
INSERT INTO Priorities ([Name],[Description]) VALUES ('NONE','No Priority');