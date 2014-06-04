using System.Collections.Generic;
using Auth_202.Model.Data;
using RESTServiceUtilities.Interfaces;
using ServiceStack;

namespace Auth_202.Model.Operations
{
    [Api("Return a List of Transaction Status Type Resources")]
    [Route("/TransactionNotificationStatusTypes", "GET")]
    [Route("/TransactionNotificationStatusTypes/{Ids}", "GET")]
    public class GetTransactionNotificationStatusTypes : IReturn<List<TransactionStatusType>>, IDatas
    {
        public List<long> Ids { get; set; }
    }
}