﻿using Auth_202.Model.Data;
using MessagingServiceUtilities.Interfaces;

namespace Auth_202.Model.Operations
{
    public class PutCurrencyType : IMessagingPutDto<CurrencyType>
    {
        public CurrencyType Body { get; set; }
    }
}