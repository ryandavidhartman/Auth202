
using System;
using System.Collections.Generic;
using Auth_202.DatabaseSetup;
using Auth_202.Model.Constants;
using Auth_202.Model.Data;
using Auth_202.WebAPI;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using ServiceStack.OrmLite;


namespace Auth_202.UnitTests
{
    [TestFixture]
    public class WebAuthenticationTests
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

        private IServiceClient GetJsonClient()
        {
            return new JsonServiceClient(ListeningOn);
        }

        [Test]
        public void get_currency_types_ok_with_authentication()
        {
            var client = GetJsonClient();
            client.SetCredentials(DefaultAdmin.Username, DefaultAdmin.Password);
            var response = client.Get<List<CurrencyType>>("/currencytypes");
            Assert.IsNotNull(response);
        }


        [Test]
        public void get_currency_types_ok_without_authentication()
        {
            var client = GetJsonClient();
            var response = client.Get<List<CurrencyType>>("/currencytypes");
            Assert.IsNotNull(response);
        }


        [Test]
        public void post_transaction_fails_without_authentication()
        {
            var transaction = new Transaction
            {
                Amount = 10.00m,
                Card = "XXXXXXXXXX124",
                CreateDate = DateTime.UtcNow,
                SubscriptionId =  101,
                GatewayTransactionId =  "123456",
                TransactionTypeId = (long) TRANSACTION_TYPE.AuthorizeAndCapture,
                TransactionStatusId = (long) TRANSACTION_STATUS.Pending,
                GatewayResponse = "ok"
            };
            var client = GetJsonClient();
            var error = Assert.Throws<WebServiceException>(() => client.Post(transaction));
            Assert.AreEqual("Unauthorized", error.Message);
        }
    }
}
