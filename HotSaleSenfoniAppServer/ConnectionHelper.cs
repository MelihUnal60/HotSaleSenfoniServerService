using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace HotSaleSenfoniAppServer
{
    public class ConnectionHelper
    {
        static ConnectionHelper instance;
        public static ConnectionHelper Instance
        {
            get
            {
                if (instance == null) instance = new ConnectionHelper();
                return instance;
            }
        }

        private int connectionCount = 0;
        private DbProviderFactory providerFactory = null;
        private string connString = null, providerName = null;

        public IDbConnection GetConnection()
        {
            IDbConnection conn = null;
            try
            {

                if (string.IsNullOrWhiteSpace(connString) && System.Configuration.ConfigurationManager.ConnectionStrings["uyum"] != null)
                {
                    if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.ConnectionStrings["uyum"].ProviderName))
                        providerName = System.Configuration.ConfigurationManager.ConnectionStrings["uyum"].ProviderName;
                    connString = System.Configuration.ConfigurationManager.ConnectionStrings["uyum"].ConnectionString;
                }

                if (providerFactory == null)
                {
                    providerFactory = DbProviderFactories.GetFactory(providerName);
                }

                conn = providerFactory.CreateConnection();
                conn.ConnectionString = connString;

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    connectionCount++;
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteFileLog(new StringBuilder().Append("Bağlantı hatası:").Append(ex.Message).Append("Detay:").Append(ex.StackTrace).ToString());
                //EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 999);
            }
            return conn;
        }
    }
}