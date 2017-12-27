

/*
  Re-create schema
 */

CREATE TABLE IF NOT EXISTS Families (
  Id          INTEGER PRIMARY KEY AUTOINCREMENT,
  Name        CHAR(8000) NOT NULL,
  Description CHAR(8000) NOT NULL
);

CREATE TABLE IF NOT EXISTS Priorities (
  Id          INTEGER PRIMARY KEY AUTOINCREMENT,
  Name        CHAR(8000) NOT NULL,
  Description CHAR(8000) NOT NULL
);

CREATE TABLE IF NOT EXISTS Baselines (
  Id             INTEGER PRIMARY KEY AUTOINCREMENT,
  ImpactLow      BOOLEAN    NOT NULL CHECK (ImpactLow IN (0, 1)),
  ImpactModerate BOOLEAN    NOT NULL CHECK (ImpactModerate IN (0, 1)),
  ImpactHigh     BOOLEAN    NOT NULL CHECK (ImpactHigh IN (0, 1)),
  Description    CHAR(8000) NOT NULL
);

CREATE TABLE IF NOT EXISTS Capabilities (
  Id                   INTEGER PRIMARY KEY AUTOINCREMENT,
  Domain               CHAR(8000) NOT NULL,
  Container            CHAR(8000) NOT NULL,
  Capability           CHAR(8000) NOT NULL,
  Capability2          CHAR(8000) NOT NULL,
  UniqueId             CHAR(8000) NOT NULL,
  Description          CHAR(8000) NOT NULL,
  CSADescription       CHAR(8000) NOT NULL,
  Notes                CHAR(8000) NOT NULL,
  Scopes               CHAR(8000) NOT NULL,
  C                    INTEGER     NOT NULL,
  I                    INTEGER     NOT NULL,
  A                    INTEGER     NOT NULL,
  ResponsibilityVector CHAR(8000) NOT NULL,
  OtherActors          CHAR(8000) NOT NULL
);

CREATE TABLE IF NOT EXISTS TICMappings (
  Id           INTEGER PRIMARY KEY AUTOINCREMENT,
  CapabilityId INTEGER     NOT NULL,
  TICName      CHAR(8000) NOT NULL,
  FOREIGN KEY (CapabilityID) REFERENCES Capabilities (Id)
);

CREATE TABLE IF NOT EXISTS Controls (
  Id          INTEGER PRIMARY KEY AUTOINCREMENT,
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

CREATE TABLE IF NOT EXISTS Relateds (
  Id       INTEGER PRIMARY KEY AUTOINCREMENT,
  ParentId INTEGER NOT NULL,
  ChildID  INTEGER NOT NULL,
  FOREIGN KEY (ParentId) REFERENCES Controls (Id),
  FOREIGN KEY (ChildID) REFERENCES Controls (Id)
);

CREATE TABLE IF NOT EXISTS Specs (
  Id               INTEGER PRIMARY KEY NOT NULL,
  ControlsId       INTEGER             NOT NULL,
  SecificationName Char(8000)         NOT NULL,
  Description      Char(8000)         NOT NULL,
  Guidance         Char(8000)         NOT NULL,
  FOREIGN KEY (ControlsId) REFERENCES Controls (Id)
);

CREATE TABLE IF NOT EXISTS BaselineSecurityMappings (
  Id             INTEGER PRIMARY KEY AUTOINCREMENT,
  Level          INTEGER  NOT NULL,
  BaselineAuthor INTEGER  NOT NULL,
  IsControlMap   BOOLEAN NOT NULL CHECK (IsControlMap IN (0, 1)),
  SpecsId        INTEGER  NOT NULL,
  ControlsId     INTEGER  NOT NULL,
  FOREIGN KEY (SpecsId) REFERENCES Specs (Id),
  FOREIGN KEY (ControlsId) REFERENCES Controls (Id)
);

CREATE TABLE IF NOT EXISTS MapTypesCapabilitiesControls (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  CapabilitiesId INTEGER  NOT NULL,
  ControlsId     INTEGER  NOT NULL,
  MapTypesId     INTEGER  NOT NULL,
  SpecsId        INTEGER  NOT NULL,
  IsControlMap   BOOLEAN NOT NULL CHECK (IsControlMap IN (0, 1)),
  FOREIGN KEY (CapabilitiesId) REFERENCES Capabilities (Id),
  FOREIGN KEY (ControlsId) REFERENCES Controls (Id),
  FOREIGN KEY (SpecsId) REFERENCES Specs (Id)
);