DROP TABLE IF EXISTS DogInfoSchema.DogInfo;

CREATE TABLE DogInfoSchema.DogInfo
(
    DogId INT IDENTITY(1, 1) PRIMARY KEY
    , Name NVARCHAR(50)
    , LifeSpan NVARCHAR(20)
    , Weight NVARCHAR(20)
    , PictureUrl NVARCHAR(MAX)
    , Size NVARCHAR(20)
    , CountryId INT
);

DROP TABLE IF EXISTS DogInfoSchema.Countries;

CREATE TABLE DogInfoSchema.Countries
(
    CountryId INT
    , Name NVARCHAR(100)
);

