using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using RESTServiceUtilities.Implementations.Db;

namespace Auth_202.DataLayer.Repositories
{
    public class TransactionStatusTypeRepository : DbStatusTypeRepository<TransactionStatusType, GetTransactionStatusTypes>
    {
    }
}