using System;
using RESTServiceUtilities.Interfaces;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace Auth_202.Model.Data
{
    [Serializable]
    [Api("Insert update or delete a Currency Type Resource")]
    [Route("/CurrencyTypes", "POST")]
    [Route("/CurrencyTypes/{Id}", "PUT")]
    [Route("/CurrencyTypes/{Id}", "DELETE")]
    public class CurrencyType : IReturn<CurrencyType>, IType
    {
        [AutoIncrement]
        public long Id { get; set; }

        public string Description { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
    }
}