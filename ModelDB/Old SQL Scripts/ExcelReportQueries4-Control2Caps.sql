select 
	fam.Description
	,fam.Name
	,Replace(Replace(ctrl.Name,fam.Name, ' '),'-',' ')as Num4Name
	,ctrl.Name
	,caps.Domain
	,caps.Container
	,caps.Capability
	,caps.Capability2
	,caps.ScopesId
	,(mtyp.Id)
from 
	Controls as ctrl
inner join 
	Families as fam
	on ctrl.FamilyId=fam.Id
inner join 
	MapTypesCapabilitiesControls as mt 
	on ctrl.Id=mt.ControlsId
inner join 
	Capabilities as caps
	on caps.Id=mt.CapabilitiesId
inner join 
	MapTypes as mtyp
	on mtyp.Id=mt.MapTypesId
where mtyp.Id<=3
order by fam.Name, Num4Name
--order by ctrl.Name
select * from families
	