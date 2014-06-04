using Auth_202.Model.Data;
using MessagingServiceUtilities.Interfaces;

namespace Auth_202.Model.Operations
{
    public class PutTransactionStatusType : IMessagingPutDto<TransactionStatusType>
    {
        public TransactionStatusType Body { get; set; }
    }
}