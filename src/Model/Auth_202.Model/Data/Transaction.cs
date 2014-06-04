using System;
using Auth_202.Model.Constants;
using RESTServiceUtilities.Interfaces;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace Auth_202.Model.Data
{
    [Api("Insert update or delete a Transaction Resource")]
    [Route("/Transactions", "POST")]
    [Route("/Transactions/{Id}", "PUT")]
    [Route("/Transactions/{Id}", "DELETE")]
    public class Transaction : IReturn<Transaction>, IData
    {
        [AutoIncrement]
        public long Id { get; set; }
        
        public long SubscriptionId { get; set; }

        public string GatewayTransactionId { get; set; }

        [References(typeof (TransactionType))]
        public long TransactionTypeId { get; set; }

        [References(typeof (TransactionStatusType))]
        public long TransactionStatusId { get; set; }

        public Decimal Amount { get; set; }
        public string Card { get; set; }

        [Default((long) CURRENCY_TYPE.USDollar)]
        [References(typeof (CurrencyType))]
        public long CurrencyId { get; set; }

        public DateTime CreateDate { get; set; }
        
        public long? InvoiceId { get; set; }

        public string GatewayResponse { get; set; }

        [Default((long) TRANSACTION_NOTIFICATION_STATUS.None)]
        [References(typeof (TransactionNotificationStatusType))]
        public long TransactionNotificationStatusId { get; set; }

        public DateTime? SettlementDate { get; set; }

        [References(typeof (Transaction))]
        public long? RelatedTransactionId { get; set; }

        public string RelatedGatewayTransactionId { get; set; }

        public Transaction()
        {
            CurrencyId = (long) CURRENCY_TYPE.USDollar;
            TransactionNotificationStatusId = (long) TRANSACTION_NOTIFICATION_STATUS.None;
            SettlementDate = null;
        }
    }
}