CREATE PROCEDURE [dbo].[sp_Inventory_GetAll]
AS

begin
	set nocount on;

	select [ProductId], [Quantity], [PurchasePrice], [PurchaseDate] 
	from dbo.Inventory;
end
