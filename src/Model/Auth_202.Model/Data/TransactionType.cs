using RESTServiceUtilities.Interfaces;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace Auth_202.Model.Data
{
    [Api("Insert update or delete a Transaction Status Type Resource")]
    [Route("/TransactionTypes", "POST")]
    [Route("/TransactionTypes/{Id}", "PUT")]
    [Route("/TransactionTypes/{Id}", "DELETE")]
    public class TransactionType : IType, IReturn<TransactionType>
    {
        [AutoIncrement]
        public long Id { get; set; }

        public string Description { get; set; }
    }
}