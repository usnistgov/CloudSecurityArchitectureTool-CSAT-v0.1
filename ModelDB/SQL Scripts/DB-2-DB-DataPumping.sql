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






--Old Cleanup
use ModelDb;
select [ImpactLow],[ImpactModerate],[ImpactHigh],[Description] into ModelDb.dbo.tmpBaselines from Excel2DB.dbo.Baselines;


select * from ModelDb.dbo.Baselines
DELETE FROM ModelDb.dbo.Baselines WHERE ID>5
