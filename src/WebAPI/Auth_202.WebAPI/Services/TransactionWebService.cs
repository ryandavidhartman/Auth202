using Auth_202.BusinessLogic.BusinessLogic;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using ServiceStack;
using WebServiceUtilities.Implementations;

namespace Auth_202.WebAPI.Services
{
    public class TransactionWebService : StandardWebService<Transaction, GetTransactions, TransactionLogic>
    {
        [Authenticate]
        public override object Post(Transaction data)
        {
            return base.Post(data);
        }
    }
}