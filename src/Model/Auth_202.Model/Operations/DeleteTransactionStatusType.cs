using Auth_202.Model.Data;
using MessagingServiceUtilities.Interfaces;

namespace Auth_202.Model.Operations
{
    public class DeleteTransactionStatusType : IMessagingDeleteDto<TransactionStatusType>
    {
        public TransactionStatusType Body { get; set; }
    }
}