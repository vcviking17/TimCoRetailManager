CREATE PROCEDURE [dbo].[spUserLookup]
	@ID nvarchar(128)
AS
begin
	set nocount on;

	SELECT Id, FirstName, LastName, EmailAddress, CreatedDate
	from [dbo].[User]
	where Id = @ID;
end