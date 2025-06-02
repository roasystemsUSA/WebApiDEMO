use YourDBName;
CREATE TABLE AspNetRoles(
	Id nvarchar(256) NOT NULL,
	ConcurrencyStamp TEXT NULL,
	Name nvarchar(256) NULL,
	NormalizedName nvarchar(256) NULL,
	CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
);

CREATE TABLE AspNetUserClaims(
	Id int NOT NULL AUTO_INCREMENT,
	ClaimType TEXT NULL,
	ClaimValue TEXT NULL,
	UserId nvarchar(256) NOT NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id)
);

CREATE TABLE AspNetUserLogins(
	LoginProvider nvarchar(256) NOT NULL,
	ProviderKey nvarchar(256) NOT NULL,
	ProviderDisplayName TEXT NULL,
	UserId nvarchar(256) NOT NULL,
	CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (`LoginProvider`,`ProviderKey`)
);

CREATE TABLE AspNetUserRoles(
	UserId nvarchar(256) NOT NULL,
	RoleId nvarchar(256) NOT NULL,
	CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (`UserId`,`RoleId`)
);

CREATE TABLE AspNetUsers(
	Id nvarchar(256) NOT NULL,
	AccessFailedCount int NOT NULL,
	ConcurrencyStamp TEXT NULL,
	Email nvarchar(256) NULL,
	EmailConfirmed TINYINT(1) NOT NULL,
	LockoutEnabled TINYINT(1) NOT NULL,
	LockoutEnd TIMESTAMP NULL,
	NormalizedEmail nvarchar(256) NULL,
	NormalizedUserName nvarchar(256) NULL,
	PasswordHash TEXT NULL,
	PhoneNumber nvarchar(12) NULL,
	PhoneNumberConfirmed TINYINT(1) NOT NULL,
	SecurityStamp TEXT NULL,
	TwoFactorEnabled TINYINT(1) NOT NULL,
	UserName nvarchar(256) NULL,
	CONSTRAINT PK_AspNetUsers PRIMARY KEY (`Id`) 
);

CREATE TABLE VerificationCodes (
	Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	ApplicationId INT NOT NULL,
	PhoneNumber VARCHAR(20) NULL,
    Email nvarchar(256) NULL,
	Code VARCHAR(10),
	CreationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	ExpiresOn DATETIME NOT NULL
);

-- ALTER TABLE VerificationCodes ADD   CONSTRAINT `FK_PhoneNumber_AspNetUsers` FOREIGN KEY (`Email`) REFERENCES `AspNetUsers` (`Email`);

CREATE TABLE AspNetUserTokens(
	UserId nvarchar(256) NOT NULL,
	LoginProvider nvarchar(256) NOT NULL,
	Name nvarchar(450) NOT NULL,
	Value TEXT NULL,
	CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (`UserId`,`LoginProvider`,`Name`) 
); 

CREATE TABLE AspNetRoleClaims(
	Id nvarchar(256) NOT NULL,
	RoleId nvarchar(256) NOT NULL,
	ClaimType TEXT NULL,
	ClaimValue TEXT NULL,
	CONSTRAINT `FK_RoleId_AspNetRoleClaims` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`),
	CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (`Id`) 
); 

ALTER TABLE AspNetUserTokens ADD CONSTRAINT `FK_LoginProvider_AspNetUserTokens` FOREIGN KEY (`LoginProvider`) REFERENCES `aspnetuserlogins` (`LoginProvider`);
ALTER TABLE AspNetUserTokens ADD   CONSTRAINT `FK_UserId_AspNetUserTokens` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`);
ALTER TABLE AspNetUserClaims ADD   CONSTRAINT `FK_UserId_AspNetUserClaims` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`);
ALTER TABLE AspNetUserLogins ADD   CONSTRAINT `FK_UserId_AspNetUserLogins` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`);
ALTER TABLE AspNetUserRoles ADD   CONSTRAINT `FK_UserId_AspNetUserRoles` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`);
ALTER TABLE AspNetUserRoles ADD   CONSTRAINT `FK_RoleId_AspNetUserRoles` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`);

CREATE TABLE Person(
			Id int NOT NULL AUTO_INCREMENT,
			FirstName nvarchar(128) NOT NULL,
			MiddleName nvarchar(128) NULL,
			LastName nvarchar(128) NOT NULL,
			LastName2 nvarchar(128) NULL,
			ProfilePhoto binary NULL,
			Gender nvarchar(1) NOT NULL,
			DateOfBirth DATETIME NULL,
			CREATED_BY nvarchar(256) NOT NULL,
			CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
			LAST_UPDATED_BY nvarchar(256) NULL,
			LAST_UPDATE_DATE TIMESTAMP NULL,
			DELETED TINYINT(1) NOT NULL DEFAULT 0,
			DELETED_BY nvarchar(256) NULL,
			DELETED_DATE TIMESTAMP NULL,
			CONSTRAINT PK_Persons PRIMARY KEY (`Id`) 
); 

CREATE TABLE Language(
			LanguageCode nvarchar(8) NOT NULL,
			Name nvarchar(512) NOT NULL,
			Description Text NULL,
			CREATED_BY nvarchar(256) NOT NULL,
			CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
			LAST_UPDATED_BY nvarchar(256) NULL,
			LAST_UPDATE_DATE TIMESTAMP NULL,
			DELETED TINYINT(1) NOT NULL DEFAULT 0,
			DELETED_BY nvarchar(256) NULL,
			DELETED_DATE TIMESTAMP NULL,
			CONSTRAINT PK_Languages PRIMARY KEY (`LanguageCode`)
); 

CREATE TABLE AspNetUserProfile(
	UserId nvarchar(256) NOT NULL,
	PersonId INT NOT NULL,
	LanguageCode nvarchar(8) NOT NULL,
	Name nvarchar(255) NOT NULL,
	Description nvarchar(1024),
	CONSTRAINT `FK_UserId_AspNetUserProfile` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`),
	CONSTRAINT `FK_PersonId_AspNetUserProfile` FOREIGN KEY (`PersonId`) REFERENCES `person` (`Id`),
	CONSTRAINT `FK_LanguageCode_AspNetUserProfile` FOREIGN KEY (`LanguageCode`) REFERENCES `language` (`LanguageCode`),       
	CONSTRAINT PK_AspNetUserProfile PRIMARY KEY (`UserId`,`PersonId`) 
); 

CREATE TABLE ResourceType(
			Id nvarchar(256) NOT NULL,
			Name nvarchar(512) NOT NULL,
			Description Text NULL,
			CREATED_BY nvarchar(256) NOT NULL,
			CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
			LAST_UPDATED_BY nvarchar(256) NULL,
			LAST_UPDATE_DATE TIMESTAMP NULL,
			DELETED TINYINT(1) NOT NULL DEFAULT 0,
			DELETED_BY nvarchar(256) NULL,
			DELETED_DATE TIMESTAMP NULL,
			CONSTRAINT PK_ResourceTypes PRIMARY KEY (`Id`) 
); 

CREATE TABLE Application(
			Id int NOT NULL AUTO_INCREMENT,
			Name nvarchar(512) NOT NULL,
			Description Text NULL,
			Url nvarchar(512) NOT NULL,
			CREATED_BY nvarchar(256) NOT NULL,
			CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
			LAST_UPDATED_BY nvarchar(256) NULL,
			LAST_UPDATE_DATE TIMESTAMP NULL,
			DELETED TINYINT(1) NOT NULL DEFAULT 0,
			DELETED_BY nvarchar(256) NULL,
			DELETED_DATE TIMESTAMP NULL,
			CONSTRAINT PK_Applications PRIMARY KEY (`Id`)
); 
ALTER TABLE VerificationCodes ADD   CONSTRAINT `FK_ApplicationId_Application` FOREIGN KEY (`ApplicationId`) REFERENCES `Application` (`Id`);

CREATE TABLE UserPermissions(
			ApplicationId int NOT NULL,
			UserId nvarchar(256) NOT NULL,
			SEL TINYINT(1) NOT NULL DEFAULT 1,
			INS TINYINT(1) NOT NULL DEFAULT 0,
			UPD TINYINT(1) NOT NULL DEFAULT 0,
			DEL TINYINT(1) NOT NULL DEFAULT 0,
			UNDELETE TINYINT(1) NOT NULL DEFAULT 0,
			REPORT TINYINT(1) NOT NULL DEFAULT 0,
			PRINT TINYINT(1) NOT NULL DEFAULT 0,
			UPLOAD TINYINT(1) NOT NULL DEFAULT 0,
			DOWNLOAD TINYINT(1) NOT NULL DEFAULT 0,
			CREATED_BY nvarchar(256) NOT NULL,
			CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
			LAST_UPDATED_BY nvarchar(256) NULL,
			LAST_UPDATE_DATE TIMESTAMP NULL,
			DELETED TINYINT(1) NOT NULL DEFAULT 0,
			DELETED_BY nvarchar(256) NULL,
			DELETED_DATE TIMESTAMP NULL,
			CONSTRAINT `FK_ApplicationId_UserPermissions` FOREIGN KEY (`ApplicationId`) REFERENCES `application` (`Id`),
			CONSTRAINT `FK_UserId_UserPermissions` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`),
			CONSTRAINT PK_UserPermissions PRIMARY KEY (`ApplicationId`,`UserId`)
); 

CREATE TABLE RolePermissions(
			ApplicationId int NOT NULL,
			RoleId nvarchar(256) NOT NULL,
			SEL TINYINT(1) NOT NULL DEFAULT 1,
			INS TINYINT(1) NOT NULL DEFAULT 0,
			UPD TINYINT(1) NOT NULL DEFAULT 0,
			DEL TINYINT(1) NOT NULL DEFAULT 0,
			UNDELETE TINYINT(1) NOT NULL DEFAULT 0,
			REPORT TINYINT(1) NOT NULL DEFAULT 0,
			PRINT TINYINT(1) NOT NULL DEFAULT 0,
			UPLOAD TINYINT(1) NOT NULL DEFAULT 0,
			DOWNLOAD TINYINT(1) NOT NULL DEFAULT 0,
			CREATED_BY nvarchar(256) NOT NULL,
			CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
			LAST_UPDATED_BY nvarchar(256) NULL,
			LAST_UPDATE_DATE TIMESTAMP NULL,
			DELETED TINYINT(1) NOT NULL DEFAULT 0,
			DELETED_BY nvarchar(256) NULL,
			DELETED_DATE TIMESTAMP NULL,
			CONSTRAINT `FK_ApplicationId_RolePermissions` FOREIGN KEY (`ApplicationId`) REFERENCES `application` (`Id`),
			CONSTRAINT `FK_RoleId_RolePermissions` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`),
			CONSTRAINT PK_RolePermissions PRIMARY KEY (`ApplicationId`,`RoleId`)
); 

CREATE TABLE LocaleResourceString(
			LanguageCode nvarchar(256) NOT NULL,
            ApplicationId INT NOT NULL,
            ResourceTypeId nvarchar(256) NOT NULL,
            ResourceName nvarchar(256) NOT NULL,
            ResourceValue TEXT NOT NULL,
			CREATED_BY nvarchar(256) NOT NULL,
			CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
			LAST_UPDATED_BY nvarchar(256) NULL,
			LAST_UPDATE_DATE TIMESTAMP NULL,
			DELETED TINYINT(1) NOT NULL DEFAULT 0,
			DELETED_BY nvarchar(256) NULL,
			DELETED_DATE TIMESTAMP NULL,
			CONSTRAINT `FK_LanguageCode_LocaleResourceStrings` FOREIGN KEY (`LanguageCode`) REFERENCES `language` (`LanguageCode`),
            CONSTRAINT `FK_ApplicationId_LocaleResourceStrings` FOREIGN KEY (`ApplicationId`) REFERENCES `application` (`Id`),
            CONSTRAINT `FK_ResourceTypeId_LocaleResourceStrings` FOREIGN KEY (`ResourceTypeId`) REFERENCES `resourcetype` (`Id`),
			CONSTRAINT PK_LocaleResourceStrings PRIMARY KEY (`LanguageCode`,`ApplicationId`,`ResourceTypeId`)
); 

CREATE TABLE SubscriptionBenefitTypes
(
	Id nvarchar(256) NOT NULL,
	Name nvarchar(512) NOT NULL,
	Description Text NULL,
	Cost decimal NOT NULL DEFAULT 0, 
	CREATED_BY nvarchar(256) NOT NULL,
	CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
	LAST_UPDATED_BY nvarchar(256) NULL,
	LAST_UPDATE_DATE TIMESTAMP NULL,
	DELETED TINYINT(1) NOT NULL DEFAULT 0,
	DELETED_BY nvarchar(256) NULL,
	DELETED_DATE TIMESTAMP NULL,
    CONSTRAINT PK_SubscriptorBenefits PRIMARY KEY (`Id`) 
);

CREATE TABLE SubscriptionTypes (
UserSubscriptions	Id nvarchar(256) NOT NULL,
    ApplicationId INT NOT NULL, 
	Name nvarchar(512) NOT NULL,
	Description Text NULL,
	Cost decimal NOT NULL DEFAULT 0, 
    Tax decimal NOT NULL DEFAULT 0,
    OtherCosts decimal NOT NULL DEFAULT 0,
    OtherCostsDescription TEXT NULL,
    IsDemo tinyint(1) NOT NULL DEFAULT 1,
    Duration INT NULL DEFAULT 1, 
	CREATED_BY nvarchar(256) NOT NULL,
	CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
	LAST_UPDATED_BY nvarchar(256) NULL,
	LAST_UPDATE_DATE TIMESTAMP NULL,
	DELETED TINYINT(1) NOT NULL DEFAULT 0,
	DELETED_BY nvarchar(256) NULL,
	DELETED_DATE TIMESTAMP NULL,
	CONSTRAINT PK_Subscriptions PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ApplicationId_SubsTypes` FOREIGN KEY (`ApplicationId`) REFERENCES `Application` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
); 

CREATE TABLE SubscriptionTypeBenefits(
	SubscriptionTypeId nvarchar(256) NOT NULL,
	SubscriptionBenefitId nvarchar(256) NOT NULL,
    CONSTRAINT `FK_SubscriptionTypeId_SubsTB` FOREIGN KEY (`SubscriptionTypeId`) REFERENCES `SubscriptionTypes` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT `FK_SubscriptionBenefitId_SubsTB` FOREIGN KEY (`SubscriptionBenefitId`) REFERENCES `SubscriptionBenefitTypes` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT PK_SubscriptionTypeBenefits PRIMARY KEY (`SubscriptionTypeId`,`SubscriptionBenefitId`) 
); 

CREATE TABLE UserSubscriptions(
	Id nvarchar(256) NOT NULL,
    UserId nvarchar(256) NOT NULL,
    SubscriptionTypeId nvarchar(256) NOT NULL,
    Cost decimal NOT NULL DEFAULT 0,
    SubscriptionBegins TIMESTAMP NOT NULL DEFAULT NOW(),
    SubscriptionEnds TIMESTAMP NULL,
	CREATED_BY nvarchar(256) NOT NULL,
	CREATION_DATE TIMESTAMP NOT NULL DEFAULT NOW(),
	LAST_UPDATED_BY nvarchar(256) NULL,
	LAST_UPDATE_DATE TIMESTAMP NULL,
	DELETED TINYINT(1) NOT NULL DEFAULT 0,
	DELETED_BY nvarchar(256) NULL,
	DELETED_DATE TIMESTAMP NULL,
	CONSTRAINT PK_UserSubscriptions PRIMARY KEY (`Id`),
    CONSTRAINT `FK_UserId_UsrSubs` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT `FK_SubsTypeId_UsrSubs` FOREIGN KEY (`SubscriptionTypeId`) REFERENCES `SubscriptionTypes` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
); 

CREATE TABLE Logs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Message LONGTEXT,
    MessageTemplate LONGTEXT,
    Level VARCHAR(128),
    TimeStamp DATETIME,
    Exception LONGTEXT,
    Properties LONGTEXT,
    LogEvent VARCHAR(512)
);