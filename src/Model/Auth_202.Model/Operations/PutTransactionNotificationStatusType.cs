using Auth_202.Model.Data;
using MessagingServiceUtilities.Interfaces;

namespace Auth_202.Model.Operations
{
    public class PutTransactionNotificationStatusType : IMessagingPutDto<TransactionNotificationStatusType>
    {
        public TransactionNotificationStatusType Body { get; set; }
    }
}