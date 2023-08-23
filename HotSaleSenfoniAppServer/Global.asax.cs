using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace HotSaleSenfoniAppServer
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            #region db test

            //try
            //{
            //    var providerFactory = DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.Client");
            //    var conn = providerFactory.CreateConnection();

            //    Oracle.ManagedDataAccess.Client.OracleConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.17.37.92)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=UYUMTEST)));User Id=uyumsoft;Password=uyumsoft;");
            //    connection.Open();
            //}
            //catch (Exception exc)
            //{
            //    throw exc;
            //}

            //try
            //{
            //    var providerFactory = DbProviderFactories.GetFactory("Npgsql");
            //    var conn = providerFactory.CreateConnection();

            //    NpgsqlConnection connection = new NpgsqlConnection("Server=192.168.2.64;User Id=uyum;Password=1iu7gdS2;Database=uyumtest;Port=5432;Pooling = false;MinPoolSize = 0;CommandTimeout = 0;Timeout = 600;ConnectionIdleLifetime = 10;KeepAlive = 10;ApplicationName=WebService");
            //    connection.Open();
            //}
            //catch (Exception exc)
            //{
            //    throw exc;
            //}

            #endregion

            #region eksik alan aç
            try
            {
                //var providerFactory = DbProviderFactories.GetFactory("Npgsql");
                //var c = providerFactory.CreateConnection();

                IDbConnection conn = ConnectionHelper.Instance.GetConnection();
                IDbCommand command = conn.CreateCommand();

                if (conn is OracleConnection)
                {
                    command.CommandText = @"SELECT COUNT(*) ROW_COUNT FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = 'ZZ_USER_NOTIFY'";
                    object vcount = command.ExecuteScalar();
                    if (vcount != null && Convert.ToInt32(vcount) < 1)
                    {
                        try
                        {
                            command.CommandText = "CREATE SEQUENCE  \"UYUMSOFT\".\"ZZ_USER_NOTIFY_ID_SEQ\"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 1 NOCACHE  NOORDER  NOCYCLE  NOPARTITION";
                            command.ExecuteNonQuery();
                        }
                        catch (Exception except)
                        {
                            Logger.WriteFileLog(new StringBuilder().Append("CREATE SEQUENCE hatası:").Append(except.Message).Append("Detay:").Append(except.StackTrace).ToString());
                        }

                        try
                        {
                            command.CommandText = "CREATE TABLE \"UYUMSOFT\".\"ZZ_USER_NOTIFY\" (\r\n" +
                                      "\"ID\" NUMBER(*, 0) NOT NULL ENABLE,\r\n" +
                                      "\"BRANCH_CODE\" varchar(100),\r\n" +
                                      "\"USER_CODE\" NVARCHAR2(100),\r\n" +
                                      "\"PUSH_CODE\" NVARCHAR2(255),\r\n" +
                                      "\"CREATE_DATE\" DATE,\r\n" +
                                      "CONSTRAINT \"PK_ZZ_USER_NOTIFY\" PRIMARY KEY(\"ID\")\r\n" +
                                      ")\r\n";
                            command.ExecuteNonQuery();
                        }
                        catch (Exception except)
                        {
                            Logger.WriteFileLog(new StringBuilder().Append("CREATE TABLE hatası:").Append(except.Message).Append("Detay:").Append(except.StackTrace).ToString());
                        }

                        try
                        {
                            command.CommandText = "CREATE OR REPLACE TRIGGER UYUMSOFT.ZZ_USER_NOTIFY_EO BEFORE INSERT ON UYUMSOFT.ZZ_USER_NOTIFY FOR EACH ROW\r\n" +
                                "BEGIN SELECT UYUMSOFT.ZZ_USER_NOTIFY_ID_SEQ.nextval, CURRENT_DATE INTO :new.ID, :new.CREATE_DATE FROM DUAL; END;\r\n"; 
                            command.ExecuteNonQuery();
                        }
                        catch (Exception except)
                        {
                            Logger.WriteFileLog(new StringBuilder().Append("CREATE OR REPLACE TRIGGER hatası:").Append(except.Message).Append("Detay:").Append(except.StackTrace).ToString());
                        }

                        #region azer çalışan
                        /*
                        drop SEQUENCE "UYUMSOFT"."ZZ_USER_NOTIFY_ID_SEQ"
                        CREATE SEQUENCE  "UYUMSOFT"."ZZ_USER_NOTIFY_ID_SEQ"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 1 NOCACHE NOORDER  NOCYCLE NOKEEP  NOSCALE GLOBAL

                        drop TRIGGER UYUMSOFT.ZZ_USER_NOTIFY_EO
                        CREATE OR REPLACE EDITIONABLE TRIGGER "UYUMSOFT"."ZZ_USER_NOTIFY_EO"
                        BEFORE
                            INSERT ON UYUMSOFT.ZZ_USER_NOTIFY FOR EACH ROW
                        BEGIN SELECT UYUMSOFT.ZZ_USER_NOTIFY_ID_SEQ.nextval, CURRENT_DATE INTO :new.ID, :new.CREATE_DATE FROM DUAL; END;
                        */
                        #endregion
                    }
                }
                else if (conn is NpgsqlConnection)
                {
                    command.CommandText = "select count(*) from information_schema.columns where table_name = 'zz_user_notify'";
                    object vcount = command.ExecuteScalar();
                    if (vcount != null && Convert.ToInt32(vcount) < 1)
                    {
                        try
                        {
                            command.CommandText = "CREATE SEQUENCE \"uyumsoft\".\"zz_user_notify_id_seq\"\r\n" +
                                "INCREMENT 1\r\n" +
                                "MINVALUE  1\r\n" +
                                "MAXVALUE 99999999999999999\r\n" +
                                "START 1\r\n" +
                                "CACHE 1;\r\n";
                            command.ExecuteNonQuery();
                        }
                        catch (Exception except)
                        {
                            Logger.WriteFileLog(new StringBuilder().Append("CREATE SEQUENCE hatası:").Append(except.Message).Append("Detay:").Append(except.StackTrace).ToString());
                        }

                        try
                        {
                            command.CommandText = "CREATE TABLE \"uyumsoft\".\"zz_user_notify\"(\r\n" +
                                  "\"id\" int4 NOT NULL DEFAULT NEXTVAL('zz_user_notify_id_seq'::regclass),\r\n" +
                                  "\"branch_code\" varchar(100),\r\n" +
                                  "\"user_code\" varchar(100),\r\n" +
                                  "\"push_code\" varchar(255),\r\n" +
                                  "\"create_date\" timestamp DEFAULT now(),\r\n" +
                                  "PRIMARY KEY(\"id\")\r\n" +
                                  ");\r\n";
                            command.ExecuteNonQuery();
                        }
                        catch (Exception except)
                        {
                            Logger.WriteFileLog(new StringBuilder().Append("CREATE TABLE hatası:").Append(except.Message).Append("Detay:").Append(except.StackTrace).ToString());
                        }
                    }
                }

                command.Dispose();
                conn.Close();
            }
            catch (Exception except)
            {
                Logger.WriteFileLog(new StringBuilder().Append("Bağlantı hatası:").Append(except.Message).Append("Detay:").Append(except.StackTrace).ToString());
            }

            #endregion
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Logger.WriteFileLog(new StringBuilder().Append("Genel hata:").Append("Uygulama genel hata").ToString());
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.AllErrors.Length > 0)
                    {
                        for (int i = 0; i < HttpContext.Current.AllErrors.Length; i++)
                        {
                            Exception acException = HttpContext.Current.AllErrors[i];
                            if (acException != null)
                            {
                                if (acException.Message == "Dosya yok.")
                                {
                                    System.Diagnostics.Trace.WriteLine("----------------------------------------------->>!");
                                    System.Diagnostics.Trace.WriteLine(acException.Message);
                                    System.Diagnostics.Trace.WriteLine(HttpContext.Current.Request.Url.ToString());
                                    System.Diagnostics.Trace.WriteLine("----------------------------------------------->>!");
                                    System.Diagnostics.Trace.WriteLine(acException.StackTrace);
                                    Logger.WriteFileLog(new StringBuilder().Append("Genel hata:").Append(acException.Message).Append("Detay:").Append(acException.StackTrace).Append(",Path:").Append(HttpContext.Current.Request.Url.ToString()).ToString());
                                }
                                else
                                {
                                    System.Diagnostics.Trace.WriteLine("----------------------------------------------->>!");
                                    System.Diagnostics.Trace.WriteLine(acException.Message);
                                    System.Diagnostics.Trace.WriteLine("----------------------------------------------->>!");
                                    System.Diagnostics.Trace.WriteLine(acException.StackTrace);
                                    Logger.WriteFileLog(new StringBuilder().Append("Genel hata:").Append(acException.Message).Append("Detay:").Append(acException.StackTrace).Append(",Path:").Append(HttpContext.Current.Request.Url.ToString()).ToString());
                                }
                            }
                            Exception acInner = HttpContext.Current.AllErrors[i].InnerException;
                            if (acInner != null)
                            {
                                System.Diagnostics.Trace.WriteLine("----------------------------------------------->>!");
                                System.Diagnostics.Trace.WriteLine(acInner.Message);
                                System.Diagnostics.Trace.WriteLine("----------------------------------------------->>!");
                                System.Diagnostics.Trace.WriteLine(acInner.StackTrace);
                                Logger.WriteFileLog(new StringBuilder().Append("Genel hata:").Append(acInner.Message).Append("Detay:").Append(acInner.StackTrace).Append(",Path:").Append(HttpContext.Current.Request.Url.ToString()).ToString());
                            }
                        }
                    }
                    HttpContext.Current.Server.ClearError();
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Trace.WriteLine(exc.Message);
                Logger.WriteFileLog(new StringBuilder().Append("Genel hata:").Append(exc.Message).Append("Detay:").Append(exc.StackTrace).Append(",Path:").ToString());
                //throw exc;
            }
            finally
            {
                //System.Diagnostics.Process.Start(@"C:\Windows\System32\iisreset.exe");
                //System.Threading.Thread.Sleep(5000);
                //Response.Redirect(Request.RawUrl);
            }
        }
    }    
}