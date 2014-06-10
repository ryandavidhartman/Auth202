using System;
using System.Collections.Generic;
using System.Text;
using Auth_202.DatabaseSetup;
using Auth_202.Model.Constants;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using Auth_202.WebAPI;
using MessagingServiceUtilities.Dto.Responses;
using MessagingServiceUtilities.Implementations;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using ServiceStack.Messaging;
using ServiceStack.Messaging.Redis;
using ServiceStack.OrmLite;
using ServiceStack.Redis;

namespace Auth_202.UnitTests
{
    [TestFixture]
    public class RedisAuthenticationTests
    {
        private Auth_202AppHost _appHost;
        protected virtual string ListeningOn
        {
            get { return "http://localhost:50334/"; }
        }

        private IDbConnectionFactory _dbConnectionFactory;

        [TestFixtureSetUp]
        public void on_set_up()
        {
            LogManager.LogFactory = new Log4NetFactory(true);
            _dbConnectionFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);
            DataBaseHelper.Settup_Test_Database(_dbConnectionFactory);

            _appHost = new Auth_202AppHost(_dbConnectionFactory);
            _appHost.Init();
            _appHost.Start(ListeningOn);
        }

        [TestFixtureTearDown]
        public void on_tear_down()
        {
            _appHost.Dispose();
        }

        private static MessagingHelper GetRedisClient()
        {
            return MessagingHelper.Instance;
        }

        [Test]
        public void get_currency_types_no_authentication()
        {
            var redisMqClient = GetRedisClient();


            var response = redisMqClient.GetListFromMessageQueue<CurrencyType, GetCurrencyTypes>(new GetCurrencyTypes());
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsErrorResponse());
            
            Assert.AreEqual(typeof(Message<GetResponseList<CurrencyType>>), response.GetType());
            Assert.IsNotNull(response.GetBody());
            var result = response.GetBody().Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void post_transaction_ok_without_authentication()
        {  
            var transaction = new Transaction 
            {
                Amount = 10.00m,
                Card = "XXXXXXXXXX124",
                CreateDate = DateTime.UtcNow,
                SubscriptionId = 101,
                GatewayTransactionId = "123456",
                TransactionTypeId = (long)TRANSACTION_TYPE.AuthorizeAndCapture,
                TransactionStatusId = (long)TRANSACTION_STATUS.Pending,
                GatewayResponse = "ok"
            };

            var client = GetRedisClient();
            var response = client.PostData(transaction).GetBody();
            Assert.AreEqual("200", response.Status);
        }

        [Test]
        public void post_transaction_ok_with_authentication()
        {
            var uniqueCallbackQ = "mq:c1" + ":" + Guid.NewGuid().ToString("N");

            var transaction = new Transaction
            {
                Amount = 10.00m,
                Card = "XXXXXXXXXX124",
                CreateDate = DateTime.UtcNow,
                SubscriptionId = 101,
                GatewayTransactionId = "123456",
                TransactionTypeId = (long)TRANSACTION_TYPE.AuthorizeAndCapture,
                TransactionStatusId = (long)TRANSACTION_STATUS.Pending,
                GatewayResponse = "ok"
            };
            
            
            var clientMsg = new Message<Transaction>(transaction)
            {
                ReplyTo = uniqueCallbackQ,
                Tag = "basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(DefaultAdmin.Username + ":" + DefaultAdmin.Password))
            };

            var redisFactory = new PooledRedisClientManager("localhost:6379");
            var mqHost = new RedisMqServer(redisFactory, retryCount: 2);

            var mqClient = mqHost.CreateMessageQueueClient();

            mqClient.Publish(clientMsg);
            var response = mqClient.Get<PostResponse<Transaction>>(clientMsg.ReplyTo, new TimeSpan(0,10,10)); //Blocks thread on client until reply message is received
            var result = response.GetBody().Result;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id > 0);
        }


    }
}