CREATE TABLE Users(
[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
[Email] nvarchar(128) NOT NULL,
[PasswordHash] binary(64) NOT NULL,
[PasswordSalt] binary(128) NOT NULL
)