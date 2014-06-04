using System;
using System.Collections.Generic;
using System.Linq;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using RESTServiceUtilities.Implementations.Db;
using ServiceStack.OrmLite;

namespace Auth_202.DataLayer.Repositories
{
    public class TransactionRepository : StandardDbRepository<Transaction, GetTransactions>
    {
        public override List<Transaction> Get(GetTransactions request)
        {
            List<Transaction> results = null;

            if (IsEmptyGetRequest(request))
            {
                results = GetAll();
            }
            else if (request.Ids != null && request.Ids.Count > 0)
            {
                results = GetByIds(request.Ids);
            }
            else if (request.SubscriptionIds != null && request.SubscriptionIds.Count > 0)
            {
                results = GetBySubscriptionIds(request.SubscriptionIds);
            }
            else if (request.SubscriptionOwnerIds != null && request.SubscriptionOwnerIds.Count > 0)
            {
                results = GetBySubscriptionOwnerIds(request.SubscriptionOwnerIds);
            }
            else if (request.SubscriptionOwnerMerchantIds != null && request.SubscriptionOwnerMerchantIds.Count > 0)
            {
                results = GetBySubscriptionOwnerMerchantIds(request.SubscriptionOwnerMerchantIds);
            }
            else if (request.GatewayTransactionIds != null && request.GatewayTransactionIds.Count > 0)
            {
                results = GetByGatewayTransactionIds(request.GatewayTransactionIds);
            }
            else if (request.TransactionStatusIds != null && request.TransactionStatusIds.Count > 0)
            {
                results = GetByTransactionStatusIds(request.TransactionStatusIds);
            }
            else if (request.NotificationStatusIds != null && request.NotificationStatusIds.Count > 0)
            {
                results = GetByNotificationStatusIds(request.NotificationStatusIds);
            }

            return results;
        }

        public override bool IsEmptyGetRequest(GetTransactions request)
        {
            if (request == null)
                return true;

            if (request.Ids != null && request.Ids.Count > 0)
                return false;

            if (request.SubscriptionIds != null && request.SubscriptionIds.Count > 0)
                return false;

            if (request.GatewayTransactionIds != null && request.GatewayTransactionIds.Count > 0)
                return false;

            if (request.TransactionStatusIds != null && request.TransactionStatusIds.Count > 0)
                return false;

            if (request.NotificationStatusIds != null && request.NotificationStatusIds.Count > 0)
                return false;

            if (request.SubscriptionOwnerIds != null && request.SubscriptionOwnerIds.Count > 0)
                return false;

            if (request.SubscriptionOwnerMerchantIds != null && request.SubscriptionOwnerMerchantIds.Count > 0)
                return false;

            return true;
        }

        public virtual List<Transaction> GetByGatewayTransactionIds(List<string> gatewayTransactionIds)
        {
            if (gatewayTransactionIds == null || gatewayTransactionIds.Count < 1) throw new ArgumentException("GetByGatewayTransactionIds: gatewayTransactionIds is null");

            using (var dbConnection = DbConnectionFactory.Open())
            {
                return dbConnection.Select<Transaction>(t => Sql.In(t.GatewayTransactionId, gatewayTransactionIds)).ToList();
            }
        }

        public virtual List<Transaction> GetBySubscriptionIds(List<long> subscriptionIds)
        {
            if (subscriptionIds == null || subscriptionIds.Count < 1)
                throw new ArgumentException("GetBySubscriptionIds: subscriptionIds is null");

            using (var dbConnection = DbConnectionFactory.Open())
            {
                return dbConnection.Select<Transaction>(t => Sql.In(t.SubscriptionId, subscriptionIds)).ToList();
            }
        }

        public virtual List<Transaction> GetBySubscriptionOwnerMerchantIds(List<string> subscriptionOwnerMerchantIds)
        {
            if (subscriptionOwnerMerchantIds == null || subscriptionOwnerMerchantIds.Count < 1)
                throw new ArgumentException("GetBySubscriptionOwnerMerchantIds: subscriptionOwnerMerchantIds is null");

            using (var dbConnection = DbConnectionFactory.Open())
            {
                var merchantIds = string.Format("'{0}'", subscriptionOwnerMerchantIds[0]);

                for (var i = 1; i < subscriptionOwnerMerchantIds.Count; i++)
                {
                    merchantIds = string.Format("{0}, '{1}'", merchantIds, subscriptionOwnerMerchantIds[i]);
                }

                var sqlQuery = string.Format("select [Transaction].* from [Transaction] " +
                                             "join Subscription on Subscription.Id = [Transaction].SubscriptionId " +
                                             "join SubscriptionOwner on SubscriptionOwner.Id = Subscription.SubscriptionOwnerId " +
                                             "where SubscriptionOwner.MerchantId in ({0})", merchantIds);

                return dbConnection.Select<Transaction>(sqlQuery).ToList<Transaction>();
            }
        }

