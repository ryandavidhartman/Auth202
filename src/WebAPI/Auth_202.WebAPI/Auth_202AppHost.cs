using System;
using System.Collections.Generic;
using System.Configuration;
using Auth_202.BusinessLogic.BusinessLogic;
using Auth_202.DataLayer.Repositories;
using Auth_202.Model.Constants;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using Auth_202.WebAPI.Services;
using Funq;
using RESTServiceUtilities.Interfaces;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using ServiceStack.Messaging;
using ServiceStack.Messaging.Redis;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
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

            Plugins.Add(new AuthFeature( () => new AuthUserSession(), new IAuthProvider[] {new BasicAuthProvider()}, SystemConstants.LoginUrl ));

            var userRepo = new OrmLiteAuthRepository(_dbConnectionFactory);
            container.Register<IAuthRepository>(userRepo);
           
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

            // Dto Get Operations

            mqService.RegisterHandler<GetCurrencyTypes>(ServiceController.ExecuteMessage);

            //Action<IMessage<GetCurrencyTypes>, Exception> error1 = MessagingError; 
          
            /*
            mqService.RegisterHandler<GetCurrencyTypes>(m =>
            {
                var webService = new CurrencyTypeWebService {Logic = currencyTypeLogic};

                    var raw = webService.Get(m.GetBody());
                    var result = raw as List<CurrencyType>;
                    if (result == null)
                        throw new ApplicationException("No results!");
                return result;
            }, error1 );*/
            
            mqService.RegisterHandler<GetTransactions>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionLogic));
            mqService.RegisterHandler<GetTransactionStatusTypes>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionStatusTypeLogic));
            mqService.RegisterHandler<GetTransactionNotificationStatusTypes>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionNotificationStatusTypeLogic));
            mqService.RegisterHandler<GetTransactionTypes>(m => messagingHandlers.MessagingGetWrapper(m.GetBody(), transactionTypeLogic));

            // Dto Post Operations
            mqService.RegisterHandler<CurrencyType>(m => messagingHandlers.MessagingPostRequest(m.GetBody(), currencyTypeLogic.Post));
           
            mqService.RegisterHandler<Transaction>(m => messagingHandlers.MessagingPostRequest(m.GetBody(), new TransactionWebService().Post  ));
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

        private static void MessagingError(IMessage<GetCurrencyTypes> message, Exception error)
        {
            var errorLogger = new Logger(typeof(Auth_202AppHost).Name);
            errorLogger.LogError(error);

            var redisFactory = new PooledRedisClientManager("localhost:6379");
            var mqHost = new RedisMqServer(redisFactory, retryCount: 2);
            var mqClient = mqHost.CreateMessageQueueClient();

            var responseStatus = new ResponseStatus {ErrorCode = "500", Message = error.Message, StackTrace = error.StackTrace};
            
            mqClient.Publish(message.ReplyTo, new Message<Exception>{Error = responseStatus,
                Body = error,
                CreatedDate = DateTime.UtcNow
            });

        }
    }
   
}