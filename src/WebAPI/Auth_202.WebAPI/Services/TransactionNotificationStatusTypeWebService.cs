using Auth_202.BusinessLogic.BusinessLogic;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using WebServiceUtilities.Implementations;

namespace Auth_202.WebAPI.Services
{
    public class TransactionNotificationStatusTypeWebService :
        StandardWebService<TransactionNotificationStatusType, GetTransactionNotificationStatusTypes, TransactionNotificationStatusTypeLogic>
    {
    }
}