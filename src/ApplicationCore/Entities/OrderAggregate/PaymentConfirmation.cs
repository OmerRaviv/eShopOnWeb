using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate
{
    public class PaymentConfirmation //value Object
    {
        public DateTime PaymentDate { get; private set; }

        public string PaymentType { get; private set; }

        public string PaymentID { get; private set; }

        private PaymentConfirmation()
        {
            // required by EF
        }

        public PaymentConfirmation(DateTime paymentDate, string paymentType, string paymentID)
        {
            PaymentDate = paymentDate;
            PaymentType = paymentType;
            PaymentID = paymentID;
        }
    }
}
