--Baselines
use ModelDb;
insert into ModelDb.dbo.Baselines ([ImpactLow],[ImpactModerate],[ImpactHigh],[Description])
select ImpactLow, ImpactModerate, ImpactHigh, Description from Excel2DB.dbo.Baselines;

-- FAMILIES
use ModelDb;
insert into ModelDb.dbo.Families ([Name],[Description])
select [Name],[Description] from Excel2DB.dbo.Families;


-- PRIORITIES
use ModelDb;
insert into [ModelDb].[dbo].[Priorities] ([Name],[Description])
select [Name],[Description] from [Excel2DB].[dbo].[Priorities];


-- MapTypes Initialization
USE [ModelDb]
-- Insert Capability Implementation
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
INSERT INTO [ModelDb].[dbo].[MapTypes] ([Name], [Category], [CategoryOrder])
VALUES ('Moderate (Info Protection)', 3, 2);
INSERT INTO [ModelDb].[dbo].[MapTypes] ([Name], [Category], [CategoryOrder])
VALUES ('High (Info Protection)', 3, 3);

select * from [ModelDb].[dbo].[MapTypes];