        public virtual List<Transaction> GetBySubscriptionOwnerIds(List<long> subscriptionOwnerIds)
        {
            if (subscriptionOwnerIds == null || subscriptionOwnerIds.Count < 1)
                throw new ArgumentException("GetBySubscriptionOwnerIds: subscriptionOwnerIds is null");

            using (var dbConnection = DbConnectionFactory.Open())
            {
                var merchantIds = string.Format("{0}", subscriptionOwnerIds[0]);

                for (var i = 1; i < subscriptionOwnerIds.Count; i++)
                {
                    merchantIds = string.Format("{0}, {1}", merchantIds, subscriptionOwnerIds[i]);
                }

                var sqlQuery = string.Format("select [Transaction].* from [Transaction] " +
                                             "join Subscription on Subscription.Id = [Transaction].SubscriptionId " +
                                             "join SubscriptionOwner on SubscriptionOwner.Id = Subscription.SubscriptionOwnerId " +
                                             "where SubscriptionOwner.Id in ({0})", merchantIds);

                return dbConnection.Select<Transaction>(sqlQuery).ToList<Transaction>();
            }
        }

        public virtual List<Transaction> GetByTransactionStatusIds(List<long> transactionStatusIds)
        {
            if (transactionStatusIds == null || transactionStatusIds.Count < 1)
                throw new ArgumentException("GetByTransactionStatusIds: transactionStatusIds is null");

            using (var dbConnection = DbConnectionFactory.Open())
            {
                return dbConnection.Select<Transaction>(t => Sql.In(t.TransactionStatusId, transactionStatusIds)).ToList();
            }
        }

        public virtual List<Transaction> GetByNotificationStatusIds(List<long> notificationStatusIds)
        {
            if (notificationStatusIds == null || notificationStatusIds.Count < 1)
                throw new ArgumentException("GetByNotificationStatusIds: notificationStatusIds is null");

            using (var dbConnection = DbConnectionFactory.Open())
            {
                return dbConnection.Select<Transaction>(t => Sql.In(t.TransactionNotificationStatusId, notificationStatusIds)).ToList();
            }
        }

        public override void ValidateInsertData(Transaction data)
        {
            if (data == null)
                throw new ArgumentException("Transaction must not be null");

            if (data.SubscriptionId <= default(long))
                throw new ArgumentException("Transaction: SubscriptionId must not be null");

            if (string.IsNullOrEmpty(data.GatewayTransactionId))
                throw new ArgumentException("Transaction: GatewayTransactionId must not be null");

            if (data.TransactionTypeId <= default(long))
                throw new ArgumentException("Transaction: TransactionTypeId  must not be null");

            if (data.TransactionStatusId <= default(long))
                throw new ArgumentException("Transaction: TransactionStatusId must not be null");

            if (data.CurrencyId <= default(long))
                throw new ArgumentException(string.Format("Transaction: {0} is an invalid amount for a Credit Transaction", data.Amount));

            if (string.IsNullOrEmpty(data.GatewayResponse))
                throw new ArgumentException("Transaction: GatewayResponse must not be null");

            if (data.TransactionNotificationStatusId <= default(long))
                throw new ArgumentException("Transaction: TransactionNotificationStatusId must not be null");

            // Now we track down any foreign key constraints
            using (var dbConnection = DbConnectionFactory.Open())
            {
                var transactionType = dbConnection.SingleById<TransactionType>(data.TransactionTypeId);
                if (transactionType == null || transactionType.Id != data.TransactionTypeId)
                    throw new ArgumentException(string.Format("transaction.TransactionTypeId={0} does not exist in DB", data.TransactionTypeId));

                var transactionStatusType = dbConnection.SingleById<TransactionStatusType>(data.TransactionStatusId);
                if (transactionStatusType == null || transactionStatusType.Id != data.TransactionStatusId)
                    throw new ArgumentException(string.Format("transaction.TransactionStatusId={0} does not exist in DB", data.TransactionStatusId));

                var currencyType = dbConnection.SingleById<CurrencyType>(data.CurrencyId);
                if (currencyType == null || currencyType.Id != data.CurrencyId)
                    throw new ArgumentException(string.Format("transaction.CurrencyId){0} does not exist in DB", data.CurrencyId));

                var transactionNotificationType = dbConnection.SingleById<TransactionNotificationStatusType>(data.TransactionNotificationStatusId);
                if (transactionNotificationType == null || transactionNotificationType.Id != data.TransactionNotificationStatusId)
                    throw new ArgumentException(string.Format("transaction.TransactionNotificationStatusId){0} does not exist in DB", data.TransactionNotificationStatusId));
            }
        }
    }
}