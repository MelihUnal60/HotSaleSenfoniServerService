namespace HotSaleServiceTables
{
    using System;

    public class Notify : Token
    {
        public string PushCode;

        public static Token ToToken(Notify notify)
        {
            return new Token { BranchCode = notify.BranchCode, Username = notify.Username, Password = notify.Password };
        }
    }
}

