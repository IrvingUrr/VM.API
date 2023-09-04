USE master
GO
IF NOT EXISTS (
   SELECT name
   FROM sys.databases
   WHERE name = N'VirtualMind'
)
CREATE DATABASE [VirtualMind]
GO