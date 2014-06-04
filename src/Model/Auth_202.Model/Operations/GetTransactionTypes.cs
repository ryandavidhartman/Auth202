using System.Collections.Generic;
using Auth_202.Model.Data;
using RESTServiceUtilities.Interfaces;
using ServiceStack;

namespace Auth_202.Model.Operations
{
    [Api("Return a List of Transaction Type Resources")]
    [Route("/TransactionTypes", "GET")]
    [Route("/TransactionTypes/{Ids}", "GET")]
    public class GetTransactionTypes : IReturn<List<TransactionStatusType>>, IDatas
    {
        public List<long> Ids { get; set; }
    }
}