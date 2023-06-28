INSERT INTO "Bands"("Id", "Name", "OwnerId")
VALUES
    (1, "Band1", 1),
    (2, "Band2", 2),
    (3, "Band3", 3),
    (4, "Band4", 4)

INSERT INTO "Users"("Id", "Name", "Surname", "Age", "Gender", "Country", 
    "City", "PhotoUrl", 
    "About", "IsLookingForBand",
    "BandOpenPositionCriteriaInfo",
    "Email",
    "Password")
VALUES
    (1, 'User1', 'Surname1', 20, 0, 'Russia', 'Moscow', null, null, null, 'email1@gmail.com', 'qwerty'),
    (2, 'User2', 'Surname2', 20, 1, 'Russia', 'Moscow', null, null, null, 'email2@gmail.com', 'qwerty'),
    (3, 'User3', 'Surname3', 20, 0, 'Russia', 'Moscow', null, null, null, 'email3@gmail.com', 'qwerty'),
    (4, 'User4', 'Surname4', 20, 1, 'Russia', 'Moscow', null, null, null, 'email4@gmail.com', 'qwerty'),
    (5, 'User5', 'Surname5', 20, 0, 'Russia', 'Moscow', null, null, null, 'email5@gmail.com', 'qwerty'),
    (6, 'User6', 'Surname6', 20, 1, 'Russia', 'Moscow', null, null, null, 'email6@gmail.com', 'qwerty') 


INSERT INTO "BandOpenPositions"("Id",
    "BandId",
    "AgeMin",
    "AgeMax",
    "Gender",
    "Country",
    "City")
VALUES
    (1, 1, 0, 20, 0, 'Russia', 'Moscow'),
    (2, 1, 0, 20, 0, 'Russia', 'Moscow'),
    (3, 2, 0, 20, 0, 'Russia', 'Moscow'),
    (4, 3, 0, 20, 0, 'Russia', 'Moscow'),
    (5, 4, 0, 20, 0, 'Russia', 'Moscow')

INSERT INTO "BandUser"("BandsId",
    "MembersId")
VALUES
    (1, 1),
    (2, 2),
    (3, 3),
    (4, 4),
    (1, 5),
    (2, 6),
    (3, 1)

INSERT INTO "InstrumentsLevel"("Id",
    "BandOpenPositionId", "UserId", "Instrument",
    "Level")
VALUES
    (1, null, 1, "guitar", 0),
    (2, null, 1, "drums", 1),
    (3, null, 2, "guitar", 2),
    (4, null, 3, "violin", 3),
    (5, 1, null, "guitar", 1),
    (6, 2, null, "drums", 3),
    (7, 3, null, "bass", 2)