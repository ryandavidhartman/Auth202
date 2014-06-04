using RESTServiceUtilities.Interfaces;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace Auth_202.Model.Data
{
    [Api("Insert update or delete a Transaction Status Type Resource")]
    [Route("/TransactionNotificationStatusTypes", "POST")]
    [Route("/TransactionNotificationStatusTypes/{Id}", "PUT")]
    [Route("/TransactionNotificationStatusTypes/{Id}", "DELETE")]
    public class TransactionNotificationStatusType : IStatusType, IReturn<TransactionNotificationStatusType>
    {
        [AutoIncrement]
        public long Id { get; set; }

        public string Status { get; set; }
        public bool IsErrorStatus { get; set; }
    }
}