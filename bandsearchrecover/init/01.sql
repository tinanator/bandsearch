CREATE TABLE "Bands" (
    "Id" serial NOT NULL,
    "Name" text NOT NULL,
    "OwnerId" integer NOT NULL,
    CONSTRAINT "PK_Bands" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" serial NOT NULL,
    "Name" text NOT NULL,
    "Surname" text NOT NULL,
    "Age" integer NOT NULL,
    "Gender" integer NOT NULL,
    "Country" text NULL,
    "City" text NULL,
    "PhotoUrl" text NULL,
    "About" text NULL,
    "IsLookingForBand" boolean NULL,
    "BandOpenPositionCriteriaInfo" text NULL,
    "Email" text NOT NULL,
    "Password" text NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "BandOpenPositions" (
    "Id" serial NOT NULL,
    "BandId" integer NOT NULL,
    "AgeMin" integer NULL,
    "AgeMax" integer NULL,
    "Gender" integer NULL,
    "Country" text NULL,
    "City" text NULL,
    CONSTRAINT "PK_BandOpenPositions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_BandOpenPositions_Bands_BandId" FOREIGN KEY ("BandId") REFERENCES "Bands" ("Id") ON DELETE CASCADE
);

CREATE TABLE "BandUser" (
    "BandsId" integer NOT NULL,
    "MembersId" integer NOT NULL,
    CONSTRAINT "PK_BandUser" PRIMARY KEY ("BandsId", "MembersId"),
    CONSTRAINT "FK_BandUser_Bands_BandsId" FOREIGN KEY ("BandsId") REFERENCES "Bands" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BandUser_Users_MembersId" FOREIGN KEY ("MembersId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "InstrumentsLevel" (
    "Id" serial NOT NULL,
    "BandOpenPositionId" integer NULL,
    "UserId" integer NULL,
    "Instrument" text NOT NULL,
    "Level" integer NOT NULL,
    CONSTRAINT "PK_InstrumentsLevel" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_InstrumentsLevel_BandOpenPositions_BandOpenPositionId" FOREIGN KEY ("BandOpenPositionId") REFERENCES "BandOpenPositions" ("Id"),
    CONSTRAINT "FK_InstrumentsLevel_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id")
);

CREATE INDEX "IX_BandOpenPositions_BandId" ON "BandOpenPositions" ("BandId");

CREATE INDEX "IX_BandUser_MembersId" ON "BandUser" ("MembersId");

CREATE UNIQUE INDEX "IX_InstrumentsLevel_BandOpenPositionId" ON "InstrumentsLevel" ("BandOpenPositionId");

CREATE INDEX "IX_InstrumentsLevel_UserId" ON "InstrumentsLevel" ("UserId");
