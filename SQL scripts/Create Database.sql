USE [master]
GO

/****** Object:  Database [UmbracoUserAdminTest]    Script Date: 16/04/2014 14:21:13 ******/
CREATE DATABASE [UmbracoUserAdminTest] 

ALTER DATABASE [UmbracoUserAdminTest] SET COMPATIBILITY_LEVEL = 90
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [UmbracoUserAdminTest].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [UmbracoUserAdminTest] SET ANSI_NULL_DEFAULT OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET ANSI_NULLS OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET ANSI_PADDING OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET ANSI_WARNINGS OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET ARITHABORT OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET AUTO_CLOSE OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET AUTO_CREATE_STATISTICS ON
GO

ALTER DATABASE [UmbracoUserAdminTest] SET AUTO_SHRINK OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET AUTO_UPDATE_STATISTICS ON
GO

ALTER DATABASE [UmbracoUserAdminTest] SET CURSOR_CLOSE_ON_COMMIT OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET CURSOR_DEFAULT  GLOBAL
GO

ALTER DATABASE [UmbracoUserAdminTest] SET CONCAT_NULL_YIELDS_NULL OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET NUMERIC_ROUNDABORT OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET QUOTED_IDENTIFIER OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET RECURSIVE_TRIGGERS OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET  DISABLE_BROKER
GO

ALTER DATABASE [UmbracoUserAdminTest] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET DATE_CORRELATION_OPTIMIZATION OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET TRUSTWORTHY OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET PARAMETERIZATION SIMPLE
GO

ALTER DATABASE [UmbracoUserAdminTest] SET READ_COMMITTED_SNAPSHOT OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET RECOVERY FULL
GO

ALTER DATABASE [UmbracoUserAdminTest] SET  MULTI_USER
GO

ALTER DATABASE [UmbracoUserAdminTest] SET PAGE_VERIFY CHECKSUM
GO

ALTER DATABASE [UmbracoUserAdminTest] SET DB_CHAINING OFF
GO

ALTER DATABASE [UmbracoUserAdminTest] SET  READ_WRITE
GO


