CREATE PROCEDURE [dbo].[sp_Sale_SaleReport]
AS

begin
	set nocount on;

	select [s].[SalesDate], [s].[SubTotal], [s].[Tax], [s].[Total], u.FirstName, u.LastName, u.EmailAddress
	from dbo.Sale as s
	inner join dbo.[User] as u on s.CashierId = u.Id
end
