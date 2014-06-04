

using System.Collections.Generic;

namespace Auth_202.Model.Constants
{
    public static class SystemConstants
    {
        public const int INFINITE_OCCURENCES = int.MaxValue;
        public const string ZERO_DOLLAR_TRANSACTION = "ZeroDollar";
        public const string LoginUrl = "~/LoginPage.html";
    };

    public enum CURRENCY_TYPE : long
    {
        USDollar = 1,
        CandianDollar,
        Peso
    };

    public enum TRANSACTION_TYPE : long
    {
        AuthorizeAndCapture = 1,
        AuthorizeOnly,
        CapturePrior,
        Refund,
        Void,
        ZeroDollar,
        Unknown
    };

    // http://www.authorize.net/support/CP/helpfiles/Miscellaneous/Pop-up_Terms/ALL/Transaction_Status.htm
    // http://stackoverflow.com/questions/6079812/getting-transaction-details-from-authorize-net
    // http://www.authorize.net/support/merchant/Transaction_Response/Transaction_Response.htm
    public enum TRANSACTION_STATUS : long
    {
        Pending = 1,
        Settled,
        Refunded,
        Voided,
        Expired,
        Declined,
        Error
    };

    public enum TRANSACTION_NOTIFICATION_STATUS : long
    {
        None = 1,
        DeclinedNotification,
        ErrorNotification,
        SettledNotification,
        RefundedNotification,
        VoidedNotification
    };

    public static class DefaultAdmin
    {
        public const int Id = 101;
        public const string Email = "rhartman@omnisite";
        public const string Username = "Administrator";
        public const string Password = "dfjk232f00232D232!";
        public static readonly List<string> Roles = new List<string> {"Admin"};
        public static readonly List<string> Permissions = new List<string> {"Admin"};
    };


}
