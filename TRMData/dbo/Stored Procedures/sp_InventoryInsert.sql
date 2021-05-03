CREATE PROCEDURE [dbo].[sp_InventoryInsert]
	@ProductId int,
	@Quantity int,
	@PurchasePrice money,
	@PurchaseDate datetime2  --do we want to pass it or have system insert date
AS
begin 
	set nocount on;

	insert into dbo.Inventory(ProductId, Quantity, PurchasePrice, PurchaseDate)
	values(@ProductId, @Quantity, @PurchasePrice, @PurchaseDate)
end
