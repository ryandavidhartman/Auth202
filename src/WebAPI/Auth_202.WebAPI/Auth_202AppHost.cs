using System;
using System.Configuration;
using System.Text;
using System.Web;
using Auth_202.BusinessLogic.BusinessLogic;
using Auth_202.DataLayer.Repositories;
using Auth_202.Model.Constants;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using Auth_202.WebAPI.Services;
using Funq;
using MessagingServiceUtilities.Dto.Responses;
using RESTServiceUtilities.Interfaces;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.Host;
using ServiceStack.Host.AspNet;
using ServiceStack.Host.Handlers;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using ServiceStack.Messaging;
using ServiceStack.Messaging.Redis;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.Validation;
using WebServiceUtilities.Implementations;
using WebServiceUtilities.Utilities;
using Logger = CommonServiceUtilities.Logger;

namespace Auth_202.WebAPI
{
    public class Auth_202AppHost : AppHostHttpListenerBase
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public Auth_202AppHost() : base("Auth 202 Services", typeof(Auth_202AppHost).Assembly)
        {
            _dbConnectionFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["Auth202Db"].ConnectionString, SqlServerDialect.Provider);
        }

        public Auth_202AppHost(IDbConnectionFactory dbConnectionFactory) : base("Auth 202 Services", typeof(Auth_202AppHost).Assembly)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public override void Configure(Container container)
        {
            LogManager.LogFactory = new Log4NetFactory(true);
            
            container.Register(_dbConnectionFactory);
            var basicAuthProvider = new BasicAuthProvider();
            container.Register(basicAuthProvider);

            Plugins.Add(new AuthFeature( () => new AuthUserSession(), new IAuthProvider[] {basicAuthProvider, }, SystemConstants.LoginUrl ));

            var userRepo = new OrmLiteAuthRepository(_dbConnectionFactory);
            container.Register<IAuthRepository>(userRepo);

            var cacheClient = new MemoryCacheClient();
            container.Register(cacheClient);
           
            var currencyTypeRepository = new CurrencyTypeRepository { DbConnectionFactory = _dbConnectionFactory };
            var transactionTypeRepository = new TransactionTypeRepository { DbConnectionFactory = _dbConnectionFactory };
            var transactionStatusTypeRepository = new TransactionStatusTypeRepository { DbConnectionFactory = _dbConnectionFactory };
            var transactionNotificationStatusTypeRepository = new TransactionNotificationStatusTypeRepository { DbConnectionFactory = _dbConnectionFactory };
            var transactionRepository = new TransactionRepository { DbConnectionFactory = _dbConnectionFactory };

            var currencyTypeLogic = new CurrencyTypeLogic { Repository = currencyTypeRepository };
            var transactionTypeLogic = new TransactionTypeLogic { Repository = transactionTypeRepository };
            var transactionStatusTypeLogic = new TransactionStatusTypeLogic { Repository = transactionStatusTypeRepository };
            var transactionNotificationStatusTypeLogic = new TransactionNotificationStatusTypeLogic { Repository = transactionNotificationStatusTypeRepository };
            var transactionLogic = new TransactionLogic {Repository = transactionRepository};

            container.Register<IRest<CurrencyType, GetCurrencyTypes>>(currencyTypeLogic);
            container.Register<IRest<TransactionType, GetTransactionTypes>>(transactionTypeLogic);
            container.Register<IRest<TransactionStatusType, GetTransactionStatusTypes>>(transactionStatusTypeLogic);
            container.Register<IRest<TransactionNotificationStatusType, GetTransactionNotificationStatusTypes>>(transactionNotificationStatusTypeLogic);
            container.Register<IRest<Transaction, GetTransactions>>(transactionLogic);
           
            CatchAllHandlers.Add((httpMethod, pathInfo, filePath) => pathInfo.StartsWith("/favicon.ico") ? new FavIconHandler() : null);

            var redisLocation = ConfigurationManager.AppSettings["ReddisService"];
            Container.Register<IRedisClientsManager>(new PooledRedisClientManager(redisLocation));
            var mqService = new RedisMqServer(container.Resolve<IRedisClientsManager>());
            var messagingHandlers = new MessageService { Log = new Logger(typeof(MessageService).Name) };


            Func<IMessage, IMessage> filterSecureRequests = (message) =>
            {
                /*
                var tag = message.Tag;

                if (string.IsNullOrWhiteSpace(tag))
                    return message;

                if (tag.StartsWith("basic ", StringComparison.InvariantCultureIgnoreCase))
                {
                    var creds = Encoding.UTF8.GetString(Convert.FromBase64String(tag.Substring(5)));

                    var i = creds.IndexOf(':');
                    var userName = creds.Substring(0, i);
                    var userPass = creds.Substring(i + 1);


                    if (userName != SystemConstants.AllowedUser || userPass != SystemConstants.AllowedPass)
                    {
                        message.Tag = null;
                        return message;
                    }

                    _currentSessionGuid = Guid.NewGuid();
                    var sessionKey = userName + "/" + _currentSessionGuid.ToString("N");

                    SessionContext = new SessionContext { SessionKey = sessionKey, Username = userName };
                    container.Register(SessionContext);
                    message.Tag = sessionKey;
                    return message;
                }

                message.Tag = null;*/

                return message;
            };

            mqService.RequestFilter = filterSecureRequests;


            Func<IMessage<Transaction>, PostResponse<Transaction>> handlePostTransactions = (message) =>
            {
                var service = new TransactionWebService { Logic = transactionLogic };
                var request = new BasicRequest {Message = message, Dto = message.GetBody()};
                var response = new BasicResponse(request);
                
                //userRepo.TryAuthenticate()

                service.SessionFactory.GetOrCreateSession(request, response);
                var session = service.GetSession();
                
                
                
                
                session.UserName = "ryan";
                
                
                var results = new PostResponse<Transaction> {Result = (Transaction) service.Post(message.GetBody())};
                return results;
            };


            // Dto Get Operations

            mqService.RegisterHandler<GetCurrencyTypes>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), currencyTypeLogic));
            mqService.RegisterHandler<GetTransactions>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionLogic));
            mqService.RegisterHandler<GetTransactionStatusTypes>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionStatusTypeLogic));
            mqService.RegisterHandler<GetTransactionNotificationStatusTypes>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionNotificationStatusTypeLogic));
            mqService.RegisterHandler<GetTransactionTypes>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionTypeLogic));

            // Dto Post Operations
            mqService.RegisterHandler<CurrencyType>(m => messagingHandlers.MessagingPostRequest(m.GetBody(), currencyTypeLogic.Post));
           
            mqService.RegisterHandler<Transaction>(handlePostTransactions);
            mqService.RegisterHandler<TransactionStatusType>(m => messagingHandlers.MessagingPostRequest(m.GetBody(), transactionStatusTypeLogic.Post));
            mqService.RegisterHandler<TransactionNotificationStatusType>(m => messagingHandlers.MessagingPostRequest(m.GetBody(), transactionNotificationStatusTypeLogic.Post));
            mqService.RegisterHandler<TransactionType>(m => messagingHandlers.MessagingPostRequest(m.GetBody(), transactionTypeLogic.Post));

            // Dto Put Opertations
            mqService.RegisterHandler<DeleteCurrencyType>(m => messagingHandlers.MessagingDeleteWrapper(m.GetBody(), currencyTypeLogic));
            mqService.RegisterHandler<DeleteTransaction>(m => messagingHandlers.MessagingDeleteWrapper(m.GetBody(), transactionLogic));
            mqService.RegisterHandler<DeleteTransactionStatusType>(m => messagingHandlers.MessagingDeleteWrapper(m.GetBody(), transactionStatusTypeLogic));
            mqService.RegisterHandler<DeleteTransactionNotificationStatusType>(m => messagingHandlers.MessagingDeleteWrapper(m.GetBody(), transactionNotificationStatusTypeLogic));
            mqService.RegisterHandler<DeleteTransactionType>(m => messagingHandlers.MessagingDeleteWrapper(m.GetBody(), transactionTypeLogic));
            
            mqService.Start();

        }
    }
   
}