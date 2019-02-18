CREATE TABLE Users(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Email] nvarchar(128) NOT NULL,
	[PasswordHash] binary(64) NOT NULL,
	[PasswordSalt] binary(128) NOT NULL,
	[Role] nvarchar(32) NOT NULL
)

CREATE TABLE Filters(
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[UserId] [int] REFERENCES Users(Id),
	[PropertyType] [int] NOT NULL,
	[DealType] [int] NOT NULL,
	[MarketType] [int] NOT NULL,
	[Location] nvarchar(255) NOT NULL,
	[FlatAreaFrom] [int] NOT NULL,
	[FlatAreaTo] [int] NOT NULL,
	[PriceTo] [int] NOT NULL,
	[PriceFrom] [int] NOT NULL,
	[Newest] [bit] NULL,
	[Name] nvarchar(128) NULL
)