﻿CREATE PROCEDURE [dbo].[sp_Sale_Insert]
	@Id int output,
	@CashierId nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS

begin
	set nocount on

	insert into dbo.Sale(CashierId, SalesDate, SubTotal, Tax, Total)
	values (@CashierId, @SaleDate, @SubTotal, @Tax, @Total)

	select @Id = SCOPE_IDENTITY();

end