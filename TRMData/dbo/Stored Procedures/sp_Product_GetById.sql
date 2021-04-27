CREATE PROCEDURE [dbo].[sp_Product_GetById]
	@Id int = 0
AS
begin
	set nocount on

	SELECT [Id], [ProductName], [Description], [RetailPrice], [QuantityInStock], IsTaxable
	from dbo.Product
	where Id = @Id;
end
