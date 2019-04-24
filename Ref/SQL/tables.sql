CREATE TABLE Users(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Email] nvarchar(128) NOT NULL,
	[PasswordHash] binary(64) NOT NULL,
	[PasswordSalt] binary(128) NOT NULL,
	[Role] nvarchar(32) NOT NULL,
	[Subscription] int NOT NULL DEFAULT(100),
	[RegisteredAt] datetime NOT NULL,
	[Guid] varchar(255) NOT NULL,
	[IsActive] bit NOT NULL DEFAULT(1)
)

CREATE TABLE Cities(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(256) NOT NULL,
	[NameRaw] nvarchar(256) NOT NULL,
	[GtCodeSale] nvarchar(128) NOT NULL,
	[GtCodeRent] nvarchar(128) NOT NULL,
	[HasDistricts] bit NOT NULL DEFAULT(0)
)

CREATE TABLE Districts(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CityId] [int] REFERENCES Cities(Id),
	[Name] nvarchar(256) NOT NULL,
	[NameRaw] nvarchar(256) NOT NULL,
	[GtCodeSale] nvarchar(128) NOT NULL,
	[GtCodeRent] nvarchar(128) NOT NULL,
	[OtoDomId] int NULL,
	[OlxId] int NULL,
	[AdrId] varchar(10) null
)

CREATE TABLE Filters(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserId] [int] REFERENCES Users(Id),
	[CityId] [int] REFERENCES Cities(Id),
	[DistrictId] [int] REFERENCES Districts(Id) NULL,
	[Property] [int] NOT NULL,
	[Deal] [int] NOT NULL,
	[Market] [int] NOT NULL,
	[FlatAreaFrom] [int] NULL,
	[FlatAreaTo] [int] NULL,
	[PriceTo] [int] NULL,
	[PriceFrom] [int] NULL,
	[PricePerMeterFrom] [int] NULL,
	[PricePerMeterTo] [int] NULL,
	[Name] nvarchar(128) NULL,
	[Notification] [int] NOT NULL DEFAULT(100),
	[LastCheckedAt] datetime NULL,
	[ShouldContain] nvarchar(128) NULL,
	[ShouldNotContain] nvarchar(128) NULL
)

CREATE TABLE Offers(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CityId] [int] REFERENCES Cities(Id),
	[DistrictId] [int] REFERENCES Districts(Id) NULL,
	[SiteOfferId] nvarchar(128) NOT NULL,
	[Site] [int] NOT NULL,
	[Deal] int NOT NULL,
	[Url] nvarchar(255) NOT NULL,
	[Header] nvarchar(255) NOT NULL,
	[Price] [int] NOT NULL,
	[Area] [int] NOT NULL,
	[Rooms] [int] NULL,
	[PricePerMeter] [int] NULL,
	[Floor] [int] NULL,
	[DateAdded] datetime NOT NULL,
	[IsScrapped] bit NOT NULL DEFAULT(0) WITH VALUES,
	[Content] varchar(max) NULL
)

CREATE TABLE OfferFilters(
	[OfferId] [int] REFERENCES Offers(Id),
	[FilterId] [int] REFERENCES Filters(Id),
	[Sent] [bit] NOT NULL
)

CREATE TABLE AdminInfos(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Text] nvarchar(256) NOT NULL,
	[DateAdded] datetime NOT NULL,
	[IsActive] [bit] NOT NULL
)

CREATE TABLE Sites(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[Name] nvarchar(32) NOT NULL,
	[IsActive] [bit] NOT NULL DEFAULT(0),
)

CREATE TABLE Events(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[Category] [int] NOT NULL,
	[Message] nvarchar(256) NOT NULL,
	[UpdatedAt] datetime NOT NULL
)