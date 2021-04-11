CREATE TABLE [dbo].[User]
(	
    [Id] NvarCHAR(128) NOT NULL primary key, 
    [FirstName] NvarCHAR(50) NOT NULL, 
    [LastName] NvarCHAR(50) NOT NULL, 
    [EmailAddress] NvarCHAR(256) NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate()
)
