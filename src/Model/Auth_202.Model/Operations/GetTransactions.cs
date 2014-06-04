using System.Collections.Generic;
using Auth_202.Model.Data;
using RESTServiceUtilities.Interfaces;
using ServiceStack;

namespace Auth_202.Model.Operations
{
    [Api("Return a List of Transaction Resources")]
    [Route("/Transactions", "GET")]
    [Route("/Transactions/{Ids}", "GET")]
    [Route("/Transactions/Subscriptions/{SubscriptionIds}", "GET")]
    [Route("/Transactions/SubscriptionOwners/{SubscriptionOwnerIds}", "GET")]
    [Route("/Transactions/SubscriptionOwners/{SubscriptionOwnerMerchantIds}", "GET")]
    [Route("/Transactions/TransactionStatus/{TransactionStatusIds}", "GET")]
    [Route("/Transactions/GatewayTransactions/{GatewayTransactionIds}", "GET")]
    [Route("/Transactions/NotificationStatus/{NotificationStatusIds}", "GET")]
    public class GetTransactions : IReturn<List<Transaction>>, IDatas
    {
        public List<long> Ids { get; set; }
        public List<long> SubscriptionIds { get; set; }
        public List<long> SubscriptionOwnerIds { get; set; }
        public List<string> SubscriptionOwnerMerchantIds { get; set; }
        public List<string> GatewayTransactionIds { get; set; }
        public List<long> NotificationStatusIds { get; set; }
        public List<long> TransactionStatusIds { get; set; }
    }
}