USE [Hotels]
GO

/****** Object:  Table [dbo].[Rooms]    Script Date: 24/04/2024 5:46:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Rooms](
	[IdBD] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[NumberOfPeople] [int] NOT NULL,
	[BaseCost] [int] NOT NULL,
	[ImposedCost] [int] NOT NULL,
	[TypeOfRoom] [nvarchar](100) NOT NULL,
	[Location] [nvarchar](200) NOT NULL,
	[Observation] [nvarchar](300) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[IdHotel] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[IdBD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Rooms] ADD  DEFAULT ((1)) FOR [NumberOfPeople]
GO

ALTER TABLE [dbo].[Rooms] ADD  DEFAULT ((1)) FOR [BaseCost]
GO

ALTER TABLE [dbo].[Rooms] ADD  DEFAULT ((1)) FOR [ImposedCost]
GO

ALTER TABLE [dbo].[Rooms] ADD  DEFAULT ((1)) FOR [IsEnabled]
GO

ALTER TABLE [dbo].[Rooms]  WITH CHECK ADD FOREIGN KEY([IdHotel])
REFERENCES [dbo].[Hotels] ([IdHotel])
GO


USE [Hotels]
GO

/****** Object:  Table [dbo].[Reserves]    Script Date: 24/04/2024 5:46:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Reserves](
	[IdReserve] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,
	[RoomIdBD] [int] NOT NULL,
	[DateOfAdmission] [datetime] NOT NULL,
	[DateOfExit] [datetime] NOT NULL,
	[DateOfCreation] [datetime] NOT NULL,
	[Birthdate] [datetime] NOT NULL,
	[FullName] [nvarchar](200) NOT NULL,
	[Gender] [nvarchar](50) NOT NULL,
	[DocumentType] [nvarchar](50) NOT NULL,
	[DocumentNumber] [nvarchar](50) NOT NULL,
	[Telephone] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](200) NOT NULL,
	[EmergencyFullName] [nvarchar](200) NULL,
	[EmergencyTelephone] [nvarchar](50) NULL,
	[Observation] [nvarchar](300) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Reserves]  WITH CHECK ADD FOREIGN KEY([RoomIdBD])
REFERENCES [dbo].[Rooms] ([IdBD])
GO


USE [Hotels]
GO

/****** Object:  Table [dbo].[Hotels]    Script Date: 24/04/2024 5:46:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Hotels](
	[IdHotel] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdHotel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Hotels] ADD  DEFAULT ((1)) FOR [IsEnabled]
GO


USE [Hotels]
GO

/****** Object:  Table [dbo].[AppUsers]    Script Date: 24/04/2024 5:46:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppUsers](
	[IdBD] [int] IDENTITY(1,1) NOT NULL,
	[Id] [nvarchar](100) NOT NULL,
	[Names] [nvarchar](200) NOT NULL,
	[LastName] [nvarchar](200) NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](200) NOT NULL
) ON [PRIMARY]
GO


USE [Hotels]
GO

/****** Object:  Table [dbo].[AppConfigurations]    Script Date: 24/04/2024 5:45:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppConfigurations](
	[IdBD] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](350) NOT NULL,
	[Module] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](50) NULL
) ON [PRIMARY]
GO


