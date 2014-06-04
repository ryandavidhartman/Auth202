﻿using Auth_202.Model.Data;
using MessagingServiceUtilities.Interfaces;

namespace Auth_202.Model.Operations
{
    public class PutTransactionType : IMessagingPutDto<TransactionType>
    {
        public TransactionType Body { get; set; }
    }
}