/*
  As far as I can tell, Families, Priorities and Baselines are NOT defined by any of the excel files and must be set
  up manually in the database beforehand in order for controls to work
 */
DROP TABLE IF EXISTS Families;
CREATE TABLE Families (
  Id          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Name        CHAR(8000) NOT NULL,
  Description CHAR(8000) NOT NULL
);

DROP TABLE IF EXISTS Priorities;
CREATE TABLE Priorities (
  Id          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Name        CHAR(8000) NOT NULL,
  Description CHAR(8000) NOT NULL
);

DROP TABLE IF EXISTS Baselines;
CREATE TABLE Baselines (
  Id             INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  ImpactLow      BOOLEAN    NOT NULL CHECK (ImpactLow IN (0, 1)),
  ImpactModerate BOOLEAN    NOT NULL CHECK (ImpactModerate IN (0, 1)),
  ImpactHigh     BOOLEAN    NOT NULL CHECK (ImpactHigh IN (0, 1)),
  Description    CHAR(8000) NOT NULL
);

DROP TABLE IF EXISTS Controls;
CREATE TABLE Controls (
  Id          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Name        CHAR(8000) NOT NULL,
  BaselineId  INTEGER     NOT NULL,
  FamilyId    INTEGER     NOT NULL,
  PriorityId  INTEGER     NOT NULL,
  Description CHAR(8000) NOT NULL,
  Guidance    CHAR(8000) NOT NULL,
  FOREIGN KEY (BaselineId) REFERENCES Baselines (Id),
  FOREIGN KEY (FamilyID) REFERENCES Families (Id),
  FOREIGN KEY (PriorityId) REFERENCES Priorities (Id)
);

DROP TABLE IF EXISTS Relateds;
CREATE TABLE Relateds (
  Id       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  ParentId INTEGER NOT NULL,
  ChildID  INTEGER NOT NULL,
  FOREIGN KEY (ParentId) REFERENCES Controls (Id),
  FOREIGN KEY (ChildID) REFERENCES Controls (Id)
);

DROP TABLE IF EXISTS Specs;
CREATE TABLE Specs (
  Id               INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  ControlsId       INTEGER             NOT NULL,
  SpecificationName Char(8000)         NOT NULL,
  Description      Char(8000)         NOT NULL,
  Guidance         Char(8000)         NOT NULL,
  FOREIGN KEY (ControlsId) REFERENCES Controls (Id)
);

/*
  Defined in CC-Overlay-SRA Capabilities
 */
DROP TABLE IF EXISTS Capabilities;
CREATE TABLE Capabilities (
  Id                   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Domain               CHAR(8000) NOT NULL,
  Container            CHAR(8000) NOT NULL,
  Capability           CHAR(8000) NOT NULL,
  Capability2          CHAR(8000) NOT NULL,
  UniqueId             CHAR(8000) NOT NULL,
  Description          CHAR(8000) NOT NULL,
  CSADescription       CHAR(8000),
  Notes                CHAR(8000),
  Scopes               CHAR(8000) NOT NULL,
  C                    INTEGER     NOT NULL,
  I                    INTEGER     NOT NULL,
  A                    INTEGER     NOT NULL,
  ResponsibilityVector CHAR(8000) NOT NULL,
  OtherActors          CHAR(8000) NOT NULL
);

/*
  Maps capability to tic-style control. Defined in TIC Capabilities Mapping (H) column of CC-Overlay-SRA Capabilities
 */
DROP TABLE IF EXISTS TICMappings;
CREATE TABLE TICMappings (
  Id           INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  CapabilityId INTEGER     NOT NULL,
  TICName      CHAR(8000) NOT NULL,
  FOREIGN KEY (CapabilityID) REFERENCES Capabilities (Id)
);

/*
  Defined in Baselines_NIST_vs_FedRAMP sheet
 */
DROP TABLE IF EXISTS BaselineSecurityMappings;
CREATE TABLE BaselineSecurityMappings (
  Id             INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Level          INTEGER  NOT NULL,
  BaselineAuthor INTEGER  NOT NULL,
  IsControlMap   BOOLEAN NOT NULL CHECK (IsControlMap IN (0, 1)),
  SpecsId        INTEGER  NOT NULL,
  ControlsId     INTEGER  NOT NULL,
  FOREIGN KEY (SpecsId) REFERENCES Specs (Id),
  FOREIGN KEY (ControlsId) REFERENCES Controls (Id)
);

/*
  Defined in CC-Overlay-SRA Capabilities
 */
DROP TABLE IF EXISTS MapTypesCapabilitiesControls;
CREATE TABLE MapTypesCapabilitiesControls (
  Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  CapabilitiesId INTEGER  NOT NULL,
  ControlsId     INTEGER  NOT NULL,
  MapTypesId     INTEGER  NOT NULL,
  SpecsId        INTEGER  NOT NULL,
  IsControlMap   BOOLEAN NOT NULL CHECK (IsControlMap IN (0, 1)),
  FOREIGN KEY (CapabilitiesId) REFERENCES Capabilities (Id),
  FOREIGN KEY (ControlsId) REFERENCES Controls (Id),
  FOREIGN KEY (SpecsId) REFERENCES Specs (Id)
);