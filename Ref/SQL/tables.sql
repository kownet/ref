CREATE TABLE Users(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Email] nvarchar(128) NOT NULL,
	[PasswordHash] binary(64) NOT NULL,
	[PasswordSalt] binary(128) NOT NULL,
	[Role] nvarchar(32) NOT NULL,
	[Subscription] int NOT NULL DEFAULT(100),
	[RegisteredAt] datetime NOT NULL
)

CREATE TABLE Filters(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserId] [int] REFERENCES Users(Id),
	[CityId] [int] REFERENCES Cities(Id),
	[Property] [int] NOT NULL,
	[Deal] [int] NOT NULL,
	[Market] [int] NOT NULL,
	[FlatAreaFrom] [int] NOT NULL,
	[FlatAreaTo] [int] NOT NULL,
	[PriceTo] [int] NOT NULL,
	[PriceFrom] [int] NOT NULL,
	[Name] nvarchar(128) NULL,
	[Notification] [int] NOT NULL DEFAULT(100),
	[LastCheckedAt] datetime NULL
)

CREATE TABLE Cities(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(256) NOT NULL,
	[NameRaw] nvarchar(256) NOT NULL,
	[GtCodeSale] nvarchar(128) NOT NULL,
	[GtCodeRent] nvarchar(128) NOT NULL
)

CREATE TABLE Offers(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[CityId] [int] REFERENCES Cities(Id),
	[SiteOfferId] nvarchar(128) NOT NULL,
	[SiteType] [int] NOT NULL,
	[DealType] int NOT NULL,
	[Url] nvarchar(255) NOT NULL,
	[Header] nvarchar(255) NOT NULL,
	[Price] [int] NOT NULL,
	[DateAdded] datetime NOT NULL
)

CREATE TABLE OfferFilters(
	[OfferId] [int] REFERENCES Offers(Id),
	[FilterId] [int] REFERENCES Filters(Id),
	[Sent] [bit] NOT NULL
)