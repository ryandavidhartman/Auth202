using Auth_202.BusinessLogic.BusinessLogic;
using Auth_202.Model.Constants;
using Auth_202.Model.Data;
using Auth_202.Model.Operations;
using ServiceStack;
using WebServiceUtilities.Implementations;

namespace Auth_202.WebAPI.Services
{
    public class CurrencyTypeWebService : StandardWebService<CurrencyType, GetCurrencyTypes, CurrencyTypeLogic>
    {
        //[Authenticate(HtmlRedirect = SystemConstants.LoginUrl)]
        public object Any(GetCurrencyTypes request)
        {
            var headers = Request.Headers;
            return base.Get(request);
        }


    }
}