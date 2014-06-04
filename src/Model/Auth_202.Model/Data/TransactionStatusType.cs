using RESTServiceUtilities.Interfaces;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace Auth_202.Model.Data
{
    [Api("Insert update or delete a Transaction Status Type Resource")]
    [Route("/TransactionStatusTypes", "POST")]
    [Route("/TransactionStatusTypes/{Id}", "PUT")]
    [Route("/TransactionStatusTypes/{Id}", "DELETE")]
    public class TransactionStatusType : IStatusType, IReturn<TransactionStatusType>
    {
        [AutoIncrement]
        public long Id { get; set; }

        public string Status { get; set; }
        public bool IsErrorStatus { get; set; }
    }
}