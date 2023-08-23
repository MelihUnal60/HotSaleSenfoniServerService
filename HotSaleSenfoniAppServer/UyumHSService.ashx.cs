using System;
using System.Web;
using HotSaleServiceTables;
using System.IO;
using Newtonsoft.Json;
using HotSaleServiceTables.Request;

namespace HotSaleSenfoniAppServer
{
    /// <summary>
    /// Summary description for UyumHSService
    /// </summary>
    public class UyumHSService : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string value = string.Empty;
            var serv = new HotSaleSenfoni();

            try
            {
                string action = context.Request.QueryString["action"];
                var strJson = new StreamReader(context.Request.InputStream).ReadToEnd();
                if (!string.IsNullOrWhiteSpace(action))
                {
                    switch (action)
                    {
                        case "AlarmPrice":
                            {
                                // carikod={EntityCode}&cfiygrupkod={EntityPriceGrpCode}&branch={BranchCode}
                                string carikod = Array.IndexOf(context.Request.QueryString.AllKeys,"carikod") > -1 ? context.Request.QueryString["carikod"] : "";
                                string cfiygrupkod = Array.IndexOf(context.Request.QueryString.AllKeys, "cfiygrupkod") > -1 ?  context.Request.QueryString["cfiygrupkod"] : "";
                                string branch = Array.IndexOf(context.Request.QueryString.AllKeys, "branch") > -1 ?  context.Request.QueryString["branch"] : "";
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SendMessageAlarmPrice(branch, carikod, cfiygrupkod)
                                }.ToString();
                            }
                            break;
                        case "BaglantiTest":
                            {
                                var req = JsonConvert.DeserializeObject<BaglantiTestCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.BaglantiTest(
                                        req.userName,
                                        req.password,
                                        req.branchCode)
                                }.ToString();
                            }
                            break;
                        case "Login":
                            {
                                var req = JsonConvert.DeserializeObject<Token>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.Login(req)
                                }.ToString();
                            }
                            break;
                        case "Register":
                            {
                                var req = JsonConvert.DeserializeObject<Notify>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SetNotify(req)
                                }.ToString();
                            }
                            break;
                        case "SendMsg":
                            {
                                var req = JsonConvert.DeserializeObject<Msg>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SendMessage(req)
                                }.ToString();
                            }
                            break;
                        case "SendMsgEx":
                            {
                                var req = JsonConvert.DeserializeObject<MsgEx>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SendMessageEx(req)
                                }.ToString();
                            }
                            break;
                        case "GetDate":
                            {
                                value = new MethodResult<DateTime>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetDate()
                                }.ToString();
                            }
                            break;
                        case "GetBeginOfDay":
                            {
                                var req = JsonConvert.DeserializeObject<GetBeginOfDayCls>(strJson);
                                value = new MethodResult<BeginOfDayResult>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetBeginOfDay(
                                        req.token,
                                        req.loadingCardCode,
                                        req.stokTarihi,
                                        req.siparisSevkTarihi)
                                }.ToString();
                            }
                            break;
                        case "GetBeginOfPrice":
                            {
                                var req = JsonConvert.DeserializeObject<GetBeginOfDayCls>(strJson);
                                value = new MethodResult<BeginOfDayResult>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetBeginOfPrice(
                                        req.token,
                                        req.loadingCardCode,
                                        req.stokTarihi,
                                        req.siparisSevkTarihi)
                                }.ToString();
                            }
                            break;
                        case "GetBeginOfItem":
                            {
                                var req = JsonConvert.DeserializeObject<GetBeginOfDayCls>(strJson);
                                value = new MethodResult<BeginOfDayResult>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetBeginOfItem(
                                        req.token,
                                        req.loadingCardCode,
                                        req.stokTarihi,
                                        req.siparisSevkTarihi)
                                }.ToString();
                            }
                            break;
                        case "GetBeginOfCampaign":
                            {
                                var req = JsonConvert.DeserializeObject<GetBeginOfDayCls>(strJson);
                                value = new MethodResult<BeginOfDayResult>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetBeginOfCampaign(
                                        req.token,
                                        req.loadingCardCode,
                                        req.stokTarihi,
                                        req.siparisSevkTarihi)
                                }.ToString();
                            }
                            break;
                        case "GetPasswords":
                            {
                                var req = JsonConvert.DeserializeObject<Token>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetPasswords(req)
                                }.ToString();
                            }
                            break;
                        case "Password":
                            {
                                var req = JsonConvert.DeserializeObject<Notify>(strJson);
                                var result = serv.GetPasswords(req);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = result
                                }.ToString();

                                try
                                {
                                    if (result != null)
                                    {
                                        serv.SetNotify(req);
                                    }
                                }
                                catch { }
                            }
                            break;
                        case "GetSystemSettings":
                            {
                                var req = JsonConvert.DeserializeObject<Token>(strJson);
                                value = new MethodResult<SystemSettings>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetSystemSettings(req)
                                }.ToString();
                            }
                            break;
                        case "GetLoadingCard":
                            {
                                var req = JsonConvert.DeserializeObject<Token>(strJson);
                                value = new MethodResult<LoadingCard>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetLoadingCard(req)
                                }.ToString();
                            }
                            break;
                        case "GetOnlineItem":
                            {
                                var req = JsonConvert.DeserializeObject<GetOnlineItemCls>(strJson);
                                value = new MethodResult<OnlineItem[]>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetOnlineItem(req.token, req.whouseCode, req.itemCodes)
                                }.ToString();
                            }
                            break;
                        case "GetOnlineEntityBalance":
                            {
                                var req = JsonConvert.DeserializeObject<GetOnlineEntityBalanceCls>(strJson);
                                value = new MethodResult<OnlineEntityBalance>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetOnlineEntityBalance(req.token, req.entityCode)
                                }.ToString();
                            }
                            break;
                        case "GetOnlineEntityExtract":
                            {
                                var req = JsonConvert.DeserializeObject<GetOnlineEntityExtractCls>(strJson);
                                value = new MethodResult<OnlineEntityExtract[]>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetOnlineEntityExtract(req.token, req.entityCode)
                                }.ToString();
                            }
                            break;
                        case "GetOnlineOrderD":
                            {
                                var req = JsonConvert.DeserializeObject<GetOnlineOrderDCls>(strJson);
                                value = new MethodResult<OnlineOrderD[]>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetOnlineOrderD(req.token, req.entityCode)
                                }.ToString();
                            }
                            break;
                        case "GetOnlineOrders":
                            {
                                var req = JsonConvert.DeserializeObject<GetOnlineOrdersCls>(strJson);
                                value = new MethodResult<OrderM[]>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetOnlineOrders(req.token, req.entityCode)
                                }.ToString();
                            }
                            break;
                        case "GetOnlineOrderWithDetails":
                            {
                                var req = JsonConvert.DeserializeObject<GetOnlineOrderWithDetailsCls>(strJson);
                                value = new MethodResult<OrderM>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetOnlineOrderWithDetails(req.token, req.orderMId)
                                }.ToString();
                            }
                            break;
                        case "GetSalesPersonPerformance":
                            {
                                var req = JsonConvert.DeserializeObject<Token>(strJson);
                                value = new MethodResult<SalesPersonPerformance[]>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.GetSalesPersonPerformance(req)
                                }.ToString();
                            }
                            break;
                        case "SaveEndOfDay":
                            {
                                var req = JsonConvert.DeserializeObject<SaveEndOfDayCls>(strJson);
                                value = new MethodResult<EndOfDayResult>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveEndOfDay(req.token, req.endOfDay)
                                }.ToString();
                            }
                            break;
                        case "SaveCashPayment":
                            {
                                var req = JsonConvert.DeserializeObject<SaveCashPaymentCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveCashPayment(req.token, req.paymentList)
                                }.ToString();
                            }
                            break;
                        case "SaveCreditCardPayment":
                            {
                                var req = JsonConvert.DeserializeObject<SaveCreditCardPaymentCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveCreditCardPayment(req.token, req.paymentList)
                                }.ToString();
                            }
                            break;

                        case "SaveChequePayment":
                            {
                                var req = JsonConvert.DeserializeObject<SaveChequePaymentCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveChequePayment(req.token, req.paymentList)
                                }.ToString();
                            }
                            break;
                        case "SaveDraftPayment":
                            {
                                var req = JsonConvert.DeserializeObject<SaveDraftPaymentCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveDraftPayment(req.token, req.paymentList)
                                }.ToString();
                            }
                            break;
                        case "SaveOnlineOrder":
                            {
                                var req = JsonConvert.DeserializeObject<SaveOnlineOrderCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveOnlineOrder(req.token, req.orderMList)
                                }.ToString();
                            }
                            break;
                        case "SaveInvoice":
                            {
                                var req = JsonConvert.DeserializeObject<SaveInvoiceCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveInvoice(req.token, req.invoiceMList)
                                }.ToString();
                            }
                            break;
                        case "SaveWaybill":
                            {
                                var req = JsonConvert.DeserializeObject<SaveWaybillCls>(strJson);
                                value = new MethodResult<string>()
                                {
                                    Success = true,
                                    Message = "",
                                    Values = serv.SaveWaybill(req.token, req.waybillMList)
                                }.ToString();
                            }
                            break;
                    } 

                }
                else
                {
                    value = new MethodResult<string>()
                    {
                        Success = false,
                        Message = "Parametre gerekli (action)",
                        Values = ""
                    }.ToString();
                } 
            }
            catch (NullReferenceException nullexc)
            {
                value = new MethodResult<string>()
                {
                    Success = false,
                    Message = nullexc.Message,
                    Values = nullexc.StackTrace
                }.ToString();
            }
            catch (Exception exc)
            {
                value = new MethodResult<string>()
                {
                    Success = false,
                    Message = !exc.Message.Contains("ERROR_CODE") ? exc.Message : exc.Message + "|" + exc.InnerException.Message,
                    Values = exc.StackTrace
                }.ToString();
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(value);
            context.Response.End();
        }
        
 
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

}