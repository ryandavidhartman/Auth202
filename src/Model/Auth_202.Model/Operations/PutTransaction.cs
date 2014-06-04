using Auth_202.Model.Data;
using MessagingServiceUtilities.Interfaces;

namespace Auth_202.Model.Operations
{
    public class PutTransaction : IMessagingPutDto<Transaction>
    {
        public Transaction Body { get; set; }
    }
}