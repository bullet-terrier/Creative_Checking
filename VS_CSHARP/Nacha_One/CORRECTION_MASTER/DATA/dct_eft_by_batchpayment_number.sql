/*
    Benjamin Tiernan
	2019-09-18

	template eft utility - query from an eft containing database.
*/

DECLARE @xx XML;

DECLARE @paymentBatchID INT--        @drawDate DATE = '2019-09-15'


SET @paymentBatchID = (SELECT TOP 1 PaymentBatchID FROM EFTTransmissionControl WHERE DATEDIFF(DAY, DrawDate,@drawDate) = 0)

-- I might eventually want to enter these into a table for lulz.
SELECT 
    '6'[Record Number],
	'27'[Transaction Code], -- Technically I could swap this to look at the 
	LEFT(paymentMethodDetail.query('/PaymentMethodDetail/EFT/BankRoutingNumber').value('.','VARCHAR(9)'),8)[Routing Number], -- Routing Number
    RIGHT(paymentMethodDetail.query('/PaymentMethodDetail/EFT/BankRoutingNumber').value('.','VARCHAR(9)'),1)[Check Digit], -- Check Digit
	paymentMethodDetail.query('/PaymentMethodDetail/EFT/AccountNumber').value('.','VARCHAR(10)')[Account Number], -- Account Number,
	amount[Billed Amount], -- payment amount according to the payment entry
	PaymentBatchEntryID[Payment Id], -- ID NUMBER- We'll be able to use these to block the payment batches...
	paymentMethodDetail.query('/PaymentMethodDetail/EFT/BillToFirstName').value('.','VARCHAR(40)')+' '+paymentMethodDetail.query('/PaymentMethodDetail/EFT/BillToLastName').value('.','VARCHAR(40)')[Account Name], -- Bill name...
	' '[Discretionary Data], -- Discretionary data
	'0'[Addenda Indicator],  -- addenda indicator - should be zero
	'000000000000000'[Trace Placeholder], -- Trace number.
	(Select TOP 1 AccountReference FROM Account WHERE AccountID = pbe.accountid)[Account]

FROM PaymentBatchEntry pbe WHERE paymentBatchID = @paymentBatchID --1393-- This is what we're looking for...
ORDER BY [Account] asc