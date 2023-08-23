using HotSaleServiceTables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace HotSaleSenfoniAppServer
{
    /// <summary>
    /// Summary description for HotSaleSenfoni
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class HotSaleSenfoni : System.Web.Services.WebService
    {
        private static string ConnectionState = "";

        private static OracleConnection connection2
        {
            get
            {
                OracleConnection oracleConnection = new OracleConnection();
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["uyum"].ConnectionString;
                    oracleConnection.ConnectionString = connectionString;
                    oracleConnection.Open();
                }
                catch (Exception)
                {
                    try
                    {
                        oracleConnection.Dispose();
                        oracleConnection = null;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();

                        oracleConnection = new OracleConnection();
                        string connectionString = ConfigurationManager.ConnectionStrings["uyum"].ConnectionString;
                        oracleConnection.ConnectionString = connectionString;
                        oracleConnection.Open();
                    }
                    catch (Exception ex)
                    {
                        ConnectionState = string.Concat(ex.Message, ",Detay:", ex.StackTrace);
                        Logger.WriteFileLog(ex);
                        EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 999);
                    }
                }
                return oracleConnection;
            }
        }


        [WebMethod(Description = "Servis bağlantısını test etmek için")]
        public string BaglantiTest(string userName, string password, string branchCode)
        {
            IDbConnection connection = null;
            try
            {
                //connection = ConnectionHelper.Instance.GetConnection();
                connection = ConnectionHelper.Instance.GetConnection();
                if (connection == null)
                {
                    return "Bağlantı nesnesi oluşturulamadı! connection == null " + ConnectionState;
                }
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    return "Bağlantı kurulamadı! connection.State !=  System.Data.ConnectionState.Open " + ConnectionState;
                }
                return "Bağlandı";
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 1);
                return ex.Message;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public string Login(Token token)
        {
            IDbConnection connection = null;
            try
            {
                //connection = ConnectionHelper.Instance.GetConnection();
                connection = ConnectionHelper.Instance.GetConnection();
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    return string.Format("0|Parametre bulunamadı. UserName : {0} Password : {1} BranchCode : {2}", (object)token.Username, (object)token.Password, (object)token.BranchCode);
                if (string.IsNullOrEmpty(userParameters.ProductWhouseCode))
                    return "0|Ürün depo girilmemiş. gün başı yapılamaz.";
                return "1|" + token.Username + "|" + token.Password;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 1);
                return ex.Message;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public string SendMessageAlarmPrice(string branch, string cariKod, string cariFiyGrupKod)
        {
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();

                var ret = Helper.GetEntitiesV2(branch, connection);
                if(ret != null && ret.Count>0)
                {
                    List<string> saticilar = new List<string>();
                    var saticis = ret.Where(e => (cariFiyGrupKod != "" && e.PriceListGroupCode.Equals(cariFiyGrupKod)) || ( cariKod != "" && e.EntityCode.Equals(cariKod)));
                    foreach(Entity e in saticis)
                    {
                        if (saticilar.IndexOf(e.SaticiKod) == -1) saticilar.Add(e.SaticiKod);
                    }
                    foreach (string str in saticilar)
                    {
                        MsgEx msg = new MsgEx
                        {
                            BranchCode = branch,
                            Username = str,
                            Message = "Güncelleme var!!"
                        };
                        var liste = Helper.GetUserTokens(msg, connection);
                        Helper.SendBulkMessage(liste, msg.Message);
                    }
                }
                
                return "ok";
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
                return "";
            }
        }

        public string SendMessageEx(MsgEx msg)
        {
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();

                var liste = Helper.GetUserTokens(msg, connection);
                return Helper.SendBulkMessage(liste, msg.Message);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
                return "";
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public string SendMessage(Msg msg)
        {
            return Helper.SendMessage(msg.Id, msg.Message);
        }

        public string SetNotify(Notify notify)
        {
            IDbConnection connection = null;
            try
            {
                //connection = ConnectionHelper.Instance.GetConnection();
                var token = Notify.ToToken(notify);
                connection = ConnectionHelper.Instance.GetConnection();

                Helper.RegisterToken(notify, connection);

                return "1|" + token.Username + "|" + token.Password;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 1);
                return ex.Message;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public DateTime GetDate()
        {
            return DateTime.Today;
        }

        [WebMethod]
        public BeginOfDayResult GetBeginOfDay(Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi)
        {
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();
                return Helper.GetBeginOfDay(token, loadingCardCode, stokTarihi, siparisSevkTarihi, connection);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
                return (BeginOfDayResult)null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public BeginOfDayResult GetBeginOfPrice(Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi)
        {
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();
                return Helper.GetBeginOfPrice(token, loadingCardCode, stokTarihi, siparisSevkTarihi, connection);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
                return (BeginOfDayResult)null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public BeginOfDayResult GetBeginOfItem(Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi)
        {
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();
                return Helper.GetBeginOfItem(token, loadingCardCode, stokTarihi, siparisSevkTarihi, connection);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
                return (BeginOfDayResult)null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public BeginOfDayResult GetBeginOfCampaign(Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi)
        {
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();
                return Helper.GetBeginOfCampaign(token, loadingCardCode, stokTarihi, siparisSevkTarihi, connection);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
                return (BeginOfDayResult)null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public string GetPasswords(Token token)
        {
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, ConnectionHelper.Instance.GetConnection());
                if (userParameters == null)
                    return string.Format("0|Parametre bulunamadı. UserName : {0} Password : {1} BranchCode : {2}", (object)token.Username, (object)token.Password, (object)token.BranchCode);

                return "1|" + userParameters.PdaAdminPassword + "|" + userParameters.PdaWhouseAdminPassword + "|" + userParameters.PdaVehicleTransferPassword;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
            }
            return (string)null;
        }

        [WebMethod]
        public SystemSettings GetSystemSettings(Token token)
        {
            try
            {
                return Helper.GetSystemSettings(token, ConnectionHelper.Instance.GetConnection());
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 14);
                return (SystemSettings)null;
            }
        }

        [WebMethod]
        public LoadingCard GetLoadingCard(Token token)
        {
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();
                return Helper.GetLoadingCards(token, connection);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 17);
                return (LoadingCard)null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public OnlineItem[] GetOnlineItem(Token token, string whouseCode, string[] itemCodes)
        {
            ////OracleConnection conn = (OracleConnection)null;
            IDbConnection connection = null;
            try
            {
                connection = ConnectionHelper.Instance.GetConnection();
                return Helper.GetOnlineItems(token, connection, whouseCode, itemCodes);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 18);
                return (OnlineItem[])null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        [WebMethod]
        public OnlineEntityBalance GetOnlineEntityBalance(Token token, string entityCode)
        {
            IDbConnection conn = null;
            //OracleConnection conn = (OracleConnection)null;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                return Helper.GetOnlineEntityBalances(token, conn, entityCode);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 19);
                return (OnlineEntityBalance)null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        [WebMethod]
        public OnlineEntityExtract[] GetOnlineEntityExtract(Token token, string entityCode)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                return Helper.GetOnlineEntityExtracts(token, conn, entityCode);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 20);
                return (OnlineEntityExtract[])null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        [WebMethod]
        public OnlineOrderD[] GetOnlineOrderD(Token token, string entityCode)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                return Helper.GetOnlineOrderDs(token, conn, entityCode);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 21);
                return (OnlineOrderD[])null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        [WebMethod]
        public OrderM[] GetOnlineOrders(Token token, string entityCode)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                return Helper.GetOnlineOrders(token, conn, entityCode);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 22);
                return (OrderM[])null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        [WebMethod]
        public OrderM GetOnlineOrderWithDetails(Token token, int orderMId)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                return Helper.GetOnlineOrderWithDetails(token, conn, orderMId);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 23);
                return (OrderM)null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        [WebMethod]
        public SalesPersonPerformance[] GetSalesPersonPerformance(Token token)
        {
            IDbConnection conn = null;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                return Helper.GetSalesPersonPerformance(token, conn);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 24);
                return (SalesPersonPerformance[])null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        [WebMethod]
        public EndOfDayResult SaveEndOfDay(Token token, EndOfDay endOfDay)
        {
            IDbConnection conn = null;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                return Helper.SaveEndOfDay(token, endOfDay, conn);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2);
                return (EndOfDayResult)null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        [WebMethod]
        public string SaveCashPayment(Token token, Payment[] paymentList)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            string str;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                if (paymentList.Length == 0)
                    return string.Empty;
                str = Helper.SaveCashPayment(token, conn, paymentList);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 31);
                str = ex.Message;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return str;
        }

        [WebMethod]
        public string SaveCreditCardPayment(Token token, Payment[] paymentList)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            string str;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                if (paymentList.Length == 0)
                    return string.Empty;
                str = Helper.SaveRealCreditPayment(token, conn, paymentList); // old 30.12.2022 SaveCreditCardPayment(token, conn, paymentList);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 32);
                str = ex.Message;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return str;
        }

        [WebMethod]
        public string SaveChequePayment(Token token, Payment[] paymentList)
        {
            return string.Empty;
        }

        [WebMethod]
        public string SaveDraftPayment(Token token, Payment[] paymentList)
        {
            return string.Empty;
        }

        [WebMethod]
        public string SaveOnlineOrder(Token token, OrderM[] orderMList)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            string str;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                if (orderMList.Length == 0)
                    return string.Empty;
                str = Helper.SaveOnlineOrders(token, conn, orderMList);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 37);
                return ex.Message;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return str;
        }

        [WebMethod]
        public string SaveInvoice(Token token, InvoiceM[] invoiceMList)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            string str;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                if (invoiceMList.Length == 0)
                    return string.Empty;
                str = Helper.SaveInvoice(token, conn, invoiceMList, false);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 29);
                return (string)null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return str;
        }

        [WebMethod]
        public string SaveWaybill(Token token, WaybillM[] waybillMList)
        {
            //OracleConnection conn = (OracleConnection)null;
            IDbConnection conn = null;
            string str;
            try
            {
                conn = ConnectionHelper.Instance.GetConnection();
                if (waybillMList.Length == 0)
                    return string.Empty;
                str = Helper.SaveWaybillM(token, conn, waybillMList, false);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 29);
                return (string)null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return str;
        }

    }
}
