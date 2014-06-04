using System;
using System.Collections.Generic;
using System.Data;
using Auth_202.Model.Constants;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using RESTServiceUtilities.Implementations.Business;
using ServiceStack;

namespace Auth_202.BusinessLogic.BusinessLogic
{
    public class TransactionLogic : StandardBusinessLogic<Transaction, GetTransactions>
    {
        public virtual Transaction SettleTransaction(long transactionId, DateTime settlmentTime)
        {
            if (transactionId < 1 || settlmentTime < DateTime.UtcNow.AddYears(-1))
                throw Logger.LogError(new ArgumentException(string.Format("Bad Request in SettleTransaction: TransactionId: {0} SettlementTime: {1}", transactionId, settlmentTime)));

            var response = Get(new GetTransactions {Ids = new List<long> {transactionId}});
            if (response == null || response.Count != 1)
                throw Logger.LogError(new DataException(string.Format("SettleTransaction error retrieving TransactionId: {0}", transactionId)));

            var transaction = response[0];
            if (transaction.TransactionStatusId != (long) TRANSACTION_STATUS.Pending)
                throw Logger.LogError(new DataException(string.Format("SettleTransaction error TransactionId: {0} is in state: {1}", transactionId, transaction.TransactionStatusId)));

            transaction.SettlementDate = settlmentTime;
            transaction.TransactionStatusId = (long) TRANSACTION_STATUS.Settled;

            transaction = Put(transaction);

            return transaction;
        }
        
        public override Transaction Post(Transaction data)
        {

            return base.Post(data);
        }
    }
}