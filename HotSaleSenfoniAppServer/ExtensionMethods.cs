using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotSaleSenfoniAppServer
{
    public static class ExtensionMethods
    {
        public static int toInt(this object value)
        {
            if (object.ReferenceEquals(value, null)) return 0;
            if (object.ReferenceEquals(value, DBNull.Value)) return 0;
            return Convert.ToInt32(value);
        }

        public static decimal toDec(this object value)
        {
            if (object.ReferenceEquals(value, null)) return 0M;
            if (object.ReferenceEquals(value, DBNull.Value)) return 0M;
            return Convert.ToDecimal(value);
        }

        public static bool toBool(this object value)
        {
            if (object.ReferenceEquals(value, null)) return false;
            if (object.ReferenceEquals(value, DBNull.Value)) return false;
            if (value.ToString() == "t" || value.ToString() == "1") return true;
            if (value.ToString() == "f" || value.ToString() == "0") return false;
            return Convert.ToBoolean(value);
        }

        public static string toStr(this object value)
        {
            if (object.ReferenceEquals(value, null)) return "";
            if (object.ReferenceEquals(value, DBNull.Value)) return "";
            return Convert.ToString(value);
        }
    }
}