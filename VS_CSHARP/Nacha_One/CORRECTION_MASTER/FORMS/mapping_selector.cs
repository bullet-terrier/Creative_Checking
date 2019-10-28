using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


/// <summary>
/// 
/// We're going to try to connect to the duck creek database for this integration.
/// We'll want to get theaccount number, amount, payment id to generate a rough cut of the 
/// Entry amounts.
/// 
/// 
/// 
/// 
/// 
/// 
/// 
/// 
/// SELECT 
///     '6'[Record Number],
/// 	'27'[Transaction Code], -- Technically I could swap this to look at the
/// 
///     LEFT(paymentMethodDetail.query('/PaymentMethodDetail/EFT/BankRoutingNumber').value('.','VARCHAR(9)'),8)[Routing Number], -- Routing Number
///     RIGHT(paymentMethodDetail.query('/PaymentMethodDetail/EFT/BankRoutingNumber').value('.','VARCHAR(9)'),1)[Check Digit], -- Check Digit
///     paymentMethodDetail.query('/PaymentMethodDetail/EFT/AccountNumber').value('.','VARCHAR(10)')[Account Number], -- Account Number,
/// 	amount [Billed Amount], -- payment amount according to the payment entry
///     PaymentBatchEntryID [Payment Id], -- ID NUMBER- We'll be able to use these to block the payment batches...
/// 	paymentMethodDetail.query('/PaymentMethodDetail/EFT/BillToFirstName').value('.','VARCHAR(40)')+' '+paymentMethodDetail.query('/PaymentMethodDetail/EFT/BillToLastName').value('.','VARCHAR(40)')[Account Name], -- Bill name...
/// 	' '[Discretionary Data], -- Discretionary data
/// 	'0'[Addenda Indicator],  -- addenda indicator - should be zero
/// 	'000000000000000'[Trace Placeholder] -- Trace number.
/// 
/// FROM ExampleDataShared.dbo.DC_BIL_PaymentBatchEntry WHERE paymentBatchID = 1393-- This is what we're looking for...
/// ORDER BY amount asc
/// 
/// </summary>
namespace CORRECTION_MASTER.FORMS
{
    public partial class mapping_selector : Form
    {
        public mapping_selector()
        {
            InitializeComponent();
        }
    }
}
