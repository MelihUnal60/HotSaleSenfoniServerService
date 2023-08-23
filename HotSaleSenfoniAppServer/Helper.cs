// Decompiled with JetBrains decompiler
// Type: HotSaleSenfoniAppServer.Helper
// Assembly: HotSaleSenfoniAppServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 519FC89C-358B-4B52-A31E-384C227EC04C
// Assembly location: D:\UyumProjects\SicakSatis\SicakSatisSenfoniService\bin\HotSaleSenfoniAppServer.dll

using HotSaleSenfoniAppServer.Senfoni;
using HotSaleServiceTables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace HotSaleSenfoniAppServer
{
    public class Helper
    {
        private const string USERS_PARAMETER = "UYUMSOFT.HSMD_USERS_PARAMETER";
        private const string BANK_KART = "UYUMSOFT.FIND_CO_BANK_ACC";
        private const string ROTA_TANIM = "UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_M";
        private const string CARI_ROTA = "UYUMSOFT.HSMD_ROUTE_ENTITY_REL_D";
        private const string CARI_KART = "UYUMSOFT.FIND_ENTITY";
        private const string FIYAT_DETAY = "UYUMSOFT.INVT_PRICE_LIST_D";
        private const string STOK_KART = "UYUMSOFT.INVD_ITEM";
        private const string FIRMA = "UYUMSOFT.GNLD_BRANCH";
        private const string PRINTSECTIONS = "UYUMSOFT.HSMT_PRINT_DESIGN";
        private const string SIPARIS_MASTER = "UYUMSOFT.HSMT_ORDER_M";
        private const string SIPARIS_DETAY = "UYUMSOFT.HSMT_ORDER_D";
        private const string HSM_STOK_MASTER = "UYUMSOFT.HSMT_ITEM_M";
        private const string HSM_STOK_DETAY = "UYUMSOFT.HSMT_ITEM_D";
        private const string HSM_FATURA_MASTER = "UYUMSOFT.HSMT_INVOICE_M";
        private const string HSM_FATURA_DETAY = "UYUMSOFT.HSMT_INVOICE_D";
        private const string HSM_PAYMENT = "UYUMSOFT.HSMT_CASH_PAYMENT";
        private const string HSM_GUN_SONU_MASTER = "UYUMSOFT.HSMT_END_OF_DAY_M";
        private const string HSM_GUN_SONU_DETAY = "UYUMSOFT.HSMT_END_OF_DAY_D";
        private const string NOTE = "El terminali tarafından oluşturuldu";

        public static IDbConnection Check(IDbConnection IDbConnection)
        {
            try
            {
                if (IDbConnection.State != System.Data.ConnectionState.Open)
                {
                    IDbConnection.Open();
                }
            }
            catch
            {

            }

            return IDbConnection;
        }

        private static Senfoni.GeneralSenfoniService GetGeneralSenfoniWebService()
        {
            Senfoni.GeneralSenfoniService senfoniWebService = new Senfoni.GeneralSenfoniService();
            string url = global::HotSaleSenfoniAppServer.Properties.Settings.Default.UyumUrl;
            if (string.IsNullOrWhiteSpace(url)) url = @"http://oralive.ofis.uyumcloud.com/WebService/General/GeneralSenfoniService.asmx";
            if (!url.ToLower().Contains("generalsenfoniservice"))
            {
                if (url.EndsWith("/")) url = string.Concat(url, "WebService/General/GeneralSenfoniService.asmx");
                else
                    url = string.Concat(url, "/WebService/General/GeneralSenfoniService.asmx");
            }
            senfoniWebService.Url = url;
            senfoniWebService.Timeout = 999999;
            return senfoniWebService;
        }

        internal static BeginOfDayResult GetBeginOfDay(HotSaleServiceTables.Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi, IDbConnection connection)
        {
            BeginOfDayResult beginOfDayResult = new BeginOfDayResult();
            beginOfDayResult.BeginOfDay = new BeginOfDay();
            try
            {
                var version = "1.81";
                /*if (token.VersionNo == null || token.VersionNo == "" || token.VersionNo != version)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Versiyon Uyuşmazlığı "+ version + "'e yükseltmeniz lazım! " ;
                    return beginOfDayResult;
                }*/

                try // zz_hotsale_userparemeters
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PdaUserParams = Helper.GetUserParameters(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Get User Param - " + ex.Message;
                    return beginOfDayResult;
                }
                
                try // zz_hotsale_bankacc
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.BankAccList = ((IEnumerable<BankAcc>)Helper.GetBankAccs(token, connection)).ToList<BankAcc>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Bank Acc - " + ex.Message;
                    return beginOfDayResult;
                }
                try // zz_hotsale_categories
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CategoryList = ((IEnumerable<HotSaleServiceTables.Category>)Helper.GetCategories(token, connection)).ToList<HotSaleServiceTables.Category>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Category - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityList = ((IEnumerable<Entity>)Helper.GetEntities(token, connection)).ToList<Entity>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list1 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.EntityCode)).ToList<string>();
                List<string> list2 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.PriceListGroupCode)).ToList<string>();
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.ItemList = ((IEnumerable<Item>)Helper.GetItems(token, connection)).ToList<Item>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Item - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list3 = beginOfDayResult.BeginOfDay.ItemList.Select<Item, string>((Func<Item, string>)(x => x.ItemCode)).ToList<string>(); // Turan-1
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityItemList = ((IEnumerable<EntityItem>)Helper.GetEntityItems(token, list1.ToArray(), list3.ToArray(), connection)).ToList<EntityItem>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteList = ((IEnumerable<EntityRoute>)Helper.GetEntityRoutes(token, connection)).ToList<EntityRoute>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.IadeNedeniList = ((IEnumerable<IadeNedeni>)Helper.GetIadeNedeniDefs(token, connection)).ToList<IadeNedeni>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Iade Nedeni - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.ItemBarcodeList = ((IEnumerable<ItemBarcode>)Helper.GetItemBarcodes(token, list3.ToArray(), connection)).ToList<ItemBarcode>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Item Barcode " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022 - depo stok burası
                    beginOfDayResult.BeginOfDay.ItemTransactionList = ((IEnumerable<ItemTransaction>)Helper.GetItemTransactions(token, connection, loadingCardCode, stokTarihi)).ToList<ItemTransaction>();  // ?
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Item Transaction - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.ItemUnitList = ((IEnumerable<ItemUnit>)Helper.GetItemUnits(token, list3.ToArray(), connection)).ToList<ItemUnit>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Item Unit - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.LoadingCard = Helper.GetLoadingCards(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Loading Card - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.OrderMList = ((IEnumerable<OrderM>)Helper.GetOrderM(token, connection, list1.ToArray(), siparisSevkTarihi, loadingCardCode)).ToList<OrderM>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Order M - " + ex.Message;
                    return beginOfDayResult;
                }
                List<int> list4 = beginOfDayResult.BeginOfDay.OrderMList.Select<OrderM, int>((Func<OrderM, int>)(x => x.ERPOrderMId)).ToList<int>();
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.OrderDList = ((IEnumerable<OrderD>)Helper.GetOrderD(token, connection, list4.ToArray(), siparisSevkTarihi, loadingCardCode)).ToList<OrderD>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Order D - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PriceListList = ((IEnumerable<PriceList>)Helper.GetEntityPriceList(token, list2.ToArray(), connection)).ToList<PriceList>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Price List - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.RouteList = ((IEnumerable<Route>)Helper.GetRoutes(token, connection)).ToList<Route>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Route  - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.SystemSettings = Helper.GetSystemSettings(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "System Settings  - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.WhouseList = Helper.GetOtherDepos(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Other Depots - " + ex.Message;
                    return beginOfDayResult;
                }
                beginOfDayResult.BeginOfDay.BatchProcessList = new List<BatchProcess>();
                beginOfDayResult.BeginOfDay.EntityDiscGroupList = new List<EntityDiscGroup>();
                beginOfDayResult.BeginOfDay.ItemDiscGroupList = new List<ItemDiscGroup>();
                beginOfDayResult.BeginOfDay.WaybillMList = new List<WaybillM>();
                beginOfDayResult.BeginOfDay.WaybillDList = new List<WaybillD>();
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.OrderInvoiceMList = Helper.GetOrderInvoiceMList(token, list1.ToArray(), connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Order Invoice M - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.OrderInvoiceDList = Helper.GetOrderInvoiceDList(token, list1.ToArray(), connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Order Invoice D - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DepositTransactionList = Helper.GetDepositTransactionList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Deposit Transaction - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DepositList = Helper.GetDeposits(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Deposit - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteItemList = Helper.GetEntityRouteItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try // 1
                {
                    connection = Check(connection); // 10.03.2022 -- a1
                    beginOfDayResult.BeginOfDay.CampaignList = Helper.GetCampaigns(token, connection); // ?2
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 04.05.2023
                    beginOfDayResult.BeginOfDay.CampaignExEntityList = Helper.GetCampaignExEntities(token, connection); // ?3
                    
                    /* geri aldık // 10.03.2022
                    List<CampaignExEntity> all = Helper.GetCampaignExEntities(token, connection); // ?3

                    // filtreleme ekledik, sadece kendi carilerine göre işlem yapacak - 20.06.2022
                    beginOfDayResult.BeginOfDay.CampaignExEntityList = all.
                                Where(x => beginOfDayResult.BeginOfDay.EntityRouteList.Any(y => y.EntityCode == x.EntityCode)).ToList();
                    */
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    List<CampaignInEntity> all = Helper.GetCampaignInEntities(token, connection);
                    beginOfDayResult.BeginOfDay.CampaignInEntityList = all;

                    // filtreleme ekledik, sadece kendi carilerine göre işlem yapacak - 20.06.2022
                    //beginOfDayResult.BeginOfDay.CampaignInEntityList = all.
                    //            Where(x => beginOfDayResult.BeginOfDay.EntityRouteList.Any(y => y.EntityCode == x.EntityCode)).ToList();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignInItemList = Helper.GetCampaignInItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignExItemList = Helper.GetCampaignExItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignResultItemList = Helper.GetCampaignResultItemList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Result Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DueDiscList = Helper.GetDueDiscList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "DueDisc Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.InvoiceMList = Helper.GetInvoiceMList(token, list1.ToArray(), connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Invoice M Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.SurveyTemplateMList = Helper.GetSurveyTemplateMList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Survey Template M Item - " + ex.Message;
                    return beginOfDayResult;
                }
                beginOfDayResult.Success = true;
            }
            catch (Exception ex)
            {
                beginOfDayResult.Success = false;
                beginOfDayResult.ErrorMessage = ex.Message;
            }
            return beginOfDayResult;
        }

        internal static BeginOfDayResult GetBeginOfPrice(HotSaleServiceTables.Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi, IDbConnection connection)
        {
            BeginOfDayResult beginOfDayResult = new BeginOfDayResult();
            beginOfDayResult.BeginOfDay = new BeginOfDay();
            try
            {
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PdaUserParams = Helper.GetUserParameters(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Get User Param - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityList = ((IEnumerable<Entity>)Helper.GetEntities(token, connection)).ToList<Entity>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list1 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.EntityCode)).ToList<string>();
                List<string> list2 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.PriceListGroupCode)).ToList<string>();
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.ItemList = ((IEnumerable<Item>)Helper.GetItems(token, connection)).ToList<Item>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Item - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list3 = beginOfDayResult.BeginOfDay.ItemList.Select<Item, string>((Func<Item, string>)(x => x.ItemCode)).ToList<string>(); // Turan-1
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityItemList = ((IEnumerable<EntityItem>)Helper.GetEntityItems(token, list1.ToArray(), list3.ToArray(), connection)).ToList<EntityItem>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Item - " + ex.Message;
                    return beginOfDayResult;
                }
                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteList = ((IEnumerable<EntityRoute>)Helper.GetEntityRoutes(token, connection)).ToList<EntityRoute>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route - " + ex.Message;
                    return beginOfDayResult;
                }*/
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PriceListList = ((IEnumerable<PriceList>)Helper.GetEntityPriceList(token, list2.ToArray(), connection)).ToList<PriceList>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Price List - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.SystemSettings = Helper.GetSystemSettings(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "System Settings  - " + ex.Message;
                    return beginOfDayResult;
                }
               
                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DepositList = Helper.GetDeposits(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Deposit - " + ex.Message;
                    return beginOfDayResult;
                }*/
                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteItemList = Helper.GetEntityRouteItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route Item - " + ex.Message;
                    return beginOfDayResult;
                }*/
                /*
                try // 1
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignList = Helper.GetCampaigns(token, connection); // ?2
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    List<CampaignExEntity> all = Helper.GetCampaignExEntities(token, connection); // ?3

                    // filtreleme ekledik, sadece kendi carilerine göre işlem yapacak - 20.06.2022
                    beginOfDayResult.BeginOfDay.CampaignExEntityList = all.
                                Where(x => beginOfDayResult.BeginOfDay.EntityRouteList.Any(y => y.EntityCode == x.EntityCode)).ToList();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    List<CampaignInEntity> all = Helper.GetCampaignInEntities(token, connection);

                    // filtreleme ekledik, sadece kendi carilerine göre işlem yapacak - 20.06.2022
                    beginOfDayResult.BeginOfDay.CampaignInEntityList = all.
                                Where(x => beginOfDayResult.BeginOfDay.EntityRouteList.Any(y => y.EntityCode == x.EntityCode)).ToList();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignInItemList = Helper.GetCampaignInItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignExItemList = Helper.GetCampaignExItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignResultItemList = Helper.GetCampaignResultItemList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Result Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DueDiscList = Helper.GetDueDiscList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "DueDisc Item - " + ex.Message;
                    return beginOfDayResult;
                }
                */
                beginOfDayResult.Success = true;
            }
            catch (Exception ex)
            {
                beginOfDayResult.Success = false;
                beginOfDayResult.ErrorMessage = ex.Message;
            }
            return beginOfDayResult;
        }

        internal static BeginOfDayResult GetBeginOfItem(HotSaleServiceTables.Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi, IDbConnection connection)
        {
            BeginOfDayResult beginOfDayResult = new BeginOfDayResult();
            beginOfDayResult.BeginOfDay = new BeginOfDay();
            try
            {
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PdaUserParams = Helper.GetUserParameters(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Get User Param - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityList = ((IEnumerable<Entity>)Helper.GetEntities(token, connection)).ToList<Entity>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list1 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.EntityCode)).ToList<string>();
                List<string> list2 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.PriceListGroupCode)).ToList<string>();
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.ItemList = ((IEnumerable<Item>)Helper.GetItems(token, connection)).ToList<Item>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Item - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list3 = beginOfDayResult.BeginOfDay.ItemList.Select<Item, string>((Func<Item, string>)(x => x.ItemCode)).ToList<string>(); // Turan-1
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityItemList = ((IEnumerable<EntityItem>)Helper.GetEntityItems(token, list1.ToArray(), list3.ToArray(), connection)).ToList<EntityItem>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Item - " + ex.Message;
                    return beginOfDayResult;
                }
                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteList = ((IEnumerable<EntityRoute>)Helper.GetEntityRoutes(token, connection)).ToList<EntityRoute>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PriceListList = ((IEnumerable<PriceList>)Helper.GetEntityPriceList(token, list2.ToArray(), connection)).ToList<PriceList>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Price List - " + ex.Message;
                    return beginOfDayResult;
                }*/
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.SystemSettings = Helper.GetSystemSettings(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "System Settings  - " + ex.Message;
                    return beginOfDayResult;
                }

                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DepositList = Helper.GetDeposits(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Deposit - " + ex.Message;
                    return beginOfDayResult;
                }*/
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteItemList = Helper.GetEntityRouteItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route Item - " + ex.Message;
                    return beginOfDayResult;
                }
                /*
                try // 1
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignList = Helper.GetCampaigns(token, connection); // ?2
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    List<CampaignExEntity> all = Helper.GetCampaignExEntities(token, connection); // ?3

                    // filtreleme ekledik, sadece kendi carilerine göre işlem yapacak - 20.06.2022
                    beginOfDayResult.BeginOfDay.CampaignExEntityList = all.
                                Where(x => beginOfDayResult.BeginOfDay.EntityRouteList.Any(y => y.EntityCode == x.EntityCode)).ToList();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    List<CampaignInEntity> all = Helper.GetCampaignInEntities(token, connection);

                    // filtreleme ekledik, sadece kendi carilerine göre işlem yapacak - 20.06.2022
                    beginOfDayResult.BeginOfDay.CampaignInEntityList = all.
                                Where(x => beginOfDayResult.BeginOfDay.EntityRouteList.Any(y => y.EntityCode == x.EntityCode)).ToList();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignInItemList = Helper.GetCampaignInItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignExItemList = Helper.GetCampaignExItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignResultItemList = Helper.GetCampaignResultItemList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Result Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DueDiscList = Helper.GetDueDiscList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "DueDisc Item - " + ex.Message;
                    return beginOfDayResult;
                }
                */
                beginOfDayResult.Success = true;
            }
            catch (Exception ex)
            {
                beginOfDayResult.Success = false;
                beginOfDayResult.ErrorMessage = ex.Message;
            }
            return beginOfDayResult;
        }

        internal static BeginOfDayResult GetBeginOfCampaign(HotSaleServiceTables.Token token, string loadingCardCode, string stokTarihi, string siparisSevkTarihi, IDbConnection connection)
        {
            BeginOfDayResult beginOfDayResult = new BeginOfDayResult();
            beginOfDayResult.BeginOfDay = new BeginOfDay();
            try
            {
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PdaUserParams = Helper.GetUserParameters(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Get User Param - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityList = ((IEnumerable<Entity>)Helper.GetEntities(token, connection)).ToList<Entity>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list1 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.EntityCode)).ToList<string>();
                List<string> list2 = beginOfDayResult.BeginOfDay.EntityList.Select<Entity, string>((Func<Entity, string>)(x => x.PriceListGroupCode)).ToList<string>();
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.ItemList = ((IEnumerable<Item>)Helper.GetItems(token, connection)).ToList<Item>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Item - " + ex.Message;
                    return beginOfDayResult;
                }
                List<string> list3 = beginOfDayResult.BeginOfDay.ItemList.Select<Item, string>((Func<Item, string>)(x => x.ItemCode)).ToList<string>(); // Turan-1
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityItemList = ((IEnumerable<EntityItem>)Helper.GetEntityItems(token, list1.ToArray(), list3.ToArray(), connection)).ToList<EntityItem>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Item - " + ex.Message;
                    return beginOfDayResult;
                }
                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteList = ((IEnumerable<EntityRoute>)Helper.GetEntityRoutes(token, connection)).ToList<EntityRoute>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route - " + ex.Message;
                    return beginOfDayResult;
                }*/
                /*
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.PriceListList = ((IEnumerable<PriceList>)Helper.GetEntityPriceList(token, list2.ToArray(), connection)).ToList<PriceList>();
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Price List - " + ex.Message;
                    return beginOfDayResult;
                }*/
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.SystemSettings = Helper.GetSystemSettings(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "System Settings  - " + ex.Message;
                    return beginOfDayResult;
                }

                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DepositList = Helper.GetDeposits(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Deposit - " + ex.Message;
                    return beginOfDayResult;
                }*/
                /*try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.EntityRouteItemList = Helper.GetEntityRouteItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Entity Route Item - " + ex.Message;
                    return beginOfDayResult;
                }*/
                
                try // 1
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignList = Helper.GetCampaigns(token, connection); // ?2
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    List<CampaignExEntity> all = Helper.GetCampaignExEntities(token, connection); // ?3

                    beginOfDayResult.BeginOfDay.CampaignExEntityList = all;
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    List<CampaignInEntity> all = Helper.GetCampaignInEntities(token, connection);

                    beginOfDayResult.BeginOfDay.CampaignInEntityList = all;
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Entity - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignInItemList = Helper.GetCampaignInItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign In Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignExItemList = Helper.GetCampaignExItems(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Ex Item - " + ex.Message;
                    return beginOfDayResult;
                }
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.CampaignResultItemList = Helper.GetCampaignResultItemList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "Campaign Result Item - " + ex.Message;
                    return beginOfDayResult;
                }
                /*
                try
                {
                    connection = Check(connection); // 10.03.2022
                    beginOfDayResult.BeginOfDay.DueDiscList = Helper.GetDueDiscList(token, connection);
                }
                catch (Exception ex)
                {
                    beginOfDayResult.Success = false;
                    beginOfDayResult.ErrorMessage = "DueDisc Item - " + ex.Message;
                    return beginOfDayResult;
                }
                */
                beginOfDayResult.Success = true;
            }
            catch (Exception ex)
            {
                beginOfDayResult.Success = false;
                beginOfDayResult.ErrorMessage = ex.Message;
            }
            return beginOfDayResult;
        }


        private static List<CampaignInEntity> GetCampaignInEntities(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<CampaignInEntity> campaignInEntityList = new List<CampaignInEntity>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                // 04.05.2023
                string str = string.Format("SELECT \r\n                                                        T.CAMPAIGN_ID ,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 1 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 2 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP1_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 3 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP2_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 4 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP3_CODE\r\n                                                        FROM HSMD_CAMPAIGN_IN_ENTITY T\r\n                                                        LEFT OUTER JOIN FINW_ENTITY_GROUP_TABLES FW ON FW.ENTITYORGROUPTYPE = T.ENTITY_OR_GROUP_TYPE \r\n                                                                                                   AND FW.ENTITY_OR_GROUP_TYPE_ID = T.ENTITY_OR_GROUP_TYPE_ID\r\n                                                        INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = T.BRANCH_ID\r\n                                                        INNER JOIN HSMD_CAMPAIGN C ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                        WHERE B.BRANCH_CODE = '{0}'\r\n                                                        AND C.IS_PASSIVE = 0\r\n                                                 ", (object)userParameters.BranchCode);
                // yenisi 04.05.2023 string message = string.Format("SELECT \r\n                                                        T.CAMPAIGN_ID ,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 1 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 2 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP1_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 3 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP2_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 4 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP3_CODE\r\n                                                        FROM HSMD_CAMPAIGN_IN_ENTITY T\r\n                                                        LEFT OUTER JOIN FINW_ENTITY_GROUP_TABLES FW ON FW.ENTITYORGROUPTYPE = T.ENTITY_OR_GROUP_TYPE \r\n                                                                                                   AND FW.ENTITY_OR_GROUP_TYPE_ID = T.ENTITY_OR_GROUP_TYPE_ID\r\n                                                        INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = T.BRANCH_ID\r\n                                                        INNER JOIN HSMD_CAMPAIGN C ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                        WHERE B.BRANCH_CODE = '{0}'\r\n                                                        AND C.IS_PASSIVE = 0\r\n                                                 ", (object)(Helper.GetUserParameters(token, connection) ?? throw new Exception("Invalid token")).BranchCode);

                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 5654);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = str;
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    CampaignInEntity campaignInEntity = new CampaignInEntity()
                    {
                        CampaignId = oracleDataReader[0].toInt(),
                        EntityCode = ConvertToString(oracleDataReader[1]),
                        EntityGroup1Code = ConvertToString(oracleDataReader[2]),
                        EntityGroup2Code = ConvertToString(oracleDataReader[3]),
                        EntityGroup3Code = ConvertToString(oracleDataReader[4])
                    };
                    campaignInEntityList.Add(campaignInEntity);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return campaignInEntityList;
        }

        private static List<OrderInvoiceD> GetOrderInvoiceDList(HotSaleServiceTables.Token token, string[] entityCodes, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OrderInvoiceD> orderInvoiceDList = new List<OrderInvoiceD>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                string inFilter = Helper._GenerateInFilter(entityCodes, "E.ENTITY_CODE");
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT D.ORDER_D_ID,D.ORDER_M_ID,\r\n                                    D.QTY - D.QTY_SHIPPING AS QTY,\r\n                                    D.UNIT_PRICE,\r\n                                    D.DISC1_RATE,\r\n                                    D.DISC2_RATE,\r\n                                    CASE WHEN  D.QTY > 0 THEN D.QTY_PRM - ((D.QTY_PRM / D.QTY) * D.QTY_SHIPPING) ELSE 0 END AS QTY_PRM,\r\n                                    U.UNIT_CODE,\r\n                                    CASE WHEN  D.QTY > 0 THEN D.QTY_PRM - ((D.QTY_PRM / D.QTY) * D.QTY_SHIPPING) ELSE 0 END AS QTY_PRM2,\r\n                                    I.ITEM_CODE\r\n                                FROM PSMT_ORDER_D D \r\n                                LEFT OUTER JOIN INVD_UNIT U ON U.UNIT_ID = D.UNIT_ID\r\n                                LEFT OUTER JOIN INVD_ITEM I ON I.ITEM_ID = D.ITEM_ID\r\n                                WHERE D.ORDER_M_ID IN (SELECT M.ORDER_M_ID\r\n                                                        FROM PSMT_ORDER_M M INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = M.BRANCH_ID\r\n                                                        LEFT OUTER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                                                        WHERE B.BRANCH_CODE = '{0}'\r\n                                                        AND M.PURCHASE_SALES = 2\r\n                                                        AND M.ORDER_STATUS = 1\r\n                                                        AND {1}\r\n                                                        AND EXISTS(SELECT 1 FROM PSMT_ORDER_D D WHERE D.ORDER_M_ID = M.ORDER_M_ID AND D.SHIPPING_DATE = TO_DATE('{2}','dd.MM.yyyy')))", (object)token.BranchCode, (object)inFilter, (object)DateTime.Today.ToString("dd.MM.yyyy"));
                oracleDataReader = oracleCommand.ExecuteReader();

                while (oracleDataReader.Read())
                {
                    OrderInvoiceD orderInvoiceD = new OrderInvoiceD()
                    {
                        SourceDId = ConvertToInt32(oracleDataReader[0]),
                        SourceMId = ConvertToInt32(oracleDataReader[1]),
                        SourceApp = "Siparis",
                        Qty = ConvertToDecimal(oracleDataReader[2]),
                        Price = ConvertToDecimal(oracleDataReader[3]),
                        DiscRate1 = ConvertToDecimal(oracleDataReader[4]),
                        DiscRate2 = ConvertToDecimal(oracleDataReader[5]),
                        QtyPrm = ConvertToDecimal(oracleDataReader[6]),
                        UnitCode = ConvertToString(oracleDataReader[7]),
                        UnitCode2 = ConvertToString(oracleDataReader[7]),
                        QtyPrm2 = ConvertToDecimal(oracleDataReader[8]),
                        ItemCode = ConvertToString(oracleDataReader[9]),
                        LoadingCardNo = string.Empty
                    };
                    orderInvoiceDList.Add(orderInvoiceD);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
                string str1 = string.Format("SELECT D.ITEM_D_ID,\r\n                              D.ITEM_M_ID,\r\n                              D.QTY,\r\n                              D.UNIT_PRICE,\r\n                              D.DISC1_RATE,\r\n                              D.DISC2_RATE,\r\n                              D.QTY_PRM,\r\n                              U.UNIT_CODE,\r\n                              D.QTY_PRM AS QTY_PRM2,\r\n                              I.ITEM_CODE\r\n                         FROM INVT_ITEM_D D  \r\n                        LEFT OUTER JOIN INVD_UNIT U ON U.UNIT_ID = D.UNIT_ID\r\n                        LEFT OUTER JOIN INVD_ITEM I ON I.ITEM_ID = D.ITEM_ID\r\n                        WHERE D.ITEM_M_ID IN (\r\n                        SELECT M.ITEM_M_ID\r\n                        FROM INVT_ITEM_M M INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = M.BRANCH_ID\r\n                        LEFT OUTER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                        LEFT OUTER JOIN FIND_SALES_PERSON SP ON SP.SALES_PERSON_ID = M.SALES_PERSON_ID\r\n                        WHERE B.BRANCH_CODE = '{0}'\r\n                        AND M.DOC_DATE = TO_DATE('{1}','dd.MM.yyyy')\r\n                        AND M.PURCHASE_SALES = 2\r\n                        AND SP.SALES_PERSON_CODE = '{2}'\r\n                        AND M.INVOICE_STATUS = 1\r\n                        AND {3} )", (object)token.BranchCode, (object)DateTime.Today.ToString("dd.MM.yyyy"), (object)userParameters.SalesPersonCode, (object)inFilter);
                EventLog.WriteEntry("Application", str1, EventLogEntryType.Information, 564);
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = str1;

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OrderInvoiceD orderInvoiceD = new OrderInvoiceD()
                    {
                        SourceDId = ConvertToInt32(oracleDataReader[0]),
                        SourceMId = ConvertToInt32(oracleDataReader[1]),
                        SourceApp = "İrsaliye",
                        Qty = ConvertToDecimal(oracleDataReader[2]),
                        Price = ConvertToDecimal(oracleDataReader[3]),
                        DiscRate1 = ConvertToDecimal(oracleDataReader[4]),
                        DiscRate2 = ConvertToDecimal(oracleDataReader[5]),
                        QtyPrm = ConvertToDecimal(oracleDataReader[6]),
                        UnitCode = ConvertToString(oracleDataReader[7]),
                        UnitCode2 = ConvertToString(oracleDataReader[7]),
                        QtyPrm2 = ConvertToDecimal(oracleDataReader[8]),
                        ItemCode = ConvertToString(oracleDataReader[9]),
                        LoadingCardNo = string.Empty
                    };
                    orderInvoiceDList.Add(orderInvoiceD);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
                string str2 = string.Format("SELECT D.INVOICE_D_ID,\r\n                              D.INVOICE_M_ID,\r\n                              D.QTY,\r\n                              D.UNIT_PRICE,\r\n                              D.DISC1_RATE,\r\n                              D.DISC2_RATE,\r\n                              D.QTY_PRM,\r\n                              U.UNIT_CODE,\r\n                              D.QTY_PRM AS QTY_PRM2,\r\n                              I.ITEM_CODE\r\n                        FROM PSMT_INVOICE_D D \r\n                        LEFT OUTER JOIN INVD_UNIT U ON U.UNIT_ID = D.UNIT_ID\r\n                        LEFT OUTER JOIN INVD_ITEM I ON I.ITEM_ID = D.ITEM_ID\r\n                        WHERE D.INVOICE_M_ID IN (\r\n                        SELECT M.INVOICE_M_ID\r\n                        FROM PSMT_INVOICE_M M INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = M.BRANCH_ID\r\n                        LEFT OUTER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                        LEFT OUTER JOIN FIND_SALES_PERSON SP ON SP.SALES_PERSON_ID = M.SALES_PERSON_ID\r\n                        WHERE B.BRANCH_CODE = '{0}'\r\n                        AND M.DOC_DATE = TO_DATE('{1}','dd.MM.yyyy')\r\n                        AND M.CARD_TYPE = 2\r\n                        AND M.PURCHASE_SALES = 2\r\n                        AND SP.SALES_PERSON_CODE = '{2}'\r\n                        AND M.SOURCE_APP != 1000\r\n                        AND {3})", (object)token.BranchCode, (object)DateTime.Today.ToString("dd.MM.yyyy"), (object)userParameters.SalesPersonCode, (object)inFilter);
                EventLog.WriteEntry("Application", str2, EventLogEntryType.Information, 564);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = str2;

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OrderInvoiceD orderInvoiceD = new OrderInvoiceD()
                    {
                        SourceDId = ConvertToInt32(oracleDataReader[0]),
                        SourceMId = ConvertToInt32(oracleDataReader[1]),
                        SourceApp = "Fatura",
                        Qty = ConvertToDecimal(oracleDataReader[2]),
                        Price = ConvertToDecimal(oracleDataReader[3]),
                        DiscRate1 = ConvertToDecimal(oracleDataReader[4]),
                        DiscRate2 = ConvertToDecimal(oracleDataReader[5]),
                        QtyPrm = ConvertToDecimal(oracleDataReader[6]),
                        UnitCode = ConvertToString(oracleDataReader[7]),
                        UnitCode2 = ConvertToString(oracleDataReader[7]),
                        QtyPrm2 = ConvertToDecimal(oracleDataReader[8]),
                        ItemCode = ConvertToString(oracleDataReader[9]),
                        LoadingCardNo = string.Empty
                    };
                    orderInvoiceDList.Add(orderInvoiceD);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
            }
            catch (Exception ex)
            {
                String ee = ex.Message;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return orderInvoiceDList;
        }

        private static List<OrderInvoiceM> GetOrderInvoiceMList(HotSaleServiceTables.Token token, string[] entityCodes, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OrderInvoiceM> orderInvoiceMList = new List<OrderInvoiceM>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                string inFilter = Helper._GenerateInFilter(entityCodes, "E.ENTITY_CODE");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT M.ORDER_M_ID,M.AMT_VAT,M.AMT_RECEIPT,M.AMT,M.AMT_DISC_TOTAL,M.DOC_DATE,M.DOC_NO, E.ENTITY_CODE,B.BRANCH_CODE\r\n                                FROM PSMT_ORDER_M M INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = M.BRANCH_ID\r\n                                LEFT OUTER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                                WHERE B.BRANCH_CODE = '{0}'\r\n                                AND M.PURCHASE_SALES = 2\r\n                                AND M.ORDER_STATUS = 1\r\n                                AND {1}\r\n                                AND EXISTS(SELECT 1 FROM PSMT_ORDER_D D WHERE D.ORDER_M_ID = M.ORDER_M_ID AND D.SHIPPING_DATE = TO_DATE('{2}','dd.MM.yyyy'))", (object)token.BranchCode, (object)inFilter, (object)DateTime.Today.ToString("dd.MM.yyyy"));

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OrderInvoiceM orderInvoiceM = new OrderInvoiceM()
                    {
                        SourceMId = ConvertToInt32(oracleDataReader[0]),
                        SourceApp = "Siparis",
                        AmtVat = ConvertToDecimal(oracleDataReader[1]),
                        AmtReceipt = ConvertToDecimal(oracleDataReader[2]),
                        Amt = ConvertToDecimal(oracleDataReader[3]),
                        AmtDisc = ConvertToDecimal(oracleDataReader[4]),
                        DocDate = ConvertToDateTime(oracleDataReader[5]),
                        DocNo = ConvertToString(oracleDataReader[6]),
                        EntityCode = ConvertToString(oracleDataReader[7]),
                        BranchCode = ConvertToString(oracleDataReader[8])
                    };
                    orderInvoiceMList.Add(orderInvoiceM);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT M.ITEM_M_ID,M.AMT_VAT,M.AMT_RECEIPT,M.AMT,M.AMT_DISC_TOTAL,M.DOC_DATE,M.DOC_NO, E.ENTITY_CODE,B.BRANCH_CODE\r\n                            FROM INVT_ITEM_M M INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = M.BRANCH_ID\r\n                            LEFT OUTER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                            LEFT OUTER JOIN FIND_SALES_PERSON SP ON SP.SALES_PERSON_ID = M.SALES_PERSON_ID\r\n                            WHERE B.BRANCH_CODE = '{0}'\r\n                            AND M.DOC_DATE = TO_DATE('{1}','dd.MM.yyyy')\r\n                            AND M.PURCHASE_SALES = 2\r\n                            AND SP.SALES_PERSON_CODE = '{2}'\r\n                            AND M.INVOICE_STATUS = 1\r\n                            AND {3}", (object)token.BranchCode, (object)DateTime.Today.ToString("dd.MM.yyyy"), (object)userParameters.SalesPersonCode, (object)inFilter);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OrderInvoiceM orderInvoiceM = new OrderInvoiceM()
                    {
                        SourceMId = ConvertToInt32(oracleDataReader[0]),
                        SourceApp = "İrsaliye",
                        AmtVat = ConvertToDecimal(oracleDataReader[1]),
                        AmtReceipt = ConvertToDecimal(oracleDataReader[2]),
                        Amt = ConvertToDecimal(oracleDataReader[3]),
                        AmtDisc = ConvertToDecimal(oracleDataReader[4]),
                        DocDate = ConvertToDateTime(oracleDataReader[5]),
                        DocNo = ConvertToString(oracleDataReader[6]),
                        EntityCode = ConvertToString(oracleDataReader[7]),
                        BranchCode = ConvertToString(oracleDataReader[8])
                    };
                    orderInvoiceMList.Add(orderInvoiceM);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT M.INVOICE_M_ID,M.AMT_VAT,M.AMT_RECEIPT,M.AMT,M.AMT_DISC_TOTAL,M.DOC_DATE,M.DOC_NO, E.ENTITY_CODE,B.BRANCH_CODE\r\n                        FROM PSMT_INVOICE_M M INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = M.BRANCH_ID\r\n                        LEFT OUTER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                        LEFT OUTER JOIN FIND_SALES_PERSON SP ON SP.SALES_PERSON_ID = M.SALES_PERSON_ID\r\n                        WHERE B.BRANCH_CODE = '{0}'\r\n                        AND M.DOC_DATE = TO_DATE('{1}','dd.MM.yyyy')\r\n                        AND M.CARD_TYPE = 2\r\n                        AND M.PURCHASE_SALES = 2\r\n                        AND SP.SALES_PERSON_CODE = '{2}'\r\n                        AND M.SOURCE_APP != 1000\r\n                        AND {3}", (object)token.BranchCode, (object)DateTime.Today.ToString("dd.MM.yyyy"), (object)userParameters.SalesPersonCode, (object)inFilter);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OrderInvoiceM orderInvoiceM = new OrderInvoiceM()
                    {
                        SourceMId = ConvertToInt32(oracleDataReader[0]),
                        SourceApp = "Fatura",
                        AmtVat = ConvertToDecimal(oracleDataReader[1]),
                        AmtReceipt = ConvertToDecimal(oracleDataReader[2]),
                        Amt = ConvertToDecimal(oracleDataReader[3]),
                        AmtDisc = ConvertToDecimal(oracleDataReader[4]),
                        DocDate = ConvertToDateTime(oracleDataReader[5]),
                        DocNo = ConvertToString(oracleDataReader[6]),
                        EntityCode = ConvertToString(oracleDataReader[7]),
                        BranchCode = ConvertToString(oracleDataReader[8])
                    };
                    orderInvoiceMList.Add(orderInvoiceM);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT M.HSM_ITEM_M_ID,{4}(M.AMT_VAT,0) AS AMT_VAT,{4}(M.AMT_RECEIPT,0) AS AMT_RECEIPT,{4}(M.AMT,0) AS AMT,{4}(M.AMT_DISC_TOTAL,0) AS AMT_DISC_TOTAL,M.DOC_DATE,M.DOC_NO, E.ENTITY_CODE,B.BRANCH_CODE\r\n                        FROM HSMT_ITEM_M M INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = M.BRANCH_ID\r\n                        LEFT OUTER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                        LEFT OUTER JOIN FIND_SALES_PERSON SP ON SP.SALES_PERSON_ID = M.SALES_PERSON_ID\r\n                        WHERE B.BRANCH_CODE = '{0}'\r\n                        AND M.DOC_DATE = TO_DATE('{1}','dd.MM.yyyy')\r\n                        AND SP.SALES_PERSON_CODE = '{2}'\r\n                        AND M.IS_DELETED = 0 AND M.IS_UPLOAD = 0\r\n                        AND {3}", (object)token.BranchCode, (object)DateTime.Today.ToString("dd.MM.yyyy"), (object)userParameters.SalesPersonCode, (object)inFilter, Helper.GetIsNullCommand(connection));

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OrderInvoiceM orderInvoiceM = new OrderInvoiceM()
                    {
                        SourceMId = ConvertToInt32(oracleDataReader[0]),
                        SourceApp = "İrsaliye",
                        AmtVat = ConvertToDecimal(oracleDataReader[1]),
                        AmtReceipt = ConvertToDecimal(oracleDataReader[2]),
                        Amt = ConvertToDecimal(oracleDataReader[3]),
                        AmtDisc = ConvertToDecimal(oracleDataReader[4]),
                        DocDate = ConvertToDateTime(oracleDataReader[5]),
                        DocNo = ConvertToString(oracleDataReader[6]),
                        EntityCode = ConvertToString(oracleDataReader[7]),
                        BranchCode = ConvertToString(oracleDataReader[8])
                    };
                    orderInvoiceMList.Add(orderInvoiceM);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return orderInvoiceMList;
        }

        private static List<DepositTransaction> GetDepositTransactionList(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<DepositTransaction> depositTransactionList = new List<DepositTransaction>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                string str = string.Format("select dc.deposit_card_code,e.entity_code, SUM(T.QTY_IN - T.QTY_OUT - T.QTY_WASTE) AS QTY_IN \r\n                            from hsmt_deposit_transaction t \r\n                            inner join gnld_branch br on br.branch_id = t.branch_id\r\n                            inner join hsmd_deposit_card dc on dc.deposit_card_id = t.deposit_card_id\r\n                            inner join find_entity e on e.entity_id = t.entity_id\r\n                            inner join find_sales_person sp on sp.sales_person_id = t.sales_person_id\r\n                            where t.doc_date <= CURRENT_DATE\r\n                            and br.branch_code = '{0}'\r\n                            and sp.sales_person_code = '{1}'\r\n                            GROUP BY e.entity_code,dc.deposit_card_code", (object)userParameters.BranchCode, (object)userParameters.SalesPersonCode);
                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 564);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = str;

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    DepositTransaction depositTransaction = new DepositTransaction()
                    {
                        BranchCode = token.BranchCode,
                        DepositCode = ConvertToString(oracleDataReader[0]),
                        EntityCode = ConvertToString(oracleDataReader[1]),
                        IsCession = true,
                        QtyDrop = ConvertToDecimal(oracleDataReader[2]),
                        QtyTake = Decimal.Zero,
                        RegionCode = userParameters.RegionCode,
                        WhouseCode = userParameters.VehicleWhouseCode
                    };
                    depositTransactionList.Add(depositTransaction);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return depositTransactionList;
        }

        private static List<Deposit> GetDeposits(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<Deposit> depositList = new List<Deposit>();
            try
            {
                if (Helper.GetUserParameters(token, connection) == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = "select t.deposit_card_code,t.deposit_name from hsmd_deposit_card t";

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    Deposit deposit = new Deposit()
                    {
                        DepositCode = ConvertToString(oracleDataReader[0]),
                        DepositDesc = ConvertToString(oracleDataReader[1])
                    };
                    depositList.Add(deposit);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return depositList;
        }

        private static List<EntityRouteItem> GetEntityRouteItems(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<EntityRouteItem> entityRouteItemList = new List<EntityRouteItem>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT E.ENTITY_CODE,\r\n                                        I.ITEM_CODE,\r\n                                        D.IS_SALES_ORDER,\r\n                                        D.IS_SALES_INVOICE,\r\n                                        D.IS_SALES_RETURN_INVOICE,\r\n                                        D.IS_SALES_WAYYBILL,\r\n                                        D.IS_SALES_RETURN_WAYBILL,\r\n                                        D.IS_ONE_TO_ONE\r\n                                    FROM HSMD_ROUTE_ITEM_D D\r\n                                    INNER JOIN HSMD_ROUTE_ITEM_M M ON M.ROUTE_ITEM_M_ID = D.ROUTE_ITEM_M_ID\r\n                                    INNER JOIN HSMD_SAL_PER_ROUTE_REL_D SRD ON SRD.ROUTE_ID = M.ROUTE_ID AND SRD.CO_ID = M.CO_ID AND SRD.BRANCH_ID = M.BRANCH_ID\r\n                                    INNER JOIN HSMD_SAL_PER_ROUTE_REL_M SRM ON SRM.SAL_PER_ROUTE_REL_M_ID = SRD.SAL_PER_ROUTE_REL_M_ID\r\n                                    INNER JOIN FIND_SALES_PERSON SP ON SP.SALES_PERSON_ID = SRM.SALES_PERSON_ID\r\n                                    INNER JOIN GNLD_BRANCH BR ON BR.BRANCH_ID = D.BRANCH_ID\r\n                                    INNER JOIN FIND_ENTITY E ON E.ENTITY_ID = M.ENTITY_ID\r\n                                    INNER JOIN INVD_ITEM I ON I.ITEM_ID = D.ITEM_ID\r\n                                 WHERE BR.BRANCH_CODE = '{0}' AND\r\n                                 SP.SALES_PERSON_CODE = '{1}'", (object)token.BranchCode, (object)userParameters.SalesPersonCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    EntityRouteItem entityRouteItem = new EntityRouteItem()
                    {
                        EntityCode = ConvertToString(oracleDataReader[0]),
                        ItemCode = ConvertToString(oracleDataReader[1]),
                        IsOrder = ConvertToBoolean(oracleDataReader[2]),
                        IsInvoice = ConvertToBoolean(oracleDataReader[3]),
                        IsWaybill = ConvertToBoolean(oracleDataReader[5]),
                        IsWaybillReturn = ConvertToBoolean(oracleDataReader[6]),
                        IsOneToOne = ConvertToBoolean(oracleDataReader[7])
                    };
                    entityRouteItemList.Add(entityRouteItem);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return entityRouteItemList;
        }

        private static List<SurveyTemplateM> GetSurveyTemplateMList(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<SurveyTemplateM> surveyTempateMList = new List<SurveyTemplateM>();
            List<SurveyTemplateD> source1 = new List<SurveyTemplateD>();
            List<SurveyTemplateAnswer> source2 = new List<SurveyTemplateAnswer>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT STM.SURVEY_TEMPLATE_M_ID,BR.BRANCH_ID,BR.BRANCH_CODE,STM.SURVEY_TEMPLATE_CODE,STM.DESCRIPTION,STM.SURVEY_TYPE\r\n                                    FROM HSMD_SURVEY_TEMPLATE_M STM INNER JOIN GNLD_BRANCH BR ON BR.BRANCH_ID = STM.BRANCH_ID\r\n                                    WHERE BR.BRANCH_CODE = '{0}'\r\n                                    AND STM.START_DATE <= CURRENT_DATE\r\n                                    AND STM.END_DATE >= CURRENT_DATE\r\n                                    AND STM.IS_PASSIVE = 0", (object)userParameters.BranchCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    SurveyTemplateM surveyTemplateM = new SurveyTemplateM()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        BranchId = ConvertToInt32(oracleDataReader[1]),
                        BranchCode = ConvertToString(oracleDataReader[2]),
                        SurveyTemplateCode = ConvertToString(oracleDataReader[3]),
                        Description = ConvertToString(oracleDataReader[4]),
                        SurveyType = ConvertToInt32(oracleDataReader[5])
                    };
                    surveyTempateMList.Add(surveyTemplateM);
                }
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
                string str1 = string.Format("SELECT STD.SURVEY_TEMPLATE_D_ID,\r\n                                                       STD.SURVEY_TEMPLATE_M_ID,\r\n                                                       STD.QUEST_NO,\r\n                                                       STD.DESCRIPTION,\r\n                                                       STD.QUEST_INPUT_TYPE,\r\n                                                       STD.IS_REQUIRED\r\n                                                  FROM HSMD_SURVEY_TEMPLATE_D STD\r\n                                                 WHERE {0}", (object)Helper._GenerateInFilter(surveyTempateMList.Select<SurveyTemplateM, int>((Func<SurveyTemplateM, int>)(x => x.Id)).ToList<int>().ToArray(), "STD.SURVEY_TEMPLATE_M_ID"));
                EventLog.WriteEntry("Application", str1, EventLogEntryType.Information, 565);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = str1;

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    SurveyTemplateD surveyTemplateD = new SurveyTemplateD()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        SurveyTemplateMId = ConvertToInt32(oracleDataReader[1]),
                        QuestNo = ConvertToInt32(oracleDataReader[2]),
                        Description = ConvertToString(oracleDataReader[3]),
                        QuestInputType = ConvertToInt32(oracleDataReader[4]),
                        IsRequired = ConvertToBoolean(oracleDataReader[5])
                    };
                    source1.Add(surveyTemplateD);
                }
                string inFilter = Helper._GenerateInFilter(source1.Select<SurveyTemplateD, int>((Func<SurveyTemplateD, int>)(x => x.Id)).ToList<int>().ToArray(), "STA.SURVEY_TEMPLATE_D_ID");
                oracleCommand.Dispose();
                oracleDataReader.Dispose();
                string str2 = string.Format("SELECT STA.SURVEY_TEMPLATE_ANSWER_ID,\r\n                                               STA.SURVEY_TEMPLATE_D_ID,\r\n                                               STA.SURVEY_TEMPLATE_M_ID,\r\n                                               STA.ANSWER_NO,\r\n                                               STA.ANSWER,\r\n                                               STA.DEPENDENCY_QUEST_NO\r\n                                          FROM HSMD_SURVEY_TEMPLATE_ANSWER STA\r\n                                         WHERE {0}", (object)inFilter);
                EventLog.WriteEntry("Application", str2, EventLogEntryType.Information, 566);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = str2;

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    SurveyTemplateAnswer surveyTemplateAnswer = new SurveyTemplateAnswer()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        SurveyTemplateDId = ConvertToInt32(oracleDataReader[1]),
                        SurveyTemplateMId = ConvertToInt32(oracleDataReader[2]),
                        AnswerNo = ConvertToString(oracleDataReader[3]),
                        Answer = ConvertToString(oracleDataReader[4]),
                        DependencyQuestNo = ConvertToInt32(oracleDataReader[5])
                    };
                    source2.Add(surveyTemplateAnswer);
                }
                for (int i = 0; i < surveyTempateMList.Count; i++)
                {
                    surveyTempateMList[i].SurveyTemplateDList = source1.Where<SurveyTemplateD>((Func<SurveyTemplateD, bool>)(x => x.SurveyTemplateMId == surveyTempateMList[i].Id)).ToList<SurveyTemplateD>();
                    for (int j = 0; j < surveyTempateMList[i].SurveyTemplateDList.Count; j++)
                        surveyTempateMList[i].SurveyTemplateDList[j].SurveyTemplateAnswerList = source2.Where<SurveyTemplateAnswer>((Func<SurveyTemplateAnswer, bool>)(x => x.SurveyTemplateDId == surveyTempateMList[i].SurveyTemplateDList[j].Id)).ToList<SurveyTemplateAnswer>();
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return surveyTempateMList;
        }

        private static List<InvoiceM> GetInvoiceMList(HotSaleServiceTables.Token token, string[] entityCodes, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<InvoiceM> invoiceMList = new List<InvoiceM>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                string inFilter = Helper._GenerateInFilter(entityCodes, "e.entity_code");
                if (userParameters.InvoiceStartDate == DateTime.MinValue)
                    return invoiceMList;

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("select t.invoice_m_id, \r\n                                                            t.doc_date,\r\n                                                            e.entity_name,\r\n                                                            t.amt_receipt ,\r\n                                                            CASE dt.purchase_sales \r\n                                                                 WHEN 1 THEN 'Alış' \r\n                                                                 WHEN 2 THEN 'Satış' \r\n                                                                 WHEN 3 THEN 'Satış İade' \r\n                                                                 WHEN 4 THEN 'Alış İade' END AS PurchaseSales,\r\n                                                            t.DOC_NO\r\n                                                            from psmt_invoice_m t \r\n                                                            inner join GNLD_BRANCH B ON B.BRANCH_ID = T.BRANCH_ID\r\n                                                            left outer join find_entity e on e.entity_id = t.entity_id\r\n                                                            left outer join gnld_doc_tra dt on dt.doc_tra_id = t.doc_tra_id\r\n                                                            WHERE B.BRANCH_CODE = '{0}'\r\n                                                            AND TO_DATE(TO_CHAR(T.DOC_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') >= TO_DATE('{1}','dd/MM/yyyy')\r\n                                                            AND t.PURCHASE_SALES IN (2,3) AND {2}", (object)userParameters.BranchCode, (object)userParameters.InvoiceStartDate.ToString("dd/MM/yyyy"), (object)inFilter);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    InvoiceM invoiceM = new InvoiceM()
                    {
                        InvoiceMId = ConvertToInt32(oracleDataReader[0]),
                        DocDate = ConvertToDateTime(oracleDataReader[1]),
                        EntityName = ConvertToString(oracleDataReader[2]),
                        AmtReceipt = ConvertToDecimal(oracleDataReader[3]),
                        PurchaseSales = ConvertToString(oracleDataReader[4]),
                        DocNo = ConvertToString(oracleDataReader[5])
                    };
                    invoiceMList.Add(invoiceM);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return invoiceMList;
        }

        private static List<DueDisc> GetDueDiscList(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<DueDisc> dueDiscList = new List<DueDisc>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT C.CO_CODE,\r\n                                                           I.ITEM_CODE,\r\n                                                           I.ITEM_NAME,\r\n                                                           DD.DUE_DISC_CODE,\r\n                                                           {1}(T.LINE_NO,0) AS LINE_NO,\r\n                                                           IDD.ITEM_DUE_DISC_GRP_CODE,\r\n                                                           {1}(D1.DISC_RATE,0)        AS DISC_RATE1,\r\n                                                           {1}(D2.DISC_RATE,0)        AS DISC_RATE2,\r\n                                                           {1}(T.DUE_DAY,0) AS DUE_DAY\r\n                                                      from UYUMSOFT.FIND_DUE_DISC_D T\r\n                                                     INNER JOIN FIND_DUE_DISC_M M\r\n                                                        ON M.DUE_DISC_M_ID = T.DUE_DISC_M_ID\r\n                                                     INNER JOIN FIND_DUE_DISC DD\r\n                                                        ON DD.DUE_DISC_ID = M.DUE_DISC_ID\r\n                                                     INNER JOIN GNLD_COMPANY C\r\n                                                        ON C.CO_ID = M.CO_ID\r\n                                                      LEFT OUTER JOIN INVD_ITEM I\r\n                                                        ON I.ITEM_ID = T.ITEM_ID\r\n                                                      LEFT OUTER JOIN INVD_ITEM_DUE_DISC_GRP IDD\r\n                                                        ON IDD.ITEM_DUE_DISC_GRP_ID = T.ITEM_DUE_DISC_GRP_ID\r\n                                                      LEFT OUTER JOIN FIND_DISC D1\r\n                                                        ON D1.DISC_ID = T.DISC1_ID\r\n                                                      LEFT OUTER JOIN FIND_DISC D2\r\n                                                        ON D2.DISC_ID = T.DISC2_ID\r\n                                                     WHERE C.CO_CODE = '{0}'\r\n                                                       AND (T.START_DATE <= CURRENT_DATE OR T.START_DATE IS NULL)\r\n                                                       AND (T.END_DATE >= CURRENT_DATE OR T.END_DATE IS NULL)\r\n                                                    ", (object)userParameters.CoCode, Helper.GetIsNullCommand(connection));

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    DueDisc dueDisc = new DueDisc()
                    {
                        CoCode = ConvertToString(oracleDataReader[0]),
                        ItemCode = ConvertToString(oracleDataReader[1]),
                        ItemName = ConvertToString(oracleDataReader[2]),
                        DueDiscCode = ConvertToString(oracleDataReader[3]),
                        LineNo = ConvertToInt32(oracleDataReader[4]),
                        ItemDueDiscCode = ConvertToString(oracleDataReader[5]),
                        DiscRate1 = ConvertToDecimal(oracleDataReader[6]),
                        DiscRate2 = ConvertToDecimal(oracleDataReader[7]),
                        DueDay = ConvertToInt32(oracleDataReader[8])
                    };
                    dueDiscList.Add(dueDisc);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return dueDiscList;
        }

        private static List<CampaignResultItem> GetCampaignResultItemList(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<CampaignResultItem> campaignResultItemList = new List<CampaignResultItem>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                // "SELECT T.CAMPAIGN_ID,\r\n                                                               T.LINE_NO,\r\n                                                               M.BRAND_CODE,\r\n                                                               C1.CATEGORIES_CODE,\r\n                                                               C2.CATEGORIES_CODE,\r\n                                                               C3.CATEGORIES_CODE,\r\n                                                               C4.CATEGORIES_CODE,\r\n                                                               ITM.ITEM_CODE,\r\n                                                               U.UNIT_CODE,\r\n                                                               T.QTY_PRM,\r\n                                                               T.IS_ALL,\r\n                                                               T.DISC_RATE\r\n                                                          FROM HSMD_CAMPAIGN_RESULT_ITEM T\r\n                                                         INNER JOIN HSMD_CAMPAIGN C\r\n                                                            ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                         INNER JOIN GNLD_BRANCH B\r\n                                                            ON B.BRANCH_ID = C.BRANCH_ID\r\n                                                          LEFT OUTER JOIN INVD_BRAND M\r\n                                                            ON M.BRAND_ID = T.BRAND_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C1\r\n                                                            ON C1.CATEGORIES_ID = T.CATEGORIES1_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C2\r\n                                                            ON C2.CATEGORIES_ID = T.CATEGORIES2_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C3\r\n                                                            ON C3.CATEGORIES_ID = T.CATEGORIES3_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C4\r\n                                                            ON C4.CATEGORIES_ID = T.CATEGORIES4_ID\r\n                                                          LEFT OUTER JOIN INVD_ITEM ITM\r\n                                                            ON ITM.ITEM_ID = T.ITEM_ID\r\n                                                          LEFT OUTER JOIN INVD_UNIT U\r\n                                                            ON U.UNIT_ID = T.UNIT_ID\r\n                                                         WHERE B.BRANCH_CODE = '{0}'\r\n                                                           AND C.IS_PASSIVE = 0\r\n                                                 "

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT T.CAMPAIGN_ID,\r\n                                                               T.LINE_NO,\r\n                                                               M.BRAND_CODE,\r\n                                                               C1.CATEGORIES_CODE,\r\n                                                               C2.CATEGORIES_CODE,\r\n                                                               C3.CATEGORIES_CODE,\r\n                                                               C4.CATEGORIES_CODE,\r\n                                                               ITM.ITEM_CODE,\r\n                                                               U.UNIT_CODE,\r\n                                                               T.QTY_PRM,\r\n                                                               T.IS_ALL,\r\n                                                               T.DISC_RATE\r\n                                                          FROM HSMD_CAMPAIGN_RESULT_ITEM T\r\n                                                         INNER JOIN HSMD_CAMPAIGN C\r\n                                                            ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                         INNER JOIN GNLD_BRANCH B\r\n                                                            ON B.BRANCH_ID = C.BRANCH_ID\r\n                                                          LEFT OUTER JOIN INVD_BRAND M\r\n                                                            ON M.BRAND_ID = T.BRAND_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C1\r\n                                                            ON C1.CATEGORIES_ID = T.CATEGORIES1_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C2\r\n                                                            ON C2.CATEGORIES_ID = T.CATEGORIES2_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C3\r\n                                                            ON C3.CATEGORIES_ID = T.CATEGORIES3_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C4\r\n                                                            ON C4.CATEGORIES_ID = T.CATEGORIES4_ID\r\n                                                          LEFT OUTER JOIN INVD_ITEM ITM\r\n                                                            ON ITM.ITEM_ID = T.ITEM_ID\r\n                                                          LEFT OUTER JOIN INVD_UNIT U\r\n                                                            ON U.UNIT_ID = T.UNIT_ID\r\n                                                         WHERE B.BRANCH_CODE = '{0}'\r\n                                                           AND C.IS_PASSIVE = 0\r\n                                                 ", (object)userParameters.BranchCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    CampaignResultItem campaignResultItem = new CampaignResultItem()
                    {
                        CampaignId = ConvertToInt32(oracleDataReader[0]),
                        LineNo = ConvertToInt32(oracleDataReader[1]),
                        BrandCode = ConvertToString(oracleDataReader[2]),
                        Categories1Code = ConvertToString(oracleDataReader[3]),
                        Categories2Code = ConvertToString(oracleDataReader[4]),
                        Categories3Code = ConvertToString(oracleDataReader[5]),
                        Categories4Code = ConvertToString(oracleDataReader[6]),
                        ItemCode = ConvertToString(oracleDataReader[7]),
                        UnitCode = ConvertToString(oracleDataReader[8]),
                        QtyPrm = ConvertToDecimal(oracleDataReader[9]),
                        IsAll = ConvertToBoolean(oracleDataReader[10]),
                        DiscRate = ConvertToDecimal(oracleDataReader[11])
                    };
                    campaignResultItemList.Add(campaignResultItem);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return campaignResultItemList;
        }

        private static List<CampaignExItem> GetCampaignExItems(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<CampaignExItem> campaignExItemList = new List<CampaignExItem>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT T.CAMPAIGN_ID,T.LINE_NO, I.ITEM_CODE\r\n                                                          FROM HSMD_CAMPAIGN_EX_ITEM T\r\n                                                         INNER JOIN INVD_ITEM I\r\n                                                            ON T.ITEM_ID = I.ITEM_ID\r\n                                                         INNER JOIN HSMD_CAMPAIGN C\r\n                                                            ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                         INNER JOIN GNLD_BRANCH B\r\n                                                            ON B.BRANCH_ID = C.BRANCH_ID\r\n                                                         WHERE B.BRANCH_CODE = '{0}'\r\n                                                           AND C.IS_PASSIVE = 0\r\n                                                 ", (object)userParameters.BranchCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    CampaignExItem campaignExItem = new CampaignExItem()
                    {
                        CampaignId = ConvertToInt32(oracleDataReader[0]),
                        LineNo = ConvertToInt32(oracleDataReader[1]),
                        ItemCode = ConvertToString(oracleDataReader[2])
                    };
                    campaignExItemList.Add(campaignExItem);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return campaignExItemList;
        }

        private static List<CampaignInItem> GetCampaignInItems(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<CampaignInItem> campaignInItemList = new List<CampaignInItem>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT T.CAMPAIGN_ID,\r\n                                                               T.LINE_NO,\r\n                                                               M.BRAND_CODE,\r\n                                                               C1.CATEGORIES_CODE,\r\n                                                               C2.CATEGORIES_CODE,\r\n                                                               C3.CATEGORIES_CODE,\r\n                                                               C4.CATEGORIES_CODE,\r\n                                                               ITM.ITEM_CODE,\r\n                                                               U.UNIT_CODE,\r\n                                                               {1}(T.QTY_PRM,0) AS QTY_PRM,\r\n                                                               {1}(T.IS_ALL,0) AS IS_ALL,\r\n                                                               {1}(T.AMT,0) AS AMT\r\n                                                          FROM HSMD_CAMPAIGN_IN_ITEM T\r\n                                                         INNER JOIN HSMD_CAMPAIGN C\r\n                                                            ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                         INNER JOIN GNLD_BRANCH B\r\n                                                            ON B.BRANCH_ID = C.BRANCH_ID\r\n                                                          LEFT OUTER JOIN INVD_BRAND M\r\n                                                            ON M.BRAND_ID = T.BRAND_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C1\r\n                                                            ON C1.CATEGORIES_ID = T.CATEGORIES1_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C2\r\n                                                            ON C2.CATEGORIES_ID = T.CATEGORIES2_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C3\r\n                                                            ON C3.CATEGORIES_ID = T.CATEGORIES3_ID\r\n                                                          LEFT OUTER JOIN GNLD_CATEGORIES C4\r\n                                                            ON C4.CATEGORIES_ID = T.CATEGORIES4_ID\r\n                                                          LEFT OUTER JOIN INVD_ITEM ITM\r\n                                                            ON ITM.ITEM_ID = T.ITEM_ID\r\n                                                          LEFT OUTER JOIN INVD_UNIT U\r\n                                                            ON U.UNIT_ID = T.UNIT_ID\r\n                                                         WHERE B.BRANCH_CODE = '{0}'\r\n                                                           AND C.IS_PASSIVE = 0\r\n                                                 ", 
                        (object)userParameters.BranchCode, 
                        Helper.GetIsNullCommand(connection)
                );

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    CampaignInItem campaignInItem = new CampaignInItem()
                    {
                        CampaignId = ConvertToInt32(oracleDataReader[0]),
                        LineNo = ConvertToInt32(oracleDataReader[1]),
                        BrandCode = ConvertToString(oracleDataReader[2]),
                        Categories1Code = ConvertToString(oracleDataReader[3]),
                        Categories2Code = ConvertToString(oracleDataReader[4]),
                        Categories3Code = ConvertToString(oracleDataReader[5]),
                        Categories4Code = ConvertToString(oracleDataReader[6]),
                        ItemCode = ConvertToString(oracleDataReader[7]),
                        UnitCode = ConvertToString(oracleDataReader[8]),
                        QtyPrm = ConvertToDecimal(oracleDataReader[9]),
                        IsAll = ConvertToBoolean(oracleDataReader[10]),
                        Amt = ConvertToDecimal(oracleDataReader[11])
                    };
                    campaignInItemList.Add(campaignInItem);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return campaignInItemList;
        }

        private static List<CampaignExEntity> GetCampaignExEntities(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<CampaignExEntity> campaignExEntityList = new List<CampaignExEntity>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                //oracleCommand.CommandText = string.Format("SELECT T.CAMPAIGN_ID, E.ENTITY_CODE\r\n                                                          FROM HSMD_CAMPAIGN_EX_ENTITY T\r\n                                                         INNER JOIN FIND_ENTITY E\r\n                                                            ON T.ENTITY_ID = E.ENTITY_ID\r\n                                                         INNER JOIN HSMD_CAMPAIGN C\r\n                                                            ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                         INNER JOIN GNLD_BRANCH B\r\n                                                            ON B.BRANCH_ID = C.BRANCH_ID\r\n                                                         WHERE B.BRANCH_CODE = '{0}'\r\n                                                           AND C.IS_PASSIVE = 0\r\n                                                 ", (object)userParameters.BranchCode), connection);

                oracleCommand = connection.CreateCommand();
                // old 05.04.2023
                //oracleCommand.CommandText = string.Format("SELECT DISTINCT CAMPAIGN_ID, ENTITY_CODE FROM ( SELECT T.CAMPAIGN_ID, E.ENTITY_CODE FROM HSMD_CAMPAIGN_EX_ENTITY T INNER JOIN FIND_ENTITY E ON T.ENTITY_OR_GROUP_TYPE_ID = E.ENTITY_ID AND T.ENTITY_OR_GROUP_TYPE = 1 INNER JOIN HSMD_CAMPAIGN C ON C.CAMPAIGN_ID = T.CAMPAIGN_ID INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = C.BRANCH_ID WHERE B.BRANCH_CODE = '{0}' AND C.IS_PASSIVE = 0 UNION ALL SELECT T.CAMPAIGN_ID, E.ENTITY_CODE FROM FIND_CO_ENTITY C1 INNER JOIN FIND_ENTITY E ON E.ENTITY_ID = C1.ENTITY_ID AND C1.ISPASSIVE = 0 INNER JOIN HSMD_CAMPAIGN_EX_ENTITY T ON T.ENTITY_OR_GROUP_TYPE_ID = C1.ENTITY_GRP_ID AND T.ENTITY_OR_GROUP_TYPE = 2 INNER JOIN HSMD_CAMPAIGN C ON C.CAMPAIGN_ID = T.CAMPAIGN_ID INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = C.BRANCH_ID WHERE B.BRANCH_CODE = '{0}' AND C.IS_PASSIVE = 0 UNION ALL SELECT T.CAMPAIGN_ID, E.ENTITY_CODE FROM FIND_CO_ENTITY C1 INNER JOIN FIND_ENTITY E ON E.ENTITY_ID = C1.ENTITY_ID AND C1.ISPASSIVE = 0 INNER JOIN HSMD_CAMPAIGN_EX_ENTITY T ON T.ENTITY_OR_GROUP_TYPE_ID = C1.ENTITY_GRP_ID AND T.ENTITY_OR_GROUP_TYPE = 3 INNER JOIN HSMD_CAMPAIGN C ON C.CAMPAIGN_ID = T.CAMPAIGN_ID INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = C.BRANCH_ID WHERE B.BRANCH_CODE = '{0}' AND C.IS_PASSIVE = 0 UNION ALL SELECT T.CAMPAIGN_ID, E.ENTITY_CODE FROM FIND_CO_ENTITY C1 INNER JOIN FIND_ENTITY E ON E.ENTITY_ID = C1.ENTITY_ID AND C1.ISPASSIVE = 0 INNER JOIN HSMD_CAMPAIGN_EX_ENTITY T ON T.ENTITY_OR_GROUP_TYPE_ID = C1.ENTITY_GRP_ID AND T.ENTITY_OR_GROUP_TYPE = 4 INNER JOIN HSMD_CAMPAIGN C ON C.CAMPAIGN_ID = T.CAMPAIGN_ID INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = C.BRANCH_ID WHERE B.BRANCH_CODE = '{0}' AND C.IS_PASSIVE = 0 ) X ORDER BY CAMPAIGN_ID, ENTITY_CODE", (object)userParameters.BranchCode);
                oracleCommand.CommandText = string.Format("SELECT \r\n                                                        T.CAMPAIGN_ID ,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 1 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 2 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP1_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 3 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP2_CODE,\r\n                                                        CASE WHEN T.ENTITY_OR_GROUP_TYPE = 4 THEN FW.ENTITY_OR_GROUP_TYPE_CODE ELSE NULL END AS ENTITY_GROUP3_CODE\r\n                                                        FROM HSMD_CAMPAIGN_EX_ENTITY T\r\n                                                        INNER JOIN HSMD_CAMPAIGN C ON C.CAMPAIGN_ID = T.CAMPAIGN_ID\r\n                                                        LEFT OUTER JOIN FINW_ENTITY_GROUP_TABLES FW ON FW.ENTITYORGROUPTYPE = T.ENTITY_OR_GROUP_TYPE \r\n                                                                                                   AND FW.ENTITY_OR_GROUP_TYPE_ID = T.ENTITY_OR_GROUP_TYPE_ID\r\n                                                        INNER JOIN GNLD_BRANCH B ON B.BRANCH_ID = C.BRANCH_ID\r\n                                                        WHERE B.BRANCH_CODE = '{0}'\r\n                                                        AND C.IS_PASSIVE = 0\r\n                                                 ", (object)(Helper.GetUserParameters(token, connection) ?? throw new Exception("Invalid token")).BranchCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    CampaignExEntity campaignExEntity = new CampaignExEntity()
                    {
                        CampaignId = ConvertToInt32(oracleDataReader[0]),
                        EntityCode = ConvertToString(oracleDataReader[1]),
                        // 05.04.2023
                        EntityGroup1Code = ConvertToString(oracleDataReader[2]), 
                        EntityGroup2Code = ConvertToString(oracleDataReader[3]),
                        EntityGroup3Code = ConvertToString(oracleDataReader[4])
                    };
                    campaignExEntityList.Add(campaignExEntity);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return campaignExEntityList;
        }

        private static List<Campaign> GetCampaigns(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<Campaign> campaignList = new List<Campaign>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT T.CAMPAIGN_ID,\r\n                                                               B.BRANCH_CODE,\r\n                                                               T.CAMPAIGN_CODE,\r\n                                                               T.DESCRIPTION AS CAMPAING_DESC,\r\n                                                               GRP1.ENTITY_GRP_CODE,\r\n                                                               GRP2.ENTITY_GRP_CODE,\r\n                                                               GRP3.ENTITY_GRP_CODE,\r\n                                                               T.IS_ALL,\r\n                                                               T.START_DATE,\r\n                                                               T.END_DATE,\r\n                                                               T.RESULT_ITEM_APPLY_SOURCE,\r\n                                                               T.NOTE,\r\n                                                               T.IS_RESET_OTHER_DISC\r\n                                                          FROM HSMD_CAMPAIGN T\r\n                                                         INNER JOIN GNLD_BRANCH B\r\n                                                            ON B.BRANCH_ID = T.BRANCH_ID\r\n                                                          LEFT OUTER JOIN FIND_ENTITY_GROUP GRP1\r\n                                                            ON GRP1.ENTITY_GRP_ID = T.ENTITY_GRP_ID\r\n                                                          LEFT OUTER JOIN FIND_ENTITY_GROUP GRP2\r\n                                                            ON GRP2.ENTITY_GRP_ID = T.ENTITY_GRP_ID2\r\n                                                          LEFT OUTER JOIN FIND_ENTITY_GROUP GRP3\r\n                                                            On GRP3.ENTITY_GRP_ID = T.ENTITY_GRP_ID3\r\n                                                         WHERE B.BRANCH_CODE = '{0}'\r\n                                                           AND T.IS_PASSIVE = 0\r\n                                                 ", (object)userParameters.BranchCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    campaignList.Add(new Campaign()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        CampaignId = ConvertToInt32(oracleDataReader[0]),
                        BranchCode = ConvertToString(oracleDataReader[1]),
                        CampaignCode = ConvertToString(oracleDataReader[2]),
                        CampaignDesc = ConvertToString(oracleDataReader[3]),
                        EntityGrpCode1 = ConvertToString(oracleDataReader[4]),
                        EntityGrpCode2 = ConvertToString(oracleDataReader[5]),
                        EntityGrpCode3 = ConvertToString(oracleDataReader[6]),
                        IsAll = ConvertToBoolean(oracleDataReader[7]),
                        StartDate = ConvertToDateTime(oracleDataReader[8]),
                        EndDate = ConvertToDateTime(oracleDataReader[9]),
                        ResultItemApplySource = ConvertToInt32(oracleDataReader[10]),
                        Note = ConvertToString(oracleDataReader[11]),
                        IsResetOtherDisc = ConvertToBoolean(oracleDataReader[12])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return campaignList;
        }

        internal static SalesPersonPerformance[] GetSalesPersonPerformance(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<SalesPersonPerformance> personPerformanceList = new List<SalesPersonPerformance>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("select t.Sales_Person_Name,\r\n                                                               t.Item_Code,\r\n                                                               t.Item_Name,\r\n                                                               t.Unit,\r\n                                                               t.Target_Amount,\r\n                                                               t.Actual_Amount,\r\n                                                               t.Amount_Variation,\r\n                                                               t.Target_Price,\r\n                                                               t.Actual_Price,\r\n                                                               t.Price_Variation\r\n                                                          from hsmv_salesperson_performance t\r\n                                                         WHERE t.Sales_Person_Code = '{0}'\r\n                                                           AND t.Branch_code = '{1}'\r\n                                                 ", (object)userParameters.SalesPersonCode, (object)userParameters.BranchCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    SalesPersonPerformance personPerformance = new SalesPersonPerformance()
                    {
                        SalesPersonName = ConvertToString(oracleDataReader[0]),
                        ItemCode = ConvertToString(oracleDataReader[1]),
                        ItemName = ConvertToString(oracleDataReader[2]),
                        UnitCode = ConvertToString(oracleDataReader[3]),
                        TargetQty = ConvertToString(oracleDataReader[4]),
                        RealQty = ConvertToString(oracleDataReader[5]),
                        QtyVariation = ConvertToString(oracleDataReader[6]),
                        TargetAmt = ConvertToString(oracleDataReader[7]),
                        RealAmt = ConvertToString(oracleDataReader[8]),
                        AmtVariation = ConvertToString(oracleDataReader[9])
                    };
                    personPerformanceList.Add(personPerformance);
                }
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return personPerformanceList.ToArray();
        }

        internal static EndOfDayResult SaveEndOfDay(HotSaleServiceTables.Token token, EndOfDay endOfDay, IDbConnection connection)
        {
            EndOfDayResult endOfDayResult = new EndOfDayResult();
            try
            {
                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveActivities(token, connection, endOfDay.ActivityList.ToArray());
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveActivities ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1000);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveActivities", EventLogEntryType.Information, 1000);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveCashPayment(token, connection, endOfDay.CashPaymentList.ToArray());
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveCashPayment ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1001);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveCashPayment", EventLogEntryType.Information, 1001);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveCreditPayment(token, connection, endOfDay.CreditCardPaymentList.ToArray() /*endOfDay.ChequePaymentList.ToArray()*/); // Helper.SaveCreditCardPayment(token, connection, endOfDay.CreditCardPaymentList.ToArray() /*endOfDay.ChequePaymentList.ToArray()*/);
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveCreditCardPayment ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1002);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveCreditCardPayment", EventLogEntryType.Information, 1002);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveEndOfDayItems(token, connection, endOfDay.EndOfDayItemsList.ToArray());
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveEndOfDayItems ERROR " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1003);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveEndOfDayItems", EventLogEntryType.Information, 1003);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveEntityCards(token, connection, endOfDay.EntityCardList.ToArray());
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveEntityCards ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1004);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveEntityCards", EventLogEntryType.Information, 1004);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveInvoice(token, connection, endOfDay.InvoiceMList.ToArray(), false);
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveInvoice ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1005);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveInvoice", EventLogEntryType.Information, 1005);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveOneToOne(token, connection, endOfDay.OneToOneMList.ToArray(), false);
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveOneToOne ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1006);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveOneToOne", EventLogEntryType.Information, 1006);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveOrders(token, connection, endOfDay.OrderMList.ToArray(), false);
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveOrders ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1007);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveOrders", EventLogEntryType.Information, 1007);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveWaybillM(token, connection, endOfDay.WaybillMList.ToArray(), false);
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveWaybillM ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1008);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveWaybillM", EventLogEntryType.Information, 1008);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveEntites(token, connection, endOfDay.EntityList.ToArray());
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveEntites ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1009);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveEntites", EventLogEntryType.Information, 1009);

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveSurveyAnswerM(token, connection, endOfDay.SurveyAnswerMList.ToArray());
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveSurveyAnswerM ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1010);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveSurveyAnswerM", EventLogEntryType.Information, 1010);

                connection = Check(connection); // 10.03.2022
                Helper.SaveEntityCheckins(token, connection, endOfDay.EntityCheckinInfoList.ToArray());
                EventLog.WriteEntry("Application", "SaveEntityCheckins : " + (object)endOfDay.EntityCheckinInfoList.Count, EventLogEntryType.Information, 1011);
                endOfDayResult.Success = true;

                connection = Check(connection); // 10.03.2022
                endOfDayResult.ErrorMessage = Helper.SaveDepositTransaction(token, connection, endOfDay.DepositTransactionList.ToArray());
                if (!string.IsNullOrEmpty(endOfDayResult.ErrorMessage))
                {
                    EventLog.WriteEntry("Application", "SaveDepositTransaction ERROR : " + endOfDayResult.ErrorMessage, EventLogEntryType.Error, 1012);
                    endOfDayResult.Success = false;
                    return endOfDayResult;
                }
                EventLog.WriteEntry("Application", "SaveDepositTransaction", EventLogEntryType.Information, 1012);

                connection = Check(connection); // 10.03.2022
                try
                {
                    endOfDayResult.ErrorMessage = Helper.SaveSystemLog(token, connection, endOfDay.LogList.ToArray());
                }
                catch { }
            }
            catch (Exception ex)
            {
                endOfDayResult.Success = false;
                endOfDayResult.ErrorMessage = ex.Message;
            }
            return endOfDayResult;
        }

        internal static void RegisterToken(HotSaleServiceTables.Notify notify, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            PdaUserParams pdaUserParams = (PdaUserParams)null;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                string str = string.Format("select COUNT(*) from UYUMSOFT.ZZ_USER_NOTIFY WHERE BRANCH_CODE = '{0}' AND USER_CODE = '{1}' AND PUSH_CODE = '{2}'", notify.BranchCode, notify.Username, notify.PushCode);
                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 23345);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(connection, str);
                object vcount = oracleCommand.ExecuteScalar();
                if (vcount != null && Convert.ToInt32(vcount) < 1)
                {
                    str = string.Format("INSERT INTO UYUMSOFT.ZZ_USER_NOTIFY (BRANCH_CODE, USER_CODE, PUSH_CODE) VALUES ('{0}', '{1}','{2}')", notify.BranchCode, notify.Username, notify.PushCode);
                    oracleCommand.CommandText = GetIsNullCommandReplace(connection, str);
                    oracleCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2334);
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
        }

        internal static PdaUserParams GetUserParameters(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            PdaUserParams pdaUserParams = (PdaUserParams)null;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                // U.US_NAME, U.US_SURNAME, U.NOTE2  added 29.01.2023
                string str = string.Format("SELECT\r\n                                                S.SALES_PERSON_CODE               AS SATICI_KOD,\r\n                                                A.ADMIN_PASSWORD                  AS YONETICI_SIFRE,\r\n                                                A.WHOUSE_MANAGER_PASSWORD         AS DEPO_YONETIIC_SIFRE,\r\n                                                A.VEHICLE_TRANS_PASSWORD          AS ARAC_TRANSFER_SIFRE,\r\n                                                W1.WHOUSE_CODE                    AS ARAC_DEPO_KOD,\r\n                                                W1.DESCRIPTION                    AS ARAC_DEPO_AD,\r\n                                                W2.WHOUSE_CODE                    AS BIR_E_BIR_DEPO_KOD,\r\n                                                W2.DESCRIPTION                    AS BIR_E_BIR_DEPO_AD,\r\n                                                W3.WHOUSE_CODE                    AS IADE_DEPO_KOD,\r\n                                                W4.WHOUSE_CODE                    AS BOSALTMA_DEPO_KOD,                                              \r\n                                                D1.DOC_TRA_CODE                   AS SATIS_FATURA_HAR_KOD,\r\n                                                D2.DOC_TRA_CODE                   AS SATIS_IRSALIYE_HAR_KOD,\r\n                                                D3.DOC_TRA_CODE                   AS SATIS_IADE_IRSALIYE_HAR_KOD,\r\n                                                D4.DOC_TRA_CODE                   AS ARAC_BOSALTMA_HAR_KOD,\r\n                                                D6.DOC_TRA_CODE                   AS ARAC_TRANSFER_HAREKET_KODU,\r\n                                                C1.CASH_BOX_CODE                  AS KASA_KOD,\r\n                                                B1.BANK_ACC_NO                    AS BANKA_HESA_NO1,\r\n                                                B2.BANK_ACC_NO                    AS BANKA_HESA_NO2,\r\n                                                B3.BANK_ACC_NO                    AS BANKA_HESA_NO3,\r\n                                                'YOK'                             AS CEK_POZISYON_KOD,\r\n                                                'YOK'                             AS SENET_POZISYON_KOD,\r\n                                                'YOK'                             AS CBORCLU_HESAP_KOD,\r\n                                                'YOK'                             AS SBORCLU_HESAP_KOD,\r\n                                                L1.LOADING_CARD_CODE              AS YUKLEME_CARD_NO1,\r\n                                                L2.LOADING_CARD_CODE              AS YUKLEME_CARD_NO2,\r\n                                                L3.LOADING_CARD_CODE              AS YUKLEME_CARD_NO3,\r\n                                                W5.WHOUSE_CODE                    AS URUN_DEPO_KOD,\r\n                                                W5.DESCRIPTION                    AS URUN_DEPO_AD,\r\n                                                RT.RECEIPT_TYPE_ID                AS BANK_RECEIPT_TYPE_ID,\r\n                                                RT2.RECEIPT_TYPE_ID               AS CASH_RECEIPT_TYPE_ID,\r\n                                                FT.TRA_TYPE_ID                    AS CREDIT_TRA_TYPE_ID,\r\n                                                D5.DOC_TRA_CODE                   AS ARAC_YUKLEME_HAR_KOD,\r\n                                                D7.DOC_TRA_CODE                   AS BIR_E_BIR_HAR_KOD,\r\n                                                D8.DOC_TRA_CODE                   AS SATIS_SIPARIS_HAR_KOD,\r\n                                                DS1.DISC_ID                       AS DISC_ID1,\r\n                                                DS2.DISC_ID                       AS DISC_ID2,\r\n                                                DS0.DISC_ID                       AS DISC_ID0,\r\n                                                A.IS_SAVE_REAL_ORDER,\r\n                                                A.IS_PAYMENT,\r\n                                                A.IS_SALES_INVOICE,\r\n                                                A.IS_SALES_WAYBILL,\r\n                                                A.IS_SALES_RETURN_WAYBILL,\r\n                                                A.IS_ONE_TO_ONE,\r\n                                                A.IS_SALES_ORDER,\r\n                                                A.IS_ACTIVITY_ENABLED,\r\n                                                A.IS_FAST_ORDER_ENABLED,\r\n                                                L4.LOADING_CARD_CODE              AS YUKLEME_CARD_NO4,\r\n                                                L5.LOADING_CARD_CODE              AS YUKLEME_CARD_NO5,\r\n                                                L6.LOADING_CARD_CODE              AS YUKLEME_CARD_NO6,\r\n                                                L1.DESCRIPTION                    AS YUKLEME_CARD_DESC1,\r\n                                                L2.DESCRIPTION                    AS YUKLEME_CARD_DESC2,\r\n                                                L3.DESCRIPTION                    AS YUKLEME_CARD_DESC3,\r\n                                                L4.DESCRIPTION                    AS YUKLEME_CARD_DESC4,\r\n                                                L5.DESCRIPTION                    AS YUKLEME_CARD_DESC5,\r\n                                                L6.DESCRIPTION                    AS YUKLEME_CARD_DESC6,\r\n                                                NVL(A.IS_SAVE_REAL_PAYMENT,0)     AS IS_SAVE_REAL_PAYMENT,\r\n                                                NVL(A.IS_SAVE_LOCATION_FOR_ENTITY,0) AS IS_SAVE_LOCATION_FOR_ENTITY,\r\n                                                NVL(A.IS_CONTROL_LOC_FOR_INVOICE,0) AS IS_CONTROL_LOC_FOR_INVOICE,\r\n                                                NVL(A.IS_CONTROL_LOC_FOR_ORDER,0) AS IS_CONTROL_LOC_FOR_ORDER,\r\n                                                NVL(A.IS_CONTROL_LOC_FOR_WAYBILL,0) AS IS_CONTROL_LOC_FOR_WAYBILL,\r\n                                                NVL(A.IS_CONTROL_LOC_FOR_ONE_TO_ONE,0) AS IS_CONTROL_LOC_FOR_ONE_TO_ONE,\r\n                                                NVL(A.IS_CONTROL_LOC_FOR_PAYMENT,0) AS IS_CONTROL_LOC_FOR_PAYMENT,\r\n                                                NVL(A.IS_CONTROL_LOC_FOR_ACTIVITY,0) AS IS_CONTROL_LOC_FOR_ACTIVITY,\r\n                                                NVL(A.ORDER_WAYBILL_PRICE_SOURCE,0) AS ORDER_WAYBILL_PRICE_SOURCE,\r\n                                                NVL(A.ORDER_INVOICE_PRICE_SOURCE,0) AS ORDER_INVOICE_PRICE_SOURCE,\r\n                                                NVL(A.WAYBILL_INVOICE_PRICE_SOURCE,0) AS WAYBILL_INVOICE_PRICE_SOURCE,\r\n                                                NVL(A.DISCOUNT_SOURCE1,0) AS DISCOUNT1_SOURCE,\r\n                                                NVL(A.DISCOUNT_SOURCE2,0) AS DISCOUNT2_SOURCE,\r\n                                                CO.CO_CODE,\r\n                                                CAT1.CAT_CODE AS CAT_CODE1,\r\n                                                CAT2.CAT_CODE AS CAT_CODE2,\r\n                                                CON.CONTACT_NAME,\r\n                                                D9.DOC_TRA_CODE                    AS SATIS_IADE_FATURA_HAR_KOD,\r\n                                                D10.DOC_TRA_CODE                   AS SATIS_IADE_SIPARIS_HAR_KOD,\r\n                                                A.IS_SAVE_REAL_INVOICE,\r\n                                                A.IS_SAVE_REAL_WAYBILL,\r\n                                                CUR.CUR_CODE,\r\n                                                A.INVOICE_START_DATE,\r\n                                                D11.DOC_TRA_CODE AS CONSIGNE_DOC_TRA_CODE,\r\n                                                W6.WHOUSE_CODE                    AS CONSIGNE_RETURN_WHOUSE_CODE,\r\n                                                W6.DESCRIPTION                    AS CONSIGNE_RETURN_WHOUSE_DESC,\r\n                                                CC.COST_CENTER_CODE ,\r\n                                                D12.DOC_TRA_CODE AS CONSIGNE_RETURN_DOC_TRA_CODE,\r\n                                                NVL(A.IS_USE_VAT_STATUS,0) AS IS_USE_VAT_STATUS,\r\n                                                NVL(A.IS_SAVE_REAL_ONETOONE,0) AS IS_SAVE_REAL_ONETOONE,\r\n NVL(A.IS_GET_QTY_FROM_LOADING_INS,0) AS IS_GET_QTY_FROM_LOADING_INS,\r\n " +
                    "A.IS_MOBILE_DNOTE_GIB_NO,\r\nA.IS_SALES_ORDER_RETURN,\r\nA.IS_SALES_RETURN_INVOICE\r\n, A.IS_PAYMENT_DEL, A.IS_SALES_INVOICE_DEL, A.IS_SALES_WAYBILL_DEL, A.IS_SALES_ORDER_DEL, A.IS_VEHICLE_ITEM, U.US_NAME, U.US_SURNAME, U.NOTE2\r\n, A.IS_PAYMENT_UPD\r\n, A.IS_SALES_INVOICE_UPD\r\n, A.IS_SALES_WAYBILL_UPD\r\n, A.IS_SALES_ORDER_UPD\r\n, A.IS_FIRST_PRICE_LIST\r\n FROM {0} A\r\n INNER JOIN UYUMSOFT.USERS U  ON U.US_ID = A.USR_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_BRANCH            BR ON BR.BRANCH_ID      = A.BRANCH_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_COMPANY           CO ON CO.CO_ID          = BR.CO_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_CURRENCY          CUR ON CUR.CUR_ID       = CO.CUR_ID\r\n                                                   INNER JOIN UYUMSOFT.INVD_WHOUSE            W1 ON W1.WHOUSE_ID      = A.VEHICLE_WHOUSE_ID\r\n                                                   INNER JOIN UYUMSOFT.INVD_WHOUSE            W2 ON W2.WHOUSE_ID      = A.ONE_TO_ONE_WHOUSE_ID\r\n                                                   INNER JOIN UYUMSOFT.INVD_WHOUSE            W3 ON W3.WHOUSE_ID      = A.VEHICLE_RETURN_WHOUSE_ID\r\n                                                   INNER JOIN UYUMSOFT.INVD_WHOUSE            W4 ON W4.WHOUSE_ID      = A.VEHICLE_UNLOAD_WHOUSE_ID\r\n                                                   INNER JOIN UYUMSOFT.INVD_WHOUSE            W5 ON W5.WHOUSE_ID      = A.PRODUCT_WHOUSE_ID\r\n                                                   INNER JOIN UYUMSOFT.INVD_WHOUSE            W6 ON W6.WHOUSE_ID      = A.CONSIGNE_RETURN_WHOUSE_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D1 ON D1.DOC_TRA_ID     = A.INVOICE_SALES_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D2 ON D2.DOC_TRA_ID     = A.WAYBILL_SALES_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D3 ON D3.DOC_TRA_ID     = A.WAYBILL_SALES_RETURN_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D4 ON D4.DOC_TRA_ID     = A.VEHICLE_UNLOAD_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D5 ON D5.DOC_TRA_ID     = A.VEHICLE_LOAD_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D6 ON D6.DOC_TRA_ID     = A.VEHICLE_TRANS_DOC_TRA_ID                                                   \r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D7 ON D7.DOC_TRA_ID     = A.ONE_TO_ONE_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D8 ON D8.DOC_TRA_ID     = A.ORDER_SALES_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D9 ON D9.DOC_TRA_ID     = A.INVOICE_SALES_RETURN_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D10 ON D10.DOC_TRA_ID   = A.ORDER_SALES_RETURN_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_DOC_TRA           D11 ON D11.DOC_TRA_ID   = A.CONSIGNE_DOC_TRA_ID\r\n                                                   INNER JOIN UYUMSOFT.FIND_CASH_BOX          C1 ON C1.CASH_BOX_ID    = A.CASH_BOX_ID \r\n                                                   INNER JOIN UYUMSOFT.FIND_BANK_ACC          B1 ON B1.BANK_ACC_ID    = A.BANK_ACC_ID1\r\n                                                   INNER JOIN UYUMSOFT.FIND_BANK_ACC          B2 ON B2.BANK_ACC_ID    = A.BANK_ACC_ID2\r\n                                                   INNER JOIN UYUMSOFT.FIND_BANK_ACC          B3 ON B3.BANK_ACC_ID    = A.BANK_ACC_ID3\r\n                                                   INNER JOIN UYUMSOFT.FIND_SALES_PERSON      S  ON S.SALES_PERSON_ID = A.SALES_PERSON_ID AND S.CO_ID = A.CO_ID\r\n                                                   INNER JOIN GNLD_CONTACT CON ON CON.CONTACT_ID = S.CONTACT_ID\r\n                                                   LEFT OUTER JOIN UYUMSOFT.HSMD_LOADING_CARD     L1 ON L1.LOADING_CARD_ID = A.LOADING_CARD_ID1\r\n                                                   LEFT OUTER JOIN UYUMSOFT.HSMD_LOADING_CARD     L2 ON L2.LOADING_CARD_ID = A.LOADING_CARD_ID2\r\n                                                   LEFT OUTER JOIN UYUMSOFT.HSMD_LOADING_CARD     L3 ON L3.LOADING_CARD_ID = A.LOADING_CARD_ID3\r\n                                                   LEFT OUTER JOIN UYUMSOFT.HSMD_LOADING_CARD     L4 ON L4.LOADING_CARD_ID = A.LOADING_CARD_ID4\r\n                                                   LEFT OUTER JOIN UYUMSOFT.HSMD_LOADING_CARD     L5 ON L5.LOADING_CARD_ID = A.LOADING_CARD_ID5\r\n                                                   LEFT OUTER JOIN UYUMSOFT.HSMD_LOADING_CARD     L6 ON L6.LOADING_CARD_ID = A.LOADING_CARD_ID6\r\n                                                   INNER JOIN UYUMSOFT.FIND_RECEIPT_TYPE     RT  ON RT.RECEIPT_TYPE_ID = A.BANK_RECEIPT_TYPE_ID\r\n                                                   INNER JOIN UYUMSOFT.FIND_RECEIPT_TYPE     RT2 ON RT2.RECEIPT_TYPE_ID = A.CASH_RECEIPT_TYPE_ID\r\n                                                   INNER JOIN UYUMSOFT.FIND_TRA_TYPE         FT  ON FT.TRA_TYPE_ID      = A.CREDIT_TRA_TYPE_ID\r\n                                                   INNER JOIN UYUMSOFT.FIND_DISC             DS1 ON DS1.DISC_ID         = A.DISC_ID1\r\n                                                   INNER JOIN UYUMSOFT.FIND_DISC             DS2 ON DS2.DISC_ID         = A.DISC_ID2\r\n                                                   INNER JOIN UYUMSOFT.FIND_DISC             DS0 ON DS0.DISC_ID         = A.DISC_ID0\r\n                                                   LEFT OUTER JOIN GNLD_CATEGORY CAT1 ON CAT1.CAT_CODE_ID = A.CAT_CODE1_ID\r\n                                                   LEFT OUTER JOIN GNLD_CATEGORY CAT2 ON CAT2.CAT_CODE_ID = A.CAT_CODE2_ID\r\n                                                   LEFT OUTER JOIN FIND_COST_CENTER CC ON CC.COST_CENTER_ID = A.COST_CENTER_ID\r\n                                                   LEFT JOIN UYUMSOFT.GNLD_DOC_TRA           D12 ON D12.DOC_TRA_ID   = A.CONSIGNE_RETURN_DOC_TRA_ID\r\n                                                 WHERE    U.US_USERNAME    = '{1}'\r\n                                                          AND A.PASSWORD   = '{2}' \r\n                                                          AND BR.BRANCH_CODE = '{3}'\r\n                                                 ", (object)"UYUMSOFT.HSMD_USERS_PARAMETER", (object)token.Username, (object)token.Password, (object)token.BranchCode, Helper.GetIsNullCommand(connection));
                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 23345);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(connection, str);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read() && pdaUserParams == null)
                {
                    pdaUserParams = new PdaUserParams()
                    {
                        SalesPersonCode = ConvertToString(oracleDataReader[0]),
                        PdaAdminPassword = ConvertToString(oracleDataReader[1]),
                        PdaWhouseAdminPassword = ConvertToString(oracleDataReader[2]),
                        PdaVehicleTransferPassword = ConvertToString(oracleDataReader[3]),
                        VehicleWhouseCode = ConvertToString(oracleDataReader[4]),
                        VehicleWhouseDesc = ConvertToString(oracleDataReader[5]),
                        OneToOneWhouseCode = ConvertToString(oracleDataReader[6]),
                        OneToOneWhouseDesc = ConvertToString(oracleDataReader[7]),
                        VehicleReturnWhouseCode = ConvertToString(oracleDataReader[8]),
                        VehicleUnloadingWhouseCode = ConvertToString(oracleDataReader[9]),
                        SalesInvoiceDocTraCode = ConvertToString(oracleDataReader[10]),
                        SalesWaybillDocTraCode = ConvertToString(oracleDataReader[11]),
                        SalesReturnWaybillDocTraCode = ConvertToString(oracleDataReader[12]),
                        UnloadingDocTraCode = ConvertToString(oracleDataReader[13]),
                        ItemTransactionDocTraCode = ConvertToString(oracleDataReader[14]),
                        CashBoxCode = ConvertToString(oracleDataReader[15]),
                        BankAccCode1 = ConvertToString(oracleDataReader[16]),
                        BankAccCode2 = ConvertToString(oracleDataReader[17]),
                        BankAccCode3 = ConvertToString(oracleDataReader[18]),
                        ChequePositionCode = ConvertToString(oracleDataReader[19]),
                        DraftPositionCode = ConvertToString(oracleDataReader[20]),
                        EntityDebitAccCode = ConvertToString(oracleDataReader[21]),
                        DraftDebitAccCode = ConvertToString(oracleDataReader[22]),
                        LoadingCardNo1 = ConvertToString(oracleDataReader[23]),
                        LoadingCardNo2 = ConvertToString(oracleDataReader[24]),
                        LoadingCardNo3 = ConvertToString(oracleDataReader[25]),
                        ProductWhouseCode = ConvertToString(oracleDataReader[26]),
                        ProductWhouseDesc = ConvertToString(oracleDataReader[27]),
                        BankReceiptTypeId = ConvertToInt32(oracleDataReader[28]),
                        CashReceiptTypeId = ConvertToInt32(oracleDataReader[29]),
                        TraTypeId = ConvertToInt32(oracleDataReader[30]),
                        VehicleLoadDocTraCode = ConvertToString(oracleDataReader[31]),
                        OneToOneDocTraCode = ConvertToString(oracleDataReader[32]),
                        OrderSaleDocTraCode = ConvertToString(oracleDataReader[33]),
                        DiscId1 = ConvertToInt32(oracleDataReader[34]),
                        DiscId2 = ConvertToInt32(oracleDataReader[35]),
                        DiscId0 = ConvertToInt32(oracleDataReader[36]),
                        IsSaveRealOrder = ConvertToBoolean(oracleDataReader[37]),
                        IsPayment = ConvertToBoolean(oracleDataReader[38]),
                        IsSalesInvoice = ConvertToBoolean(oracleDataReader[39]),
                        IsSalesWaybill = ConvertToBoolean(oracleDataReader[40]),
                        IsSalesReturnWaybill = ConvertToBoolean(oracleDataReader[41]),
                        IsOneToOne = ConvertToBoolean(oracleDataReader[42]),
                        IsSalesOrder = ConvertToBoolean(oracleDataReader[43]),
                        IsActivityEnabled = ConvertToBoolean(oracleDataReader[44]),
                        IsFastOrderEnabled = ConvertToBoolean(oracleDataReader[45]),
                        LoadingCardNo4 = ConvertToString(oracleDataReader[46]),
                        LoadingCardNo5 = ConvertToString(oracleDataReader[47]),
                        LoadingCardNo6 = ConvertToString(oracleDataReader[48]),
                        LoadingCardNo1Name = ConvertToString(oracleDataReader[49]),
                        LoadingCardNo2Name = ConvertToString(oracleDataReader[50]),
                        LoadingCardNo3Name = ConvertToString(oracleDataReader[51]),
                        LoadingCardNo4Name = ConvertToString(oracleDataReader[52]),
                        LoadingCardNo5Name = ConvertToString(oracleDataReader[53]),
                        LoadingCardNo6Name = ConvertToString(oracleDataReader[54]),
                        BranchCode = token.BranchCode,
                        RegionCode = "",
                        SalesOrderDocTraCode = "",
                        IsSaveRealPayment = ConvertToBoolean(oracleDataReader[55]),
                        IsSaveLocationForEntity = ConvertToBoolean(oracleDataReader[56]),
                        IsControlLocForInvoice = ConvertToBoolean(oracleDataReader[57]),
                        IsControlLocForOrder = ConvertToBoolean(oracleDataReader[58]),
                        IsControlLocForWaybill = ConvertToBoolean(oracleDataReader[59]),
                        IsControlLocForOneToOne = ConvertToBoolean(oracleDataReader[60]),
                        IsControlLocForPayment = ConvertToBoolean(oracleDataReader[61]),
                        IsControlLocForActivity = ConvertToBoolean(oracleDataReader[62]),
                        OrderInvoicePriceSource = ConvertToInt32(oracleDataReader[63]),
                        OrderWaybillPriceSource = ConvertToInt32(oracleDataReader[64]),
                        WaybillInvoicePriceSource = ConvertToInt32(oracleDataReader[65]),
                        Discount1Source = ConvertToInt32(oracleDataReader[66]),
                        Discount2Source = ConvertToInt32(oracleDataReader[67]),
                        CoCode = ConvertToString(oracleDataReader[68]),
                        OrderDeliveryDay = 0,
                        LoadingCardNo1Order = 1,
                        LoadingCardNo2Order = 2,
                        LoadingCardNo3Order = 3,
                        LoadingCardNo4Order = 4,
                        LoadingCardNo5Order = 5,
                        LoadingCardNo6Order = 6,
                        CatCode1 = ConvertToString(oracleDataReader[69]),
                        CatCode2 = ConvertToString(oracleDataReader[70]),
                        SalesPersonName = ConvertToString(oracleDataReader[71]),
                        SalesReturnInvoiceDocTraCode = ConvertToString(oracleDataReader[72]),
                        SalesOrderReturnDocTraCode = ConvertToString(oracleDataReader[73]),
                        IsSaveRealInvoice = ConvertToBoolean(oracleDataReader[74]),
                        IsSaveRealWaybill = ConvertToBoolean(oracleDataReader[75]),
                        CoCurCode = ConvertToString(oracleDataReader[76]),
                        ConsigneDocTraCode = ConvertToString(oracleDataReader[78]),
                        ConsigneReturnWhouseCode = ConvertToString(oracleDataReader[79]),
                        ConsigneReturnWhouseDesc = ConvertToString(oracleDataReader[80]),
                        CostCenterCode = ConvertToString(oracleDataReader[81]),
                        ConsigneReturnDocTraCode = ConvertToString(oracleDataReader[82]),
                        IsUseVatStatus = ConvertToBoolean(oracleDataReader[83]),
                        IsSaveRealOneToOne = ConvertToBoolean(oracleDataReader[84]),
                        IsGetQtyFromLoadingIns = ConvertToBoolean(oracleDataReader[85])
                    };

                    // kontrol edilecek olan.
                    if (!string.IsNullOrEmpty(oracleDataReader[77].ToString()))
                        pdaUserParams.InvoiceStartDate = ConvertToDateTime(oracleDataReader[77]);
                    if (pdaUserParams.InvoiceStartDate < DateTime.Now)
                    {
                        pdaUserParams.InvoiceStartDate = new DateTime(
                            DateTime.Now.Year,
                            DateTime.Now.Month,
                            DateTime.Now.Day, 0, 0, 0);
                    }

                    if (!string.IsNullOrEmpty(oracleDataReader[86].ToString()))
                        pdaUserParams.IsMobileDnoteGibNo = ConvertToBoolean(oracleDataReader[86]);
                    if (!string.IsNullOrEmpty(oracleDataReader[87].ToString()))
                        pdaUserParams.IsSalesOrderReturn = ConvertToBoolean(oracleDataReader[87]);
                    if (!string.IsNullOrEmpty(oracleDataReader[88].ToString()))
                        pdaUserParams.IsSalesReturnInvoice = ConvertToBoolean(oracleDataReader[88]);

                    if (!string.IsNullOrEmpty(oracleDataReader[89].ToString()))
                        pdaUserParams.IsPaymentDel = ConvertToBoolean(oracleDataReader[89]);
                    if (!string.IsNullOrEmpty(oracleDataReader[90].ToString()))
                        pdaUserParams.IsSalesInvoiceDel = ConvertToBoolean(oracleDataReader[90]);
                    if (!string.IsNullOrEmpty(oracleDataReader[91].ToString()))
                        pdaUserParams.IsSalesWatbillDel = ConvertToBoolean(oracleDataReader[91]);
                    if (!string.IsNullOrEmpty(oracleDataReader[92].ToString()))
                        pdaUserParams.IsSalesOrderDel = ConvertToBoolean(oracleDataReader[92]);

                    if (!string.IsNullOrEmpty(oracleDataReader[93].ToString()))
                        pdaUserParams.IsVehicleItem = ConvertToBoolean(oracleDataReader[93]);

                    // added 29.01.2023 U.US_NAME, U.US_SURNAME, U.NOTE2 
                    try
                    {
                        pdaUserParams.UserName = ConvertToString(oracleDataReader[94]);
                        pdaUserParams.UserSurname = ConvertToString(oracleDataReader[95]);
                        pdaUserParams.UserNote2 = ConvertToString(oracleDataReader[96]);
                    }
                    catch { }

                    try
                    {
                        if (!string.IsNullOrEmpty(oracleDataReader[97].ToString()))
                            pdaUserParams.IsPaymentUpd = ConvertToBoolean(oracleDataReader[97]);
                        if (!string.IsNullOrEmpty(oracleDataReader[98].ToString()))
                            pdaUserParams.IsSalesInvoiceUpd = ConvertToBoolean(oracleDataReader[98]);
                        if (!string.IsNullOrEmpty(oracleDataReader[99].ToString()))
                            pdaUserParams.IsSalesWaybillUpd = ConvertToBoolean(oracleDataReader[99]);
                        if (!string.IsNullOrEmpty(oracleDataReader[100].ToString()))
                            pdaUserParams.IsSalesOrderUpd = ConvertToBoolean(oracleDataReader[100]);

                        // 18.05.2023
                        if (!string.IsNullOrEmpty(oracleDataReader[101].ToString()))
                            pdaUserParams.IsFirstPriceList = ConvertToBoolean(oracleDataReader[101]);
                    }
                    catch { }
                }
                if (pdaUserParams != null)
                    Helper._Validate(pdaUserParams);
                if (pdaUserParams == null)
                    throw new Exception(string.Format("Kullanıcı parametreleri bulunamadı. Firma : {0} Kullanıcı Kodu : {1} Şifre : {2} ", (object)token.BranchCode, (object)token.Username, (object)token.Password));
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2334);
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return pdaUserParams;
        }

        internal static List<string> GetUserTokens(HotSaleServiceTables.MsgEx msg, IDbConnection connection)
        {
            var ret = new List<string>();
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                
                string str = string.Format(@"SELECT DISTINCT N.PUSH_CODE FROM HSMD_USERS_PARAMETER A INNER JOIN UYUMSOFT.USERS U ON U.US_ID = A.USR_ID
	                INNER JOIN UYUMSOFT.GNLD_BRANCH BR ON BR.BRANCH_ID = A.BRANCH_ID
	                INNER JOIN UYUMSOFT.FIND_SALES_PERSON S ON S.SALES_PERSON_ID = A.SALES_PERSON_ID 
	                INNER JOIN ZZ_USER_NOTIFY N ON N.BRANCH_CODE = BR.BRANCH_CODE AND N.USER_CODE = U.US_USERNAME AND N.PUSH_CODE IS NOT NULL AND N.PUSH_CODE != ''
	                WHERE S.SALES_PERSON_CODE = '{0}' AND BR.BRANCH_CODE = '{1}'", msg.Username, msg.BranchCode);
                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 23345);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(connection, str);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    ret.Add(ConvertToString(oracleDataReader[0]));
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 2334);
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return ret;
        }

        internal static BankAcc[] GetBankAccs(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<BankAcc> bankAccList = new List<BankAcc>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (string.IsNullOrEmpty(userParameters.BankAccCode1) && string.IsNullOrEmpty(userParameters.BankAccCode2) && string.IsNullOrEmpty(userParameters.BankAccCode3))
                    return new BankAcc[0];

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT BA.BANK_ACC_NO AS BANKA_HESA_NO,\r\n                                                        BA.DESCRIPTION AS BANKA_HESA_AD,\r\n                                                        BR.BRANCH_CODE  AS BRANCH_CODE\r\n                                                FROM {0} CBA\r\n                                                  INNER JOIN FIND_BANK_ACC BA ON BA.BANK_ACC_ID = CBA.BANK_ACC_ID\r\n                                                  INNER JOIN GNLD_BRANCH BR ON BR.BRANCH_ID = CBA.BRANCH_ID\r\n                                                WHERE  BR.BRANCH_CODE ='{1}' \r\n                                                      AND BA.BANK_ACC_NO IN ({2})", (object)"UYUMSOFT.FIND_CO_BANK_ACC", (object)token.BranchCode, (object)Helper._GenerateBankCodeFilter(userParameters));

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    BankAcc bankAcc = new BankAcc()
                    {
                        BankAccCode = ConvertToString(oracleDataReader[0]),
                        BankAccDesc = ConvertToString(oracleDataReader[1]),
                        BranchCode = token.BranchCode
                    };
                    bankAccList.Add(bankAcc);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return bankAccList.ToArray();
        }

        internal static bool _HasDepot(string depoKod, IEnumerable<Whouse> depos)
        {
            depoKod = depoKod.ToLowerInvariant();
            return depos.Any<Whouse>((Func<Whouse, bool>)(t => t.WhouseCode.ToLowerInvariant().Equals(depoKod)));
        }

        internal static List<Whouse> GetOtherDepos(HotSaleServiceTables.Token token, IDbConnection connection)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<Whouse> whouseList1 = new List<Whouse>();
            try
            {
                EventLog.WriteEntry("Application", "Giriş 1", EventLogEntryType.Information, 1);
                PdaUserParams userParameters = Helper.GetUserParameters(token, connection);
                if (userParameters == null)
                    return (List<Whouse>)null;
                EventLog.WriteEntry("Application", "param geldi", EventLogEntryType.Information, 2);
                whouseList1.Add(new Whouse()
                {
                    WhouseCode = userParameters.VehicleWhouseCode,
                    WhouseDesc = userParameters.VehicleWhouseDesc,
                    WhouseType = HotSaleServiceTables.WhouseType.Arac,
                    BranchCode = token.BranchCode
                });
                whouseList1.Add(new Whouse()
                {
                    WhouseCode = userParameters.OneToOneWhouseCode,
                    WhouseDesc = userParameters.OneToOneWhouseDesc,
                    WhouseType = HotSaleServiceTables.WhouseType.BireBir,
                    BranchCode = token.BranchCode
                });
                whouseList1.Add(new Whouse()
                {
                    WhouseCode = userParameters.VehicleReturnWhouseCode,
                    WhouseDesc = userParameters.VehicleReturnWhouseCode,
                    WhouseType = HotSaleServiceTables.WhouseType.Iade,
                    BranchCode = token.BranchCode
                });
                whouseList1.Add(new Whouse()
                {
                    WhouseCode = userParameters.ProductWhouseCode,
                    WhouseDesc = userParameters.ProductWhouseDesc,
                    WhouseType = HotSaleServiceTables.WhouseType.Urun,
                    BranchCode = token.BranchCode
                });
                EventLog.WriteEntry("Application", "depoler eklendi", EventLogEntryType.Information, 3);
                List<Whouse> whouseList2 = new List<Whouse>();
                string cmdText = string.Format("SELECT\r\n                                                        W1.WHOUSE_CODE                    AS ARAC_DEPO_KOD,\r\n                                                        W1.DESCRIPTION                    AS ARAC_DEPO_AD\r\n\r\n                                                       FROM {0} A\r\n                                                           INNER JOIN UYUMSOFT.GNLD_BRANCH            BR ON BR.BRANCH_ID      = A.BRANCH_ID\r\n                                                           INNER JOIN UYUMSOFT.INVD_WHOUSE            W1 ON W1.WHOUSE_ID      = A.VEHICLE_WHOUSE_ID\r\n     \r\n                                                           WHERE  BR.BRANCH_CODE   = '{1}'\r\n                                                        ", (object)"UYUMSOFT.HSMD_USERS_PARAMETER", (object)token.BranchCode);

                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = cmdText;

                oracleDataReader = oracleCommand.ExecuteReader();
                EventLog.WriteEntry("Application", "diğer depolar çekildi  - " + cmdText, EventLogEntryType.Information, 4);
                while (oracleDataReader.Read())
                {
                    string depoKod = ConvertToString(oracleDataReader[0]);
                    if (!string.IsNullOrEmpty(depoKod) && !Helper._HasDepot(depoKod, (IEnumerable<Whouse>)whouseList2))
                    {
                        Whouse whouse = new Whouse()
                        {
                            WhouseCode = depoKod,
                            WhouseDesc = ConvertToString(oracleDataReader[1]),
                            WhouseType = HotSaleServiceTables.WhouseType.DigerAraclar,
                            BranchCode = token.BranchCode
                        };
                        whouseList2.Add(whouse);
                    }
                }
                EventLog.WriteEntry("Application", "diğer depolar okundu", EventLogEntryType.Information, 5);
                List<Whouse> whouseList3 = new List<Whouse>()
        {
          whouseList1[0],
          whouseList1[1],
          whouseList1[2],
          whouseList1[3]
        };
                string source1 = "Application";
                string str1 = "currents count - ";
                int num1 = whouseList3.Count;
                string str2 = num1.ToString();
                string message1 = str1 + str2;
                int num2 = 4;
                int eventID1 = 6;
                EventLog.WriteEntry(source1, message1, (EventLogEntryType)num2, eventID1);
                if (!string.IsNullOrEmpty(userParameters.VehicleUnloadingWhouseCode))
                {
                    Whouse whouse = new Whouse()
                    {
                        WhouseCode = userParameters.VehicleUnloadingWhouseCode,
                        WhouseDesc = userParameters.VehicleUnloadingWhouseCode,
                        WhouseType = HotSaleServiceTables.WhouseType.Bosaltma,
                        BranchCode = token.BranchCode
                    };
                    whouseList3.Add(whouse);
                }
                string source2 = "Application";
                string str3 = "bosaltma eklendi - ";
                num1 = whouseList3.Count;
                string str4 = num1.ToString();
                string message2 = str3 + str4;
                int num3 = 4;
                int eventID2 = 7;
                EventLog.WriteEntry(source2, message2, (EventLogEntryType)num3, eventID2);
                for (int index = 0; index < whouseList2.Count; num1 = index++)
                {
                    if (!string.IsNullOrEmpty(whouseList2[index].WhouseCode) && !Helper._HasDepot(whouseList2[index].WhouseCode, (IEnumerable<Whouse>)whouseList3))
                        whouseList3.Add(whouseList2[index]);
                }
                if (!string.IsNullOrEmpty(userParameters.ConsigneReturnWhouseCode))
                {
                    Whouse whouse = new Whouse()
                    {
                        WhouseCode = userParameters.ConsigneReturnWhouseCode,
                        WhouseDesc = userParameters.ConsigneReturnWhouseDesc,
                        WhouseType = HotSaleServiceTables.WhouseType.KonsinyeIade,
                        BranchCode = token.BranchCode
                    };
                    whouseList3.Add(whouse);
                }
                string source3 = "Application";
                string str5 = "sonuncular eklendi - ";
                num1 = whouseList3.Count;
                string str6 = num1.ToString();
                string message3 = str5 + str6;
                int num4 = 4;
                int eventID3 = 8;
                EventLog.WriteEntry(source3, message3, (EventLogEntryType)num4, eventID3);
                return whouseList3;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
        }

        internal static Route[] GetRoutes(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<Route> routeList = new List<Route>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT\r\n                                                    R.ROUTE_CODE    AS ROTA_KOD,\r\n                                                    R.DESCRIPTION   AS ROTA_AD,\r\n                                                    R.DESCRIPTION   AS ROTA_ACIKLAMA,\r\n                                                    BR.BRANCH_CODE  AS BRANCH_CODE\r\n   \r\n                                                 FROM       {0} RLM \r\n                                                   INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_D RLD ON RLD.SAL_PER_ROUTE_REL_M_ID  = RLM.SAL_PER_ROUTE_REL_M_ID\r\n                                                   INNER JOIN UYUMSOFT.GNLD_BRANCH              BR  ON BR.BRANCH_ID                = RLM.BRANCH_ID\r\n                                                   INNER JOIN UYUMSOFT.HSMD_ROUTE               R   ON R.ROUTE_ID                  = RLD.ROUTE_ID\r\n                                                   INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = RLM.SALES_PERSON_ID   AND RLM.CO_ID      = S.CO_ID   \r\n                                                 WHERE      BR.BRANCH_CODE      = '{1}'\r\n                                                          AND S.SALES_PERSON_CODE = '{2}'\r\n                                                          AND RLD.ISPASSIVE       = 0 \r\n                                                          AND R.ISPASSIVE         = 0 \r\n                                                 ", (object)"UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_M", (object)token.BranchCode, (object)userParameters.SalesPersonCode);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    Route route = new Route()
                    {
                        RouteCode = ConvertToString(oracleDataReader[0]),
                        RouteName = ConvertToString(oracleDataReader[1]),
                        RouteDescription = ConvertToString(oracleDataReader[2]),
                        BranchCode = ConvertToString(oracleDataReader[3])
                    };
                    routeList.Add(route);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return routeList.ToArray();
        }

        internal static EntityRoute[] GetEntityRoutes(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<EntityRoute> entityRouteList = new List<EntityRoute>();
            try
            {
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");

                oracleCommand = conn.CreateCommand();
                var tmp = string.Format("SELECT\r\n                                                            R.ROUTE_CODE    AS ROTA_KOD,\r\n                                                            FE.ENTITY_CODE  AS CARI_KOD,\r\n                                                            BR.BRANCH_CODE  AS BRANCH_CODE, \r\n  NVL(RED.LINE_NO, 0) AS LINE_NO\r\n, R.BACK_COLOR\r\n, R.FORE_COLOR\r\n         FROM {0}  RED\r\n                                                               INNER JOIN UYUMSOFT.HSMD_ROUTE_ENTITY_REL_M  RELM ON RELM.ROUTE_ENTITY_REL_M_ID   = RED.ROUTE_ENTITY_REL_M_ID\r\n                                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = RED.ENTITY_ID\r\n                                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR  ON BR.BRANCH_ID                = RELM.BRANCH_ID\r\n                                                               INNER JOIN UYUMSOFT.HSMD_ROUTE               R   ON R.ROUTE_ID                  = RELM.ROUTE_ID\r\n                                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_D RLD ON RLD.ROUTE_ID                = RELM.ROUTE_ID                       AND RLD.BRANCH_ID = RELM.BRANCH_ID\r\n                                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_M RLM ON RLM.SAL_PER_ROUTE_REL_M_ID  = RLD.SAL_PER_ROUTE_REL_M_ID  \r\n                                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = RLM.SALES_PERSON_ID                 AND RLM.CO_ID      = S.CO_ID \r\n                                                               INNER JOIN UYUMSOFT.HSMT_ROUTE_PLANING       RP  ON RP.ROUTE_ID                 = RELM.ROUTE_ID                        AND RP.BRANCH_ID   = RELM.BRANCH_ID   AND RP.ENTITY_ID = RED.ENTITY_ID \r\n \r\n                                                               WHERE      BR.BRANCH_CODE      = '{1}'\r\n                                                                      AND S.SALES_PERSON_CODE = '{2}'\r\n                                                                      AND RLD.ISPASSIVE       = 0 \r\n                                                                      AND R.ISPASSIVE         = 0 \r\n                                                                      AND RP.STATUS           = 1 \r\n                                                                      AND TO_DATE(TO_CHAR(RP.DOC_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{3}','dd/MM/yyyy')\r\n                                                            ", (object)"UYUMSOFT.HSMD_ROUTE_ENTITY_REL_D", (object)token.BranchCode, (object)userParameters.SalesPersonCode, (object)DateTime.Today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo));
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, tmp);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    entityRouteList.Add(new EntityRoute()
                    {
                        RouteCode = ConvertToString(oracleDataReader[0]),
                        EntityCode = ConvertToString(oracleDataReader[1]),
                        BranchCode = ConvertToString(oracleDataReader[2]),
                        LineNo = ConvertToDecimal(oracleDataReader[3]),
                        BackColor = ConvertToString(oracleDataReader[4]),
                        ForeColor = ConvertToString(oracleDataReader[5]),
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return entityRouteList.ToArray();
        }

        internal static List<Entity> GetEntitiesV2(String BranchCode, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            Dictionary<string, Entity> dictionary = new Dictionary<string, Entity>();
            try
            {
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                string format1 = "\r\n                                                    SELECT DISTINCT * FROM (\r\n                                                    SELECT\r\n                                                    S.SALES_PERSON_CODE, FE.ENTITY_CODE          AS CARI_KOD,\r\n                                                    FE.ENTITY_NAME          AS CARI_AD,\r\n                                                    FE.ENTITY_NAME_SHORT    AS CARI_KISA_AD,\r\n                                                    FE.ADDRESS1             AS FAT_ADRES1,\r\n                                                    FE.ADDRESS2             AS FAT_ADRES2,\r\n                                                    FE.ADDRESS3             AS FAT_ADRES3,\r\n                                                    C.CITY_NAME             AS SEHIR_AD,\r\n                                                    T.TOWN_NAME             AS ILCE_AD,\r\n                                                    U.COUNTRY_NAME          AS ULKE_AD,          \r\n                                                    V.DESCRIPTION           AS VERGI_DAIRE,\r\n                                                    FE.TAX_NO               AS VERGI_NO,\r\n                                                    FCE.DUE_DAY             AS VADE_GUN,\r\n                                                    NVL(P.ENTITY_PRICE_GRP_CODE, ENTITY_CODE) AS FIYAT_GRUP_KOD,\r\n                                                    FE.ADDRESS1             AS ADRES1,\r\n                                                    FE.ADDRESS2             AS ADRES2,\r\n                                                    FE.ADDRESS3             AS ADRES3,\r\n                                                    FCE.NOTE1               AS ACIKLAMA,\r\n                                                    NVL(D.DISC_RATE,0)      AS ISK_1,\r\n                                                    NVL(D2.DISC_RATE,0)      AS ISK_2,\r\n                                                    FE.TEL1                 AS TEL1,\r\n                                                    FE.TEL2                 AS TEL2,\r\n                                                    FE.FAX                  AS FAX,\r\n                                                    NVL(FE.LATITUDE,'0')            AS LATITUDE,\r\n                                                    NVL(FE.LONGITUDE,'0')            AS LOGITUDE,\r\n                                                    CASE \r\n                                                      WHEN UP.ENTITY_ID IS NULL THEN 0\r\n                                                      WHEN FE.ENTITY_ID = UP.ENTITY_ID THEN 1 ELSE 0 \r\n                                                        END AS ENTITY_TYPE,\r\n                                                    NVL(RED.IS_SALES_INVOICE,1) AS IS_SALES_INVOICE,\r\n                                                    NVL(RED.IS_ONE_TO_ONE,1) AS IS_ONE_TO_ONE,\r\n                                                    NVL(RED.IS_PAYMENT1,1) AS IS_PAYMENT1,\r\n                                                    LC1.LOADING_CARD_CODE AS LOADING_CARD_CODE1,\r\n                                                    NVL(RED.IS_PAYMENT2,1) AS IS_PAYMENT2,\r\n                                                    LC2.LOADING_CARD_CODE AS LOADING_CARD_CODE2,\r\n                                                    NVL(RED.IS_PAYMENT3,1) AS IS_PAYMENT3,\r\n                                                    LC3.LOADING_CARD_CODE AS LOADING_CARD_CODE3,\r\n                                                    NVL(RED.IS_PAYMENT4,1) AS IS_PAYMENT4,\r\n                                                    LC4.LOADING_CARD_CODE AS LOADING_CARD_CODE4,\r\n                                                    NVL(RED.IS_PAYMENT5,1) AS IS_PAYMENT5,\r\n                                                    LC5.LOADING_CARD_CODE AS LOADING_CARD_CODE5,\r\n                                                    NVL(RED.IS_PAYMENT6,1) AS IS_PAYMENT6,\r\n                                                    LC6.LOADING_CARD_CODE AS LOADING_CARD_CODE6,\r\n                                                    NVL(RED.IS_SALES_RETURN_INVOICE,1) AS IS_SALES_RETURN_INVOICE,\r\n                                                    NVL(RED.IS_SALES_WAYBILL,1) AS IS_SALES_WAYBILL,\r\n                                                    NVL(FE.RADIUS,0) AS RADIUS,\r\n                                                    GRP1.ENTITY_GRP_CODE AS ENTITY_GRP_CODE1,\r\n                                                    GRP1.ENTITY_GRP_NAME AS ENTITY_GRP_NAME1,\r\n                                                    GRP2.ENTITY_GRP_CODE AS ENTITY_GRP_CODE2,\r\n                                                    GRP2.ENTITY_GRP_NAME AS ENTITY_GRP_NAME2,\r\n                                                    GRP3.ENTITY_GRP_CODE AS ENTITY_GRP_CODE3,\r\n                                                    GRP3.ENTITY_GRP_NAME AS ENTITY_GRP_NAME3,\r\n                                                    DD.DUE_DISC_CODE,\r\n                                                    NVL(RED.IS_SALES_ORDER,1) AS IS_SALES_ORDER,\r\n                                                    W.WHOUSE_CODE,\r\n                                                    NVL(RED.IS_CONSIGNE,0) AS IS_CONSIGNE\r\n                                                    \r\n  \r\n                                               FROM       {0}  RED\r\n                                               INNER JOIN UYUMSOFT.HSMD_ROUTE_ENTITY_REL_M  REM ON REM.ROUTE_ENTITY_REL_M_ID   = RED.ROUTE_ENTITY_REL_M_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = RED.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR  ON BR.BRANCH_ID                = REM.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_ROUTE               R   ON R.ROUTE_ID                  = REM.ROUTE_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_D RLD ON RLD.ROUTE_ID                = REM.ROUTE_ID AND RLD.BRANCH_ID = REM.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_M RLM ON RLM.SAL_PER_ROUTE_REL_M_ID  = RLD.SAL_PER_ROUTE_REL_M_ID  \r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = RLM.SALES_PERSON_ID AND RLM.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.HSMT_ROUTE_PLANING       RP  ON RP.ROUTE_ID                 = REM.ROUTE_ID AND RP.BRANCH_ID   = REM.BRANCH_ID   AND RP.ENTITY_ID = RED.ENTITY_ID     \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = BR.CO_ID\r\n                                               LEFT JOIN FIND_DUE_DISC DD ON DD.DUE_DISC_ID = FCE.DUE_DISC_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP1 ON GRP1.ENTITY_GRP_ID      = FCE.ENTITY_GRP_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP2 ON GRP2.ENTITY_GRP_ID      = FCE.ENTITY_GRP2_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP3 ON GRP3.ENTITY_GRP_ID      = FCE.ENTITY_GRP3_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC1 ON RED.LOADING_CARD_ID1 = LC1.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC2 ON RED.LOADING_CARD_ID2 = LC2.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC3 ON RED.LOADING_CARD_ID3 = LC3.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC4 ON RED.LOADING_CARD_ID4 = LC4.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC5 ON RED.LOADING_CARD_ID5 = LC5.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC6 ON RED.LOADING_CARD_ID6 = LC6.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.INVD_WHOUSE W ON W.WHOUSE_ID = FCE.WHOUSE_ID\r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID   \r\n                                            WHERE      BR.BRANCH_CODE      = '{2}'\r\n AND RLD.ISPASSIVE       = 0 \r\n                                                      AND R.ISPASSIVE         = 0 \r\n                                                      AND RP.STATUS           = 1 \r\n                                                      AND TO_DATE(TO_CHAR(RP.DOC_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{4}','dd/MM/yyyy') \r\n                                               UNION ALL\r\n                                            SELECT\r\n                                                    S.SALES_PERSON_CODE, FE.ENTITY_CODE          AS CARI_KOD,\r\n                                                    FE.ENTITY_NAME          AS CARI_AD,\r\n                                                    FE.ENTITY_NAME_SHORT    AS CARI_KISA_AD,\r\n                                                    FE.ADDRESS1             AS FAT_ADRES1,\r\n                                                    FE.ADDRESS2             AS FAT_ADRES2,\r\n                                                    FE.ADDRESS3             AS FAT_ADRES3,\r\n                                                    C.CITY_NAME             AS SEHIR_AD,\r\n                                                    T.TOWN_NAME             AS ILCE_AD,\r\n                                                    U.COUNTRY_NAME          AS ULKE_AD,          \r\n                                                    V.DESCRIPTION           AS VERGI_DAIRE,\r\n                                                    FE.TAX_NO               AS VERGI_NO,\r\n                                                    FCE.DUE_DAY             AS VADE_GUN,\r\n                                                    NVL(P.ENTITY_PRICE_GRP_CODE, ENTITY_CODE) AS FIYAT_GRUP_KOD,\r\n                                                    FE.ADDRESS1             AS ADRES1,\r\n                                                    FE.ADDRESS2             AS ADRES2,\r\n                                                    FE.ADDRESS3             AS ADRES3,\r\n                                                    FCE.NOTE1               AS ACIKLAMA,\r\n                                                    NVL(D.DISC_RATE,0)      AS ISK_1,\r\n                                                    NVL(D2.DISC_RATE,0)      AS ISK_2,\r\n                                                    FE.TEL1                 AS TEL1,\r\n                                                    FE.TEL2                 AS TEL2,\r\n                                                    FE.FAX                  AS FAX,\r\n                                                    NVL(FE.LATITUDE,'0')            AS LATITUDE,\r\n                                                    NVL(FE.LONGITUDE,'0')            AS LOGITUDE,\r\n                                                    CASE \r\n                                                      WHEN UP.ENTITY_ID IS NULL THEN 0\r\n                                                      WHEN FE.ENTITY_ID = UP.ENTITY_ID THEN 1 ELSE 0 \r\n                                                        END AS ENTITY_TYPE,\r\n                                                    1 AS IS_SALES_INVOICE,\r\n                                                    1 AS IS_ONE_TO_ONE,\r\n                                                    1 AS IS_PAYMENT1,\r\n                                                    LC1.LOADING_CARD_CODE AS LOADING_CARD_CODE1,\r\n                                                    1 AS IS_PAYMENT2,\r\n                                                    LC2.LOADING_CARD_CODE AS LOADING_CARD_CODE2,\r\n                                                    1 AS IS_PAYMENT3,\r\n                                                    LC3.LOADING_CARD_CODE AS LOADING_CARD_CODE3,\r\n                                                    1 AS IS_PAYMENT4,\r\n                                                    LC4.LOADING_CARD_CODE AS LOADING_CARD_CODE4, \r\n                                                    1 AS IS_PAYMENT5,\r\n                                                    LC5.LOADING_CARD_CODE AS LOADING_CARD_CODE5,\r\n                                                    1 AS IS_PAYMENT6,\r\n                                                    LC6.LOADING_CARD_CODE AS LOADING_CARD_CODE6,\r\n                                                    1 AS IS_SALES_RETURN_INVOICE,\r\n                                                    1 AS IS_SALES_WAYBILL,\r\n                                                    NVL(FE.RADIUS,0) AS RADIUS,\r\n                                                    GRP1.ENTITY_GRP_CODE AS ENTITY_GRP_CODE1,\r\n                                                    GRP1.ENTITY_GRP_NAME AS ENTITY_GRP_NAME1,\r\n                                                    GRP2.ENTITY_GRP_CODE AS ENTITY_GRP_CODE2,\r\n                                                    GRP2.ENTITY_GRP_NAME AS ENTITY_GRP_NAME2,\r\n                                                    GRP3.ENTITY_GRP_CODE AS ENTITY_GRP_CODE3,\r\n                                                    GRP3.ENTITY_GRP_NAME AS ENTITY_GRP_NAME3,\r\n                                                    DD.DUE_DISC_CODE,\r\n                                                    1 AS IS_SALES_ORDER,\r\n                                                    W.WHOUSE_CODE,\r\n                                                    1 AS IS_CONSIGNE\r\n  \r\n                                               FROM       HSMT_LOADING_INSTRUCTION  LI\r\n                                               INNER JOIN HSMT_LOADING_INSTRUCTION_D LID ON LI.LOADING_INSTRUCTION_ID = LID.LOADING_INSTRUCTION_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = LID.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR ON BR.BRANCH_ID = LI.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = LI.SALES_PERSON_ID AND LI.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = LI.CO_ID\r\n                                               LEFT JOIN FIND_DUE_DISC DD ON DD.DUE_DISC_ID = FCE.DUE_DISC_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP1 ON GRP1.ENTITY_GRP_ID      = FCE.ENTITY_GRP_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP2 ON GRP2.ENTITY_GRP_ID      = FCE.ENTITY_GRP2_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP3 ON GRP3.ENTITY_GRP_ID      = FCE.ENTITY_GRP3_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID \r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC1 ON UP.LOADING_CARD_ID1 = LC1.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC2 ON UP.LOADING_CARD_ID2 = LC2.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC3 ON UP.LOADING_CARD_ID3 = LC3.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC4 ON UP.LOADING_CARD_ID4 = LC4.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC5 ON UP.LOADING_CARD_ID5 = LC5.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC6 ON UP.LOADING_CARD_ID6 = LC6.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.INVD_WHOUSE W ON W.WHOUSE_ID = FCE.WHOUSE_ID\r\n                                            WHERE      BR.BRANCH_CODE      = '{2}'\r\n AND TO_DATE(TO_CHAR(LI.LOADING_INSTRUCTION_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{4}','dd/MM/yyyy') \r\n                                                      AND FE.ENTITY_CODE NOT IN (SELECT\r\n                                                    FE.ENTITY_CODE          AS CARI_KOD\r\n  \r\n                                               FROM       {0}  RED\r\n                                               INNER JOIN UYUMSOFT.HSMD_ROUTE_ENTITY_REL_M  REM ON REM.ROUTE_ENTITY_REL_M_ID   = RED.ROUTE_ENTITY_REL_M_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = RED.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR  ON BR.BRANCH_ID                = REM.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_ROUTE               R   ON R.ROUTE_ID                  = REM.ROUTE_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_D RLD ON RLD.ROUTE_ID                = REM.ROUTE_ID AND RLD.BRANCH_ID = REM.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_M RLM ON RLM.SAL_PER_ROUTE_REL_M_ID  = RLD.SAL_PER_ROUTE_REL_M_ID  \r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = RLM.SALES_PERSON_ID AND RLM.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.HSMT_ROUTE_PLANING       RP  ON RP.ROUTE_ID                 = REM.ROUTE_ID AND RP.BRANCH_ID   = REM.BRANCH_ID   AND RP.ENTITY_ID = RED.ENTITY_ID     \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = BR.CO_ID\r\n                                               LEFT JOIN FIND_DUE_DISC DD ON DD.DUE_DISC_ID = FCE.DUE_DISC_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP1 ON GRP1.ENTITY_GRP_ID      = FCE.ENTITY_GRP_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP2 ON GRP2.ENTITY_GRP_ID      = FCE.ENTITY_GRP2_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP3 ON GRP3.ENTITY_GRP_ID      = FCE.ENTITY_GRP3_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC1 ON RED.LOADING_CARD_ID1 = LC1.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC2 ON RED.LOADING_CARD_ID2 = LC2.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC3 ON RED.LOADING_CARD_ID3 = LC3.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC4 ON RED.LOADING_CARD_ID4 = LC4.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC5 ON RED.LOADING_CARD_ID5 = LC5.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC6 ON RED.LOADING_CARD_ID6 = LC6.LOADING_CARD_ID\r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID WHERE      BR.BRANCH_CODE      = '{2}'\r\n AND RLD.ISPASSIVE       = 0 \r\n                                                      AND R.ISPASSIVE         = 0 \r\n                                                      AND RP.STATUS           = 1 \r\n                                                      AND TO_DATE(TO_CHAR(RP.DOC_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{4}','dd/MM/yyyy') )\r\n                                                ) t\r\n                                               ";
                object[] objArray1 = new object[5]
                {
          (object) "UYUMSOFT.HSMD_ROUTE_ENTITY_REL_D",
          (object) 0,//coBranchId,
          (object) BranchCode,
          (object) "", //userParameters.SalesPersonCode,
          null
                };
                int index1 = 4;
                DateTime today = DateTime.Today;
                string str1 = today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo);
                objArray1[index1] = (object)str1;
                string str2 = string.Format(format1, objArray1);
                EventLog.WriteEntry("Application", str2, EventLogEntryType.Information, 1283);

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, str2);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    Entity entity = new Entity();
                    entity.SaticiKod = ConvertToString(oracleDataReader[0]);
                    entity.EntityCode = ConvertToString(oracleDataReader[1]);
                    entity.EntityName = ConvertToString(oracleDataReader[2]);
                    entity.ShortEntityName = ConvertToString(oracleDataReader[3]);
                    entity.ShippingAddress1 = ConvertToString(oracleDataReader[4]);
                    entity.ShippingAddress2 = ConvertToString(oracleDataReader[5]);
                    entity.ShippingAddress3 = ConvertToString(oracleDataReader[6]);
                    entity.CityName = ConvertToString(oracleDataReader[7]);
                    entity.TownName = ConvertToString(oracleDataReader[8]);
                    entity.CountryName = ConvertToString(oracleDataReader[9]);
                    entity.TaxOfficeCode = ConvertToString(oracleDataReader[10]);
                    entity.TaxNo = ConvertToString(oracleDataReader[11]);
                    entity.DueDay = ConvertToInt32(oracleDataReader[12]);
                    entity.PriceListGroupCode = ConvertToString(oracleDataReader[13]);
                    entity.Address1 = ConvertToString(oracleDataReader[14]);
                    entity.Address2 = ConvertToString(oracleDataReader[15]);
                    entity.Address3 = ConvertToString(oracleDataReader[16]);
                    entity.Note = ConvertToString(oracleDataReader[17]);
                    entity.DiscRate1 = ConvertToDecimal(oracleDataReader[18]);
                    entity.DiscRate2 = ConvertToDecimal(oracleDataReader[19]);
                    entity.Tel1 = ConvertToString(oracleDataReader[20]);
                    entity.Tel2 = ConvertToString(oracleDataReader[21]);
                    entity.Fax = ConvertToString(oracleDataReader[22]);
                    string str3 = ConvertToString(oracleDataReader[23]);
                    string str4 = ConvertToString(oracleDataReader[24]);
                    string decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                    if (decimalSeparator == ".")
                    {
                        str3 = str3.Replace(',', '.');
                        str4 = str4.Replace(',', '.');
                    }
                    else if (decimalSeparator == ",")
                    {
                        str3 = str3.Replace('.', ',');
                        str4 = str4.Replace('.', ',');
                    }
                    entity.Latitude = ConvertToDecimal(str3);
                    entity.Longitude = ConvertToDecimal(str4);
                    entity.EntityType = ConvertToInt32(oracleDataReader[24]);
                    entity.IsInvoice = ConvertToBoolean(ConvertToInt32(oracleDataReader[26]));
                    entity.IsOneToOne = ConvertToBoolean(ConvertToInt32(oracleDataReader[27]));
                    entity.IsPayment1 = ConvertToBoolean(ConvertToInt32(oracleDataReader[28]));
                    entity.LoadingCard1 = ConvertToString(oracleDataReader[29]);
                    entity.IsPayment2 = ConvertToBoolean(ConvertToInt32(oracleDataReader[30]));
                    entity.LoadingCard2 = ConvertToString(oracleDataReader[31]);
                    entity.IsPayment3 = ConvertToBoolean(ConvertToInt32(oracleDataReader[32]));
                    entity.LoadingCard3 = ConvertToString(oracleDataReader[33]);
                    entity.IsPayment4 = ConvertToBoolean(ConvertToInt32(oracleDataReader[34]));
                    entity.LoadingCard4 = ConvertToString(oracleDataReader[35]);
                    entity.IsPayment5 = ConvertToBoolean(ConvertToInt32(oracleDataReader[36]));
                    entity.LoadingCard5 = ConvertToString(oracleDataReader[37]);
                    entity.IsPayment6 = ConvertToBoolean(ConvertToInt32(oracleDataReader[38]));
                    entity.LoadingCard6 = ConvertToString(oracleDataReader[39]);
                    entity.IsReturnWaybill = ConvertToBoolean(ConvertToInt32(oracleDataReader[40]));
                    entity.IsWaybill = ConvertToBoolean(ConvertToInt32(oracleDataReader[41]));
                    entity.Radius = (Decimal)ConvertToInt32(oracleDataReader[42]);
                    entity.BranchCode = BranchCode;
                    entity.EntityGrpCode1 = ConvertToString(oracleDataReader[43]);
                    entity.EntityGrpName1 = ConvertToString(oracleDataReader[44]);
                    entity.EntityGrpCode2 = ConvertToString(oracleDataReader[45]);
                    entity.EntityGrpName2 = ConvertToString(oracleDataReader[46]);
                    entity.EntityGrpCode3 = ConvertToString(oracleDataReader[47]);
                    entity.EntityGrpName3 = ConvertToString(oracleDataReader[48]);
                    entity.DueDiscCode = ConvertToString(oracleDataReader[49]);
                    entity.IsOrder = ConvertToBoolean(ConvertToInt32(oracleDataReader[50]));
                    entity.WhouseCode = ConvertToString(oracleDataReader[51]);
                    entity.IsConsigne = ConvertToBoolean(oracleDataReader[52]);
                    dictionary.Add(entity.EntityCode, entity);
                }
                if (dictionary.Count == 0)
                    throw new Exception("Hiç cari bulunamamıştır. Rota planlamayı kontrol ediniz!!!");
                string[] strArray = new string[dictionary.Count];
                string inFilter = Helper._GenerateInFilter(dictionary.Keys.ToArray<string>(), "FE.ENTITY_CODE");
                string format2 = "SELECT\r\n                                                            FE.ENTITY_CODE             AS CARI_KOD,\r\n                                                            NVL(SUM(FD.PLUS_MINUS*FD.AMT),0)  AS BAKIYE,\r\n                                                            NVL(SUM(CASE WHEN TO_DATE(TO_CHAR(FD.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') = TO_DATE('{3}','dd/MM/yyyy') THEN FD.PLUS_MINUS*FD.AMT ELSE 0 END),0)  AS BAKIYE_GUN\r\n   \r\n                                                            FROM       {0}      FE\r\n                                                               INNER JOIN UYUMSOFT.FINT_FIN_D       FD  ON FD.ENTITY_ID  = FE.ENTITY_ID\r\n                                                               INNER JOIN UYUMSOFT.GNLD_BRANCH      BR  ON BR.BRANCH_ID  = FD.BRANCH_ID\r\n         \r\n                    INNER JOIN FINP_FIN_PARAMETER FP ON FP.CO_ID = FD.CO_ID \r\n         \r\n                                        WHERE     BR.BRANCH_CODE      = '{1}'\r\n                                                                      AND {2}\r\n                                                                      AND TO_DATE(TO_CHAR(FD.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') <= TO_DATE('{3}','dd/MM/yyyy')\r\n          \r\n                                           AND TO_DATE(TO_CHAR(FP.ENTITY_PERIOD_START_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy') <= TO_DATE(TO_CHAR(FD.DOC_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy')  \r\n          \r\n               GROUP BY FE.ENTITY_CODE\r\n                                                              ";
                object[] objArray2 = new object[4]
                {
          (object) "UYUMSOFT.FIND_ENTITY",
          (object) BranchCode,
          (object) inFilter,
          null
                };
                int index2 = 3;
                today = DateTime.Today;
                string str5 = today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo);
                objArray2[index2] = (object)str5;

                // kapat ve temizle
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, string.Format(format2, objArray2));

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    string index3 = ConvertToString(oracleDataReader[0]);
                    dictionary[index3].Balance = ConvertToDecimal(oracleDataReader[1]);
                    dictionary[index3].DailyBalance = ConvertToDecimal(oracleDataReader[2]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            List<Entity> entityList = new List<Entity>();
            entityList.AddRange((IEnumerable<Entity>)dictionary.Values);
            return entityList;
        }

        internal static Entity[] GetEntities(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            Dictionary<string, Entity> dictionary = new Dictionary<string, Entity>();
            try
            {
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId = Helper.GetCoBranchId(token, conn, userParameters, "U");
                string format1 = "\r\n                                                    SELECT DISTINCT * FROM (\r\n                                                    SELECT\r\n                                                    FE.ENTITY_CODE          AS CARI_KOD,\r\n                                                    FE.ENTITY_NAME          AS CARI_AD,\r\n                                                    FE.ENTITY_NAME_SHORT    AS CARI_KISA_AD,\r\n                                                    FE.ADDRESS1             AS FAT_ADRES1,\r\n                                                    FE.ADDRESS2             AS FAT_ADRES2,\r\n                                                    FE.ADDRESS3             AS FAT_ADRES3,\r\n                                                    C.CITY_NAME             AS SEHIR_AD,\r\n                                                    T.TOWN_NAME             AS ILCE_AD,\r\n                                                    U.COUNTRY_NAME          AS ULKE_AD,          \r\n                                                    V.DESCRIPTION           AS VERGI_DAIRE,\r\n                                                    FE.TAX_NO               AS VERGI_NO,\r\n                                                    FCE.DUE_DAY             AS VADE_GUN,\r\n                                                    NVL(P.ENTITY_PRICE_GRP_CODE, ENTITY_CODE) AS FIYAT_GRUP_KOD,\r\n                                                    FE.ADDRESS1             AS ADRES1,\r\n                                                    FE.ADDRESS2             AS ADRES2,\r\n                                                    FE.ADDRESS3             AS ADRES3,\r\n                                                    FCE.NOTE1               AS ACIKLAMA,\r\n                                                    NVL(D.DISC_RATE,0)      AS ISK_1,\r\n                                                    NVL(D2.DISC_RATE,0)      AS ISK_2,\r\n                                                    FE.TEL1                 AS TEL1,\r\n                                                    FE.TEL2                 AS TEL2,\r\n                                                    FE.FAX                  AS FAX,\r\n                                                    NVL(FE.LATITUDE,'0')            AS LATITUDE,\r\n                                                    NVL(FE.LONGITUDE,'0')            AS LOGITUDE,\r\n                                                    CASE \r\n                                                      WHEN UP.ENTITY_ID IS NULL THEN 0\r\n                                                      WHEN FE.ENTITY_ID = UP.ENTITY_ID THEN 1 ELSE 0 \r\n                                                        END AS ENTITY_TYPE,\r\n                                                    NVL(RED.IS_SALES_INVOICE,1) AS IS_SALES_INVOICE,\r\n                                                    NVL(RED.IS_ONE_TO_ONE,1) AS IS_ONE_TO_ONE,\r\n                                                    NVL(RED.IS_PAYMENT1,1) AS IS_PAYMENT1,\r\n                                                    LC1.LOADING_CARD_CODE AS LOADING_CARD_CODE1,\r\n                                                    NVL(RED.IS_PAYMENT2,1) AS IS_PAYMENT2,\r\n                                                    LC2.LOADING_CARD_CODE AS LOADING_CARD_CODE2,\r\n                                                    NVL(RED.IS_PAYMENT3,1) AS IS_PAYMENT3,\r\n                                                    LC3.LOADING_CARD_CODE AS LOADING_CARD_CODE3,\r\n                                                    NVL(RED.IS_PAYMENT4,1) AS IS_PAYMENT4,\r\n                                                    LC4.LOADING_CARD_CODE AS LOADING_CARD_CODE4,\r\n                                                    NVL(RED.IS_PAYMENT5,1) AS IS_PAYMENT5,\r\n                                                    LC5.LOADING_CARD_CODE AS LOADING_CARD_CODE5,\r\n                                                    NVL(RED.IS_PAYMENT6,1) AS IS_PAYMENT6,\r\n                                                    LC6.LOADING_CARD_CODE AS LOADING_CARD_CODE6,\r\n                                                    NVL(RED.IS_SALES_RETURN_INVOICE,1) AS IS_SALES_RETURN_INVOICE,\r\n                                                    NVL(RED.IS_SALES_WAYBILL,1) AS IS_SALES_WAYBILL,\r\n                                                    NVL(FE.RADIUS,0) AS RADIUS,\r\n                                                    GRP1.ENTITY_GRP_CODE AS ENTITY_GRP_CODE1,\r\n                                                    GRP1.ENTITY_GRP_NAME AS ENTITY_GRP_NAME1,\r\n                                                    GRP2.ENTITY_GRP_CODE AS ENTITY_GRP_CODE2,\r\n                                                    GRP2.ENTITY_GRP_NAME AS ENTITY_GRP_NAME2,\r\n                                                    GRP3.ENTITY_GRP_CODE AS ENTITY_GRP_CODE3,\r\n                                                    GRP3.ENTITY_GRP_NAME AS ENTITY_GRP_NAME3,\r\n                                                    DD.DUE_DISC_CODE,\r\n                                                    NVL(RED.IS_SALES_ORDER,1) AS IS_SALES_ORDER,\r\n                                                    W.WHOUSE_CODE,\r\n                                                    NVL(RED.IS_CONSIGNE,0) AS IS_CONSIGNE,\r\n                                                    NVL(RL.MAX_LIMIT,0) AS MAX_LIMIT\r\n FROM       {0}  RED\r\n                                               INNER JOIN UYUMSOFT.HSMD_ROUTE_ENTITY_REL_M  REM ON REM.ROUTE_ENTITY_REL_M_ID   = RED.ROUTE_ENTITY_REL_M_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = RED.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR  ON BR.BRANCH_ID                = REM.BRANCH_ID\r\n LEFT JOIN UYUMSOFT.FIND_RISK_LIMIT RL ON RL.CO_ID = BR.CO_ID AND RL.ENTITY_ID = FE.ENTITY_ID \r\n INNER JOIN UYUMSOFT.HSMD_ROUTE               R   ON R.ROUTE_ID                  = REM.ROUTE_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_D RLD ON RLD.ROUTE_ID                = REM.ROUTE_ID AND RLD.BRANCH_ID = REM.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_M RLM ON RLM.SAL_PER_ROUTE_REL_M_ID  = RLD.SAL_PER_ROUTE_REL_M_ID  \r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = RLM.SALES_PERSON_ID AND RLM.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.HSMT_ROUTE_PLANING       RP  ON RP.ROUTE_ID                 = REM.ROUTE_ID AND RP.BRANCH_ID   = REM.BRANCH_ID   AND RP.ENTITY_ID = RED.ENTITY_ID     \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = BR.CO_ID\r\n                                               LEFT JOIN FIND_DUE_DISC DD ON DD.DUE_DISC_ID = FCE.DUE_DISC_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP1 ON GRP1.ENTITY_GRP_ID      = FCE.ENTITY_GRP_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP2 ON GRP2.ENTITY_GRP_ID      = FCE.ENTITY_GRP2_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP3 ON GRP3.ENTITY_GRP_ID      = FCE.ENTITY_GRP3_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC1 ON RED.LOADING_CARD_ID1 = LC1.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC2 ON RED.LOADING_CARD_ID2 = LC2.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC3 ON RED.LOADING_CARD_ID3 = LC3.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC4 ON RED.LOADING_CARD_ID4 = LC4.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC5 ON RED.LOADING_CARD_ID5 = LC5.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC6 ON RED.LOADING_CARD_ID6 = LC6.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.INVD_WHOUSE W ON W.WHOUSE_ID = FCE.WHOUSE_ID\r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID              AND UP.USR_ID = {1}\r\n                                            WHERE      BR.BRANCH_CODE      = '{2}'\r\n                                                      AND S.SALES_PERSON_CODE = '{3}'\r\n                                                      AND RLD.ISPASSIVE       = 0 \r\n                                                      AND R.ISPASSIVE         = 0 \r\n                                                      AND RP.STATUS           = 1 \r\n                                                      AND TO_DATE(TO_CHAR(RP.DOC_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{4}','dd/MM/yyyy') \r\n                                               UNION ALL\r\n                                            SELECT\r\n                                                    FE.ENTITY_CODE          AS CARI_KOD,\r\n                                                    FE.ENTITY_NAME          AS CARI_AD,\r\n                                                    FE.ENTITY_NAME_SHORT    AS CARI_KISA_AD,\r\n                                                    FE.ADDRESS1             AS FAT_ADRES1,\r\n                                                    FE.ADDRESS2             AS FAT_ADRES2,\r\n                                                    FE.ADDRESS3             AS FAT_ADRES3,\r\n                                                    C.CITY_NAME             AS SEHIR_AD,\r\n                                                    T.TOWN_NAME             AS ILCE_AD,\r\n                                                    U.COUNTRY_NAME          AS ULKE_AD,          \r\n                                                    V.DESCRIPTION           AS VERGI_DAIRE,\r\n                                                    FE.TAX_NO               AS VERGI_NO,\r\n                                                    FCE.DUE_DAY             AS VADE_GUN,\r\n                                                    NVL(P.ENTITY_PRICE_GRP_CODE, ENTITY_CODE) AS FIYAT_GRUP_KOD,\r\n                                                    FE.ADDRESS1             AS ADRES1,\r\n                                                    FE.ADDRESS2             AS ADRES2,\r\n                                                    FE.ADDRESS3             AS ADRES3,\r\n                                                    FCE.NOTE1               AS ACIKLAMA,\r\n                                                    NVL(D.DISC_RATE,0)      AS ISK_1,\r\n                                                    NVL(D2.DISC_RATE,0)      AS ISK_2,\r\n                                                    FE.TEL1                 AS TEL1,\r\n                                                    FE.TEL2                 AS TEL2,\r\n                                                    FE.FAX                  AS FAX,\r\n                                                    NVL(FE.LATITUDE,'0')            AS LATITUDE,\r\n                                                    NVL(FE.LONGITUDE,'0')            AS LOGITUDE,\r\n                                                    CASE \r\n                                                      WHEN UP.ENTITY_ID IS NULL THEN 0\r\n                                                      WHEN FE.ENTITY_ID = UP.ENTITY_ID THEN 1 ELSE 0 \r\n                                                        END AS ENTITY_TYPE,\r\n                                                    1 AS IS_SALES_INVOICE,\r\n                                                    1 AS IS_ONE_TO_ONE,\r\n                                                    1 AS IS_PAYMENT1,\r\n                                                    LC1.LOADING_CARD_CODE AS LOADING_CARD_CODE1,\r\n                                                    1 AS IS_PAYMENT2,\r\n                                                    LC2.LOADING_CARD_CODE AS LOADING_CARD_CODE2,\r\n                                                    1 AS IS_PAYMENT3,\r\n                                                    LC3.LOADING_CARD_CODE AS LOADING_CARD_CODE3,\r\n                                                    1 AS IS_PAYMENT4,\r\n                                                    LC4.LOADING_CARD_CODE AS LOADING_CARD_CODE4, \r\n                                                    1 AS IS_PAYMENT5,\r\n                                                    LC5.LOADING_CARD_CODE AS LOADING_CARD_CODE5,\r\n                                                    1 AS IS_PAYMENT6,\r\n                                                    LC6.LOADING_CARD_CODE AS LOADING_CARD_CODE6,\r\n                                                    1 AS IS_SALES_RETURN_INVOICE,\r\n                                                    1 AS IS_SALES_WAYBILL,\r\n                                                    NVL(FE.RADIUS,0) AS RADIUS,\r\n                                                    GRP1.ENTITY_GRP_CODE AS ENTITY_GRP_CODE1,\r\n                                                    GRP1.ENTITY_GRP_NAME AS ENTITY_GRP_NAME1,\r\n                                                    GRP2.ENTITY_GRP_CODE AS ENTITY_GRP_CODE2,\r\n                                                    GRP2.ENTITY_GRP_NAME AS ENTITY_GRP_NAME2,\r\n                                                    GRP3.ENTITY_GRP_CODE AS ENTITY_GRP_CODE3,\r\n                                                    GRP3.ENTITY_GRP_NAME AS ENTITY_GRP_NAME3,\r\n                                                    DD.DUE_DISC_CODE,\r\n                                                    1 AS IS_SALES_ORDER,\r\n                                                    W.WHOUSE_CODE,\r\n                                                    1 AS IS_CONSIGNE, 0 AS MAX_LIMIT  FROM       HSMT_LOADING_INSTRUCTION  LI\r\n                                               INNER JOIN HSMT_LOADING_INSTRUCTION_D LID ON LI.LOADING_INSTRUCTION_ID = LID.LOADING_INSTRUCTION_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = LID.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR ON BR.BRANCH_ID = LI.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = LI.SALES_PERSON_ID AND LI.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = LI.CO_ID\r\n                                               LEFT JOIN FIND_DUE_DISC DD ON DD.DUE_DISC_ID = FCE.DUE_DISC_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP1 ON GRP1.ENTITY_GRP_ID      = FCE.ENTITY_GRP_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP2 ON GRP2.ENTITY_GRP_ID      = FCE.ENTITY_GRP2_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP3 ON GRP3.ENTITY_GRP_ID      = FCE.ENTITY_GRP3_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID              AND UP.USR_ID = {1}\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC1 ON UP.LOADING_CARD_ID1 = LC1.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC2 ON UP.LOADING_CARD_ID2 = LC2.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC3 ON UP.LOADING_CARD_ID3 = LC3.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC4 ON UP.LOADING_CARD_ID4 = LC4.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC5 ON UP.LOADING_CARD_ID5 = LC5.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC6 ON UP.LOADING_CARD_ID6 = LC6.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.INVD_WHOUSE W ON W.WHOUSE_ID = FCE.WHOUSE_ID\r\n                                            WHERE      BR.BRANCH_CODE      = '{2}'\r\n                                                      AND S.SALES_PERSON_CODE = '{3}'\r\n                                                      AND TO_DATE(TO_CHAR(LI.LOADING_INSTRUCTION_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{4}','dd/MM/yyyy') \r\n                                                      AND FE.ENTITY_CODE NOT IN (SELECT\r\n                                                    FE.ENTITY_CODE          AS CARI_KOD\r\n  \r\n                                               FROM       {0}  RED\r\n                                               INNER JOIN UYUMSOFT.HSMD_ROUTE_ENTITY_REL_M  REM ON REM.ROUTE_ENTITY_REL_M_ID   = RED.ROUTE_ENTITY_REL_M_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = RED.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR  ON BR.BRANCH_ID                = REM.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_ROUTE               R   ON R.ROUTE_ID                  = REM.ROUTE_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_D RLD ON RLD.ROUTE_ID                = REM.ROUTE_ID AND RLD.BRANCH_ID = REM.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.HSMD_SAL_PER_ROUTE_REL_M RLM ON RLM.SAL_PER_ROUTE_REL_M_ID  = RLD.SAL_PER_ROUTE_REL_M_ID  \r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = RLM.SALES_PERSON_ID AND RLM.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.HSMT_ROUTE_PLANING       RP  ON RP.ROUTE_ID                 = REM.ROUTE_ID AND RP.BRANCH_ID   = REM.BRANCH_ID   AND RP.ENTITY_ID = RED.ENTITY_ID     \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = BR.CO_ID\r\n                                               LEFT JOIN FIND_DUE_DISC DD ON DD.DUE_DISC_ID = FCE.DUE_DISC_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP1 ON GRP1.ENTITY_GRP_ID      = FCE.ENTITY_GRP_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP2 ON GRP2.ENTITY_GRP_ID      = FCE.ENTITY_GRP2_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_GROUP GRP3 ON GRP3.ENTITY_GRP_ID      = FCE.ENTITY_GRP3_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{4}','dd/MM/yyyy') AND TO_DATE('{4}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC1 ON RED.LOADING_CARD_ID1 = LC1.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC2 ON RED.LOADING_CARD_ID2 = LC2.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC3 ON RED.LOADING_CARD_ID3 = LC3.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC4 ON RED.LOADING_CARD_ID4 = LC4.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC5 ON RED.LOADING_CARD_ID5 = LC5.LOADING_CARD_ID\r\n                                               LEFT JOIN UYUMSOFT.HSMD_LOADING_CARD LC6 ON RED.LOADING_CARD_ID6 = LC6.LOADING_CARD_ID\r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID              AND UP.USR_ID = {1}\r\n                                            WHERE      BR.BRANCH_CODE      = '{2}'\r\n                                                      AND S.SALES_PERSON_CODE = '{3}'\r\n                                                      AND RLD.ISPASSIVE       = 0 \r\n                                                      AND R.ISPASSIVE         = 0 \r\n                                                      AND RP.STATUS           = 1 \r\n                                                      AND TO_DATE(TO_CHAR(RP.DOC_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{4}','dd/MM/yyyy') )\r\n                                                ) t\r\n                                               ";
                object[] objArray1 = new object[5]
                {
          (object) "UYUMSOFT.HSMD_ROUTE_ENTITY_REL_D",
          (object) coBranchId,
          (object) token.BranchCode,
          (object) userParameters.SalesPersonCode,
          null
                };
                int index1 = 4;
                DateTime today = DateTime.Today;
                string str1 = today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo);
                objArray1[index1] = (object)str1;
                string str2 = string.Format(format1, objArray1);
                EventLog.WriteEntry("Application", str2, EventLogEntryType.Information, 1283);

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, str2);

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    Entity entity = new Entity();
                    entity.EntityCode = ConvertToString(oracleDataReader[0]);
                    entity.EntityName = ConvertToString(oracleDataReader[1]);
                    entity.ShortEntityName = ConvertToString(oracleDataReader[2]);
                    entity.ShippingAddress1 = ConvertToString(oracleDataReader[3]);
                    entity.ShippingAddress2 = ConvertToString(oracleDataReader[4]);
                    entity.ShippingAddress3 = ConvertToString(oracleDataReader[5]);
                    entity.CityName = ConvertToString(oracleDataReader[6]);
                    entity.TownName = ConvertToString(oracleDataReader[7]);
                    entity.CountryName = ConvertToString(oracleDataReader[8]);
                    entity.TaxOfficeCode = ConvertToString(oracleDataReader[9]);
                    entity.TaxNo = ConvertToString(oracleDataReader[10]);
                    entity.DueDay = ConvertToInt32(oracleDataReader[11]);
                    entity.PriceListGroupCode = ConvertToString(oracleDataReader[12]);
                    entity.Address1 = ConvertToString(oracleDataReader[13]);
                    entity.Address2 = ConvertToString(oracleDataReader[14]);
                    entity.Address3 = ConvertToString(oracleDataReader[15]);
                    entity.Note = ConvertToString(oracleDataReader[16]);
                    entity.DiscRate1 = ConvertToDecimal(oracleDataReader[17]);
                    entity.DiscRate2 = ConvertToDecimal(oracleDataReader[18]);
                    entity.Tel1 = ConvertToString(oracleDataReader[19]);
                    entity.Tel2 = ConvertToString(oracleDataReader[20]);
                    entity.Fax = ConvertToString(oracleDataReader[21]);
                    string str3 = ConvertToString(oracleDataReader[22]);
                    string str4 = ConvertToString(oracleDataReader[23]);
                    string decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                    if (decimalSeparator == ".")
                    {
                        str3 = str3.Replace(',', '.');
                        str4 = str4.Replace(',', '.');
                    }
                    else if (decimalSeparator == ",")
                    {
                        str3 = str3.Replace('.', ',');
                        str4 = str4.Replace('.', ',');
                    }
                    entity.Latitude = ConvertToDecimal(str3);
                    entity.Longitude = ConvertToDecimal(str4);
                    entity.EntityType = ConvertToInt32(oracleDataReader[24]);
                    entity.IsInvoice = ConvertToBoolean(ConvertToInt32(oracleDataReader[25]));
                    entity.IsOneToOne = ConvertToBoolean(ConvertToInt32(oracleDataReader[26]));
                    entity.IsPayment1 = ConvertToBoolean(ConvertToInt32(oracleDataReader[27]));
                    entity.LoadingCard1 = ConvertToString(oracleDataReader[28]);
                    entity.IsPayment2 = ConvertToBoolean(ConvertToInt32(oracleDataReader[29]));
                    entity.LoadingCard2 = ConvertToString(oracleDataReader[30]);
                    entity.IsPayment3 = ConvertToBoolean(ConvertToInt32(oracleDataReader[31]));
                    entity.LoadingCard3 = ConvertToString(oracleDataReader[32]);
                    entity.IsPayment4 = ConvertToBoolean(ConvertToInt32(oracleDataReader[33]));
                    entity.LoadingCard4 = ConvertToString(oracleDataReader[34]);
                    entity.IsPayment5 = ConvertToBoolean(ConvertToInt32(oracleDataReader[35]));
                    entity.LoadingCard5 = ConvertToString(oracleDataReader[36]);
                    entity.IsPayment6 = ConvertToBoolean(ConvertToInt32(oracleDataReader[37]));
                    entity.LoadingCard6 = ConvertToString(oracleDataReader[38]);
                    entity.IsReturnWaybill = ConvertToBoolean(ConvertToInt32(oracleDataReader[39]));
                    entity.IsWaybill = ConvertToBoolean(ConvertToInt32(oracleDataReader[40]));
                    entity.Radius = (Decimal)ConvertToInt32(oracleDataReader[41]);
                    entity.BranchCode = token.BranchCode;
                    entity.EntityGrpCode1 = ConvertToString(oracleDataReader[42]);
                    entity.EntityGrpName1 = ConvertToString(oracleDataReader[43]);
                    entity.EntityGrpCode2 = ConvertToString(oracleDataReader[44]);
                    entity.EntityGrpName2 = ConvertToString(oracleDataReader[45]);
                    entity.EntityGrpCode3 = ConvertToString(oracleDataReader[46]);
                    entity.EntityGrpName3 = ConvertToString(oracleDataReader[47]);
                    entity.DueDiscCode = ConvertToString(oracleDataReader[48]);
                    entity.IsOrder = ConvertToBoolean(ConvertToInt32(oracleDataReader[49]));
                    entity.WhouseCode = ConvertToString(oracleDataReader[50]);
                    entity.IsConsigne = ConvertToBoolean(oracleDataReader[51]);
                    entity.LineNo = 0;// ConvertToDecimal(oracleDataReader[52]);

                    try
                    {
                        string str33 = ConvertToString(oracleDataReader[52]);
                        if (decimalSeparator == ".")
                            str33 = str33.Replace(',', '.');
                        else if (decimalSeparator == ",")
                            str33 = str33.Replace('.', ',');
                        entity.MaxLimit = Convert.ToDecimal(str33);
                    }
                    catch { }

                    if (!dictionary.ContainsKey(entity.EntityCode))
                    {
                        dictionary.Add(entity.EntityCode, entity);
                    }
                }
                if (dictionary.Count == 0)
                    throw new Exception("Hiç cari bulunamamıştır. Rota planlamayı kontrol ediniz!!!");
                string[] strArray = new string[dictionary.Count];
                string inFilter = Helper._GenerateInFilter(dictionary.Keys.ToArray<string>(), "FE.ENTITY_CODE");
                string format2 = "SELECT\r\n                                                            FE.ENTITY_CODE             AS CARI_KOD,\r\n                                                            NVL(SUM(FD.PLUS_MINUS*FD.AMT),0)  AS BAKIYE,\r\n                                                            NVL(SUM(CASE WHEN TO_DATE(TO_CHAR(FD.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') = TO_DATE('{3}','dd/MM/yyyy') THEN FD.PLUS_MINUS*FD.AMT ELSE 0 END),0)  AS BAKIYE_GUN\r\n   \r\n                                                            FROM       {0}      FE\r\n                                                               INNER JOIN UYUMSOFT.FINT_FIN_D       FD  ON FD.ENTITY_ID  = FE.ENTITY_ID\r\n                                                               INNER JOIN UYUMSOFT.GNLD_BRANCH      BR  ON BR.BRANCH_ID  = FD.BRANCH_ID\r\n         \r\n                    INNER JOIN FINP_FIN_PARAMETER FP ON FP.CO_ID = FD.CO_ID \r\n         \r\n                                        WHERE     BR.BRANCH_CODE      = '{1}'\r\n                                                                      AND {2}\r\n                                                                      AND TO_DATE(TO_CHAR(FD.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') <= TO_DATE('{3}','dd/MM/yyyy')\r\n          \r\n                                           AND TO_DATE(TO_CHAR(FP.ENTITY_PERIOD_START_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy') <= TO_DATE(TO_CHAR(FD.DOC_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy')  \r\n          \r\n               GROUP BY FE.ENTITY_CODE\r\n                                                              ";
                object[] objArray2 = new object[4]
                {
          (object) "UYUMSOFT.FIND_ENTITY",
          (object) token.BranchCode,
          (object) inFilter,
          null
                };
                int index2 = 3;
                today = DateTime.Today;
                string str5 = today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo);
                objArray2[index2] = (object)str5;

                // kapat ve temizle
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, string.Format(format2, objArray2));

                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    string index3 = ConvertToString(oracleDataReader[0]);
                    dictionary[index3].Balance = ConvertToDecimal(oracleDataReader[1]);
                    dictionary[index3].DailyBalance = ConvertToDecimal(oracleDataReader[2]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            List<Entity> entityList = new List<Entity>();
            entityList.AddRange((IEnumerable<Entity>)dictionary.Values);
            return entityList.ToArray();
        }

        internal static EntityItem[] GetEntityItems(HotSaleServiceTables.Token token, string[] entityCodes, string[] itemCodes, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<EntityItem> entityItemList = new List<EntityItem>();
            try
            {
                Helper.GetUserParameters(token, conn);
                if (entityCodes.Length == 0)
                    throw new Exception("Cari kodları boş olamaz. Cari stok tanımları alınamadı.");
                string inFilter1 = Helper._GenerateInFilter(entityCodes, "FE.ENTITY_CODE");
                if (itemCodes.Length == 0)
                    throw new Exception("Stok kodları boş olamaz. Cari stok tanımları alınamadı.");
                string inFilter2 = Helper._GenerateInFilter(itemCodes, "I.ITEM_CODE");

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT                                                      \r\n                                                        FE.ENTITY_CODE          AS CARI_KOD,\r\n                                                        I.ITEM_CODE             AS STOK_KOD,\r\n                                                        D.CROSS_CODE            AS CAPRAZ_KOD,\r\n                                                        D.CROSS_DESC            AS CAPRAZ_AD,\r\n                                                        BR.BRANCH_CODE          AS BRANCH_CODE\r\n  \r\n                                                   FROM       {0}          FE\r\n                                                   INNER JOIN UYUMSOFT.FIND_CO_ENTITY       FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID \r\n                                                   INNER JOIN UYUMSOFT.GNLD_BRANCH          BR     ON BR.CO_ID              = FCE.CO_ID\r\n                                                   INNER JOIN UYUMSOFT.INVT_CROSS_REF_M     M      ON M.ENTITY_ID           = FE.ENTITY_ID\r\n                                                   INNER JOIN UYUMSOFT.INVT_CROSS_REF_D     D      ON D.CROSS_REF_M_ID      = M.CROSS_REF_M_ID\r\n                                                   INNER JOIN UYUMSOFT.INVD_ITEM            I      ON I.ITEM_ID             = D.ITEM_ID\r\n        \r\n                                                   WHERE   BR.BRANCH_CODE      = '{1}'   \r\n                                                        AND {2}\r\n\t                                                    AND {3}\r\n                                                        \r\n                                                ", (object)"UYUMSOFT.FIND_ENTITY", (object)token.BranchCode, (object)inFilter1, (object)inFilter2);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    EntityItem entityItem = new EntityItem()
                    {
                        EntityCode = ConvertToString(oracleDataReader[0]),
                        ItemCode = ConvertToString(oracleDataReader[1]),
                        EntityItemCode = ConvertToString(oracleDataReader[2]),
                        EntityItemName = ConvertToString(oracleDataReader[3]),
                        BranchCode = ConvertToString(oracleDataReader[4])
                    };
                    entityItemList.Add(entityItem);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return entityItemList.ToArray();
        }

        internal static PriceList[] GetEntityPriceList(HotSaleServiceTables.Token token, string[] priceGroupCodeList, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<PriceList> priceListList = new List<PriceList>();
            try
            {
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                List<string> list = ((IEnumerable<string>)priceGroupCodeList).ToList<string>().Distinct<string>().ToList<string>();
                if (userParameters == null)
                    throw new Exception("Invalid token!");
                foreach (string str in list)
                {
                    oracleCommand = conn.CreateCommand();
                    // 15.05.2023 string tmp = string.Format("SELECT\r\n I.ITEM_CODE AS STOK_KOD,\r\n NVL(FD.UNIT_PRICE_TRA,0) AS BIRIM_FIYAT,\r\n NVL(FD.DISC1_RATE,0) AS ISK_1,\r\n NVL(FD.DISC2_RATE,0) AS ISK_2,\r\n NVL(FE.ENTITY_CODE,P.ENTITY_PRICE_GRP_CODE) AS GRUP_KOD ,\r\n U.UNIT_CODE,\r\n NVL(FD.VAT_STATUS,0) AS VAT_STATUS,\r\n RULE.RULE_CODE FROM {0} FD\r\n INNER JOIN UYUMSOFT.INVT_PRICE_LIST_M FM ON FM.PRICE_LIST_M_ID = FD.PRICE_LIST_M_ID\r\n INNER JOIN UYUMSOFT.GNLD_BRANCH BR ON BR.CO_ID = FM.CO_ID\r\n INNER JOIN UYUMSOFT.INVD_ITEM I ON I.ITEM_ID = FD.ITEM_ID\r\n LEFT JOIN UYUMSOFT.FIND_ENTITY FE ON FE.ENTITY_ID = FM.ENTITY_ID\r\n LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FM.ENTITY_PRICE_GRP_ID AND P.CO_ID = FM.CO_ID \r\n INNER JOIN UYUMSOFT.INVD_UNIT U ON U.UNIT_ID = FD.UNIT_ID\r\n LEFT JOIN INVD_PRICE_RULE_M RULE ON RULE.PRICE_RULE_M_ID = FD.RULE_ID\r\n WHERE BR.BRANCH_CODE = '{1}'\r\n AND TO_DATE('{2}','dd/MM/yyyy') >= TO_DATE(TO_CHAR(FD.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n AND TO_DATE('{2}','dd/MM/yyyy') <= TO_DATE(TO_CHAR(FD.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') \r\n AND FM.ISPASSIVE = 0\r\n AND FM.PURCHASE_SALES = 2\r\n AND NVL(FE.ENTITY_CODE, P.ENTITY_PRICE_GRP_CODE) = '{3}'\r\n",
                    string tmp = string.Format("SELECT\r\n I.ITEM_CODE AS STOK_KOD,\r\n NVL(FD.UNIT_PRICE_TRA,0) AS BIRIM_FIYAT,\r\n NVL(FD.DISC1_RATE,0) AS ISK_1,\r\n NVL(FD.DISC2_RATE,0) AS ISK_2,\r\n NVL(FE.ENTITY_CODE,P.ENTITY_PRICE_GRP_CODE) AS GRUP_KOD ,\r\n U.UNIT_CODE,\r\n NVL(FD.VAT_STATUS,0) AS VAT_STATUS,\r\n PL.PRICE_LIST_CODE AS RULE_CODE\r\n FROM {0} FD\r\n INNER JOIN UYUMSOFT.INVT_PRICE_LIST_M FM ON FM.PRICE_LIST_M_ID = FD.PRICE_LIST_M_ID\r\n INNER JOIN UYUMSOFT.GNLD_BRANCH BR ON BR.CO_ID = FM.CO_ID\r\n INNER JOIN UYUMSOFT.INVD_ITEM I ON I.ITEM_ID = FD.ITEM_ID\r\n LEFT JOIN UYUMSOFT.FIND_ENTITY FE ON FE.ENTITY_ID = FM.ENTITY_ID\r\n LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FM.ENTITY_PRICE_GRP_ID AND P.CO_ID = FM.CO_ID \r\n INNER JOIN UYUMSOFT.INVD_UNIT U ON U.UNIT_ID = FD.UNIT_ID\r\n INNER JOIN UYUMSOFT.INVD_PRICE_LIST PL ON PL.PRICE_LIST_ID = FM.PRICE_LIST_ID\r\n LEFT JOIN INVD_PRICE_RULE_M RULE ON RULE.PRICE_RULE_M_ID = FD.RULE_ID\r\n WHERE BR.BRANCH_CODE = '{1}'\r\n AND TO_DATE('{2}','dd/MM/yyyy') >= TO_DATE(TO_CHAR(FD.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n AND TO_DATE('{2}','dd/MM/yyyy') <= TO_DATE(TO_CHAR(FD.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') \r\n AND FM.ISPASSIVE = 0\r\n AND FM.PURCHASE_SALES = 2\r\n AND NVL(FE.ENTITY_CODE, P.ENTITY_PRICE_GRP_CODE) = '{3}'\r\n",
                        (object)"UYUMSOFT.INVT_PRICE_LIST_D", 
                        (object)token.BranchCode, 
                        (object)DateTime.Today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), 
                        (object)str
                    );

                    oracleCommand.CommandText = GetIsNullCommandReplace(conn, tmp);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                    {
                        PriceList priceList = new PriceList()
                        {
                            ItemCode = ConvertToString(oracleDataReader[0]),
                            Price = ConvertToDecimal(oracleDataReader[1]),
                            DiscRate1 = ConvertToDecimal(oracleDataReader[2]),
                            DiscRate2 = ConvertToDecimal(oracleDataReader[3]),
                            EntityGroupCode = ConvertToString(oracleDataReader[4]),
                            UnitCode = ConvertToString(oracleDataReader[5]),
                            BranchCode = token.BranchCode,
                            RuleCode = ConvertToString(oracleDataReader[7]) // 14.03.2023
                        };
                        if (userParameters.IsUseVatStatus)
                        {
                            switch (ConvertToInt32(oracleDataReader[6]))
                            {
                                case 1:
                                    priceList.VatStatus = true;
                                    break;
                                case 2:
                                    priceList.VatStatus = false;
                                    break;
                            }
                        }
                        else
                            priceList.VatStatus = false;
                        priceListList.Add(priceList);
                    }

                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return priceListList.ToArray();
        }

        internal static Item[] GetItems(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<Item> objList = new List<Item>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                string tmp = string.Format("SELECT\r\n                                                        I.ITEM_CODE                                 AS STOK_KOD,\r\n                                                        I.ITEM_NAME                                 AS STOK_AD,\r\n I.ITEM_NAME2                                 AS STOK_AD2,\r\n                                                       NVL(T2.TAX_RATE,0)                          AS KDV,\r\n                                                        U.UNIT_CODE                                 AS BIRIM,\r\n                                                        CAT1.CATEGORIES_CODE AS CATEGORIES1_CODE,\r\n                                                        CAT1.DESCRIPTION     AS CATEGORIES1_DESC,\r\n                                                        CAT2.CATEGORIES_CODE AS CATEGORIES2_CODE,\r\n                                                        CAT2.DESCRIPTION     AS CATEGORIES2_DESC,\r\n                                                        CAT3.CATEGORIES_CODE AS CATEGORIES3_CODE,\r\n                                                        CAT3.DESCRIPTION     AS CATEGORIES3_DESC,\r\n                                                        CAT4.CATEGORIES_CODE AS CATEGORIES3_CODE,\r\n                                                        CAT4.DESCRIPTION     AS CATEGORIES3_DESC,\r\n                                                        BRND.BRAND_CODE AS BRAND_CODE,\r\n                                                        NVL(I.QTY_PRECISION,0) AS QTY_PRECISION,\r\n                                                        IDG.ITEM_DUE_DISC_GRP_CODE,\r\n                                                        NVL(I.ADD_DEC01,0) AS ADD_DEC01\r\n, (SELECT UNIT.UNIT_CODE FROM UYUMSOFT.INVD_UNIT UNIT WHERE UNIT.UNIT_ID = NVL( I.SALES_UNIT_ID, 0)) AS SALES_UNIT_CODE \r\n                                                      FROM       {0}            I\r\n                                                           INNER JOIN UYUMSOFT.INVD_UNIT            U   ON U.UNIT_ID         = I.UNIT_ID\r\n                                                           INNER JOIN UYUMSOFT.INVD_BRANCH_ITEM     BI  ON BI.ITEM_ID = I.ITEM_ID\r\n                                                           INNER JOIN INVD_BWH_ITEM                 BW  ON BW.ITEM_ID = I.ITEM_ID\r\n                                                           INNER JOIN UYUMSOFT.INVD_WHOUSE          W   ON W.WHOUSE_ID       = BW.WHOUSE_ID\r\n                                                           INNER JOIN UYUMSOFT.GNLD_BRANCH          BR  ON BR.BRANCH_ID      = BI.BRANCH_ID\r\n                                                           LEFT OUTER JOIN INVD_ITEM_DUE_DISC_GRP IDG ON IDG.ITEM_DUE_DISC_GRP_ID = I.I_DUE_DISC_GRP_ID \r\n                                                           LEFT JOIN UYUMSOFT.INVD_BRAND BRND ON BRND.BRAND_ID = I.BRAND_ID\r\n                                                           LEFT  JOIN UYUMSOFT.INVD_ITEM_TAX        T   ON T.ITEM_ID         = I.ITEM_ID  \r\n                                                                    AND T.PURCHASE_SALES = 2  AND TO_DATE(TO_CHAR(T.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') <= TRUNC(CURRENT_DATE) AND TRUNC(CURRENT_DATE)<=TO_DATE(TO_CHAR(T.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                                           LEFT JOIN  UYUMSOFT.INVD_TAX             T2  ON T2.TAX_ID         = T.TAX_ID\r\n                                                           LEFT JOIN UYUMSOFT.GNLD_CATEGORIES CAT1 ON CAT1.CATEGORIES_ID = I.CATEGORIES1_ID\r\n                                                           LEFT JOIN UYUMSOFT.GNLD_CATEGORIES CAT2 ON CAT2.CATEGORIES_ID = I.CATEGORIES2_ID\r\n                                                           LEFT JOIN UYUMSOFT.GNLD_CATEGORIES CAT3 ON CAT3.CATEGORIES_ID = I.CATEGORIES3_ID\r\n                                                           LEFT JOIN UYUMSOFT.GNLD_CATEGORIES CAT4 ON CAT4.CATEGORIES_ID = I.CATEGORIES4_ID\r\n      \r\n                                                      WHERE      BR.BRANCH_CODE = '{1}' AND\r\n                                                                 W.WHOUSE_CODE  = '{2}'  \r\n                                                        ", (object)"UYUMSOFT.INVD_ITEM", (object)token.BranchCode, (object)userParameters.VehicleWhouseCode);
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, tmp);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    Item obj = new Item()
                    {
                        ItemCode = ConvertToString(oracleDataReader[0]),
                        ItemName = ConvertToString(oracleDataReader[1]),
                        ItemName2 = ConvertToString(oracleDataReader[2]),
                        SVatRate = (Decimal)ConvertToInt32(oracleDataReader[3]),
                        UnitCode = ConvertToString(oracleDataReader[4]),
                        BranchCode = token.BranchCode,
                        Categories1Code = ConvertToString(oracleDataReader[5]),
                        Categories1Desc = ConvertToString(oracleDataReader[6]),
                        Categories2Code = ConvertToString(oracleDataReader[7]),
                        Categories2Desc = ConvertToString(oracleDataReader[8]),
                        Categories3Code = ConvertToString(oracleDataReader[9]),
                        Categories3Desc = ConvertToString(oracleDataReader[10]),
                        Categories4Code = ConvertToString(oracleDataReader[11]),
                        Categories4Desc = ConvertToString(oracleDataReader[12]),
                        BrandCode = ConvertToString(oracleDataReader[13]),
                        QtyPrecision = ConvertToInt32(oracleDataReader[14]),
                        ItemDueDiscCode = ConvertToString(oracleDataReader[15]),
                        AddDec01 = ConvertToDecimal(oracleDataReader[16]),
                        SalesUnitCode = ConvertToString(oracleDataReader[17])
                    };
                    objList.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return objList.ToArray();
        }

        internal static ItemBarcode[] GetItemBarcodes(HotSaleServiceTables.Token token, string[] itemCodes, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<ItemBarcode> itemBarcodeList = new List<ItemBarcode>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                if (itemCodes.Length == 0)
                    throw new Exception("Item Codes can not be empty");
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT\r\n                                                        I.ITEM_CODE                                 AS STOK_KOD,\r\n                                                        B.BARCODE                                   AS BARKOD\r\n \r\n                                                       FROM       {0}            I\r\n                                                           INNER JOIN UYUMSOFT.INVD_ITEM_BARCODE    B   ON B.ITEM_ID = I.ITEM_ID  AND B.LINE_NO = 1\r\n  \r\n                                                       WHERE     {1}\r\n                                                        ", (object)"UYUMSOFT.INVD_ITEM", (object)Helper._GenerateInFilter(itemCodes, "I.ITEM_CODE"));
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    ItemBarcode itemBarcode = new ItemBarcode()
                    {
                        ItemCode = ConvertToString(oracleDataReader[0]),
                        Barcode = ConvertToString(oracleDataReader[1]),
                        BranchCode = token.BranchCode
                    };
                    itemBarcodeList.Add(itemBarcode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return itemBarcodeList.ToArray();
        }

        internal static ItemTransaction[] GetItemTransactions(HotSaleServiceTables.Token token, IDbConnection conn, string loadingCardCode, string stokTarihi)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<ItemTransaction> itemTransactionList = new List<ItemTransaction>();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                DateTime dateTime = DateTime.Today;
                EventLog.WriteEntry("Application", "stokTarihi : " + stokTarihi, EventLogEntryType.Information, 213);
                if (!string.IsNullOrEmpty(stokTarihi))
                    dateTime = DateTime.ParseExact(stokTarihi, "dd.MM.yyyy", (IFormatProvider)CultureInfo.CurrentCulture);
                string str = string.Format("SELECT\r\n                                                            I.ITEM_CODE     AS STOK_KOD,\r\n                                                            SUM(D.QTY)  AS MIKTAR,\r\n                                                            SUM(D.QTY_PRM)  AS MIKTAR_PRM,\r\n                                                            U.UNIT_CODE,\r\n                                                            MAX(CAT1.CAT_CODE) AS CAT_CODE1,\r\n                                                            MAX(CAT2.CAT_CODE) AS CAT_CODE2\r\n                                                             FROM       {0}     I\r\n                                                               INNER JOIN UYUMSOFT.INVT_ITEM_D   D      ON D.ITEM_ID      = I.ITEM_ID \r\n                                                               INNER JOIN UYUMSOFT.INVD_WHOUSE   W      ON W.WHOUSE_ID    = D.WHOUSE_ID\r\n                                                               INNER JOIN UYUMSOFT.GNLD_BRANCH   BR     ON BR.BRANCH_ID   = D.BRANCH_ID\r\n                                                               INNER JOIN UYUMSOFT.INVT_ITEM_M   M      ON M.ITEM_M_ID    = D.ITEM_M_ID\r\n                                                               INNER JOIN UYUMSOFT.HSMD_LOADING_CARD LC ON LC.LOADING_CARD_ID = M.LOADING_CARD_ID\r\n                                                               INNER JOIN UYUMSOFT.INVD_UNIT U ON U.UNIT_ID = D.UNIT_ID\r\n                                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON SP ON M.SALES_PERSON_ID = SP.SALES_PERSON_ID\r\n                                                               LEFT OUTER JOIN UYUMSOFT.GNLD_CATEGORY CAT1 ON CAT1.CAT_CODE_ID = M.CAT_CODE1_ID\r\n                                                               LEFT OUTER JOIN UYUMSOFT.GNLD_CATEGORY CAT2 ON CAT2.CAT_CODE_ID = M.CAT_CODE2_ID\r\n  \r\n                                                             WHERE    BR.BRANCH_CODE = '{1}' \r\n                                                                      AND D.PLUS_MINUS   = 1\r\n                                                                      AND D.STOCK_MOVE_TYPE = 1\r\n                                                                      AND W.WHOUSE_CODE  ='{2}'\r\n                                                                      AND TO_DATE(TO_CHAR(D.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') = TO_DATE('{3}','dd/MM/yyyy')\r\n                                                                      AND LC.LOADING_CARD_CODE = '{4}'\r\n                                                                      AND SP.SALES_PERSON_CODE = '{5}'\r\n                                                             GROUP BY BR.BRANCH_CODE, W.WHOUSE_CODE, I.ITEM_CODE, U.UNIT_CODE ", (object)"UYUMSOFT.INVD_ITEM", (object)token.BranchCode, (object)userParameters.VehicleWhouseCode, (object)dateTime.ToString("dd/MM/yyyy"), (object)loadingCardCode, (object)userParameters.SalesPersonCode);
                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 214);

                // 12.01.2023
                if (userParameters.IsVehicleItem)
                {
                    str = string.Format(@"	SELECT 
		                I.ITEM_CODE AS STOK_KOD,
		                B.QTY_PRM AS MIKTAR,
		                B.QTY_PRM AS MIKTAR_PRM,
		                U.UNIT_CODE,
	                  '' AS CAT_CODE1,
	                  '' AS CAT_CODE2 		
	                FROM {2} B
	                INNER JOIN UYUMSOFT.INVD_WHOUSE W ON W.WHOUSE_ID = B.WHOUSE_ID
	                INNER JOIN UYUMSOFT.INVD_ITEM I ON I.ITEM_ID = B.ITEM_ID	
	                INNER JOIN UYUMSOFT.GNLD_BRANCH BR ON BR.BRANCH_ID = B.BRANCH_ID	
	                INNER JOIN UYUMSOFT.INVD_UNIT U ON U.UNIT_ID = I.UNIT_ID
	                WHERE 	
	                BR.BRANCH_CODE = '{0}' 
	                AND W.WHOUSE_CODE = '{1}'",
                        (object)token.BranchCode, 
                        (object)userParameters.VehicleWhouseCode,
                        "UYUMSOFT.INVD_BWH_ITEM"
                    );
                }

                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = str;
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    ItemTransaction itemTransaction = new ItemTransaction()
                    {
                        ItemCode = ConvertToString(oracleDataReader[0]),
                        Qty = ConvertToDecimal(oracleDataReader[1]),
                        QtyPrm = ConvertToDecimal(oracleDataReader[2]),
                        UnitCode = ConvertToString(oracleDataReader[3]),
                        CatCode1 = ConvertToString(oracleDataReader[4]),
                        CatCode2 = ConvertToString(oracleDataReader[5]),
                        PlusMinus = HotSaleServiceTables.PlusMinus.Giris,
                        WhouseCode = userParameters.VehicleWhouseCode,
                        BranchCode = token.BranchCode
                    };
                    itemTransactionList.Add(itemTransaction);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return itemTransactionList.ToArray();
        }

        internal static PrintDesign[] GetPrintDesign(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<PrintDesign> printDesignList = new List<PrintDesign>();
            try
            {
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return printDesignList.ToArray();
        }

        internal static SystemSettings GetSystemSettings(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            SystemSettings systemSettings = new SystemSettings();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT\r\n                                                    'CIskG'   AS ISKONTO_ONCELIK \r\n  \r\n                                                    FROM       {0}      BR       \r\n                                                    WHERE      BR.BRANCH_CODE      = '{1}'\r\n                                                     ", (object)"UYUMSOFT.GNLD_BRANCH", (object)token.BranchCode);
                oracleDataReader = oracleCommand.ExecuteReader();
                string empty = string.Empty;
                while (oracleDataReader.Read() && string.IsNullOrEmpty(empty))
                    empty = ConvertToString(oracleDataReader[0]);
                systemSettings.DiscountPriority = empty.Equals("CIskG");
                systemSettings.SystemDate = DateTime.Today;
                systemSettings.BranchCode = token.BranchCode;
                systemSettings.IsActivityEnabled = userParameters.IsActivityEnabled;
                systemSettings.IsFastOrderEnabled = userParameters.IsFastOrderEnabled;
                systemSettings.IsOneToOne = userParameters.IsOneToOne;
                systemSettings.IsPayment = userParameters.IsPayment;
                systemSettings.IsSalesInvoice = userParameters.IsSalesInvoice;
                systemSettings.IsSalesOrder = userParameters.IsSalesOrder;
                systemSettings.IsSalesReturnWaybill = userParameters.IsSalesReturnWaybill;
                systemSettings.IsSalesWaybill = userParameters.IsSalesWaybill;
                systemSettings.IsSaveRealOrder = userParameters.IsSaveRealOrder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return systemSettings;
        }

        internal static LoadingCard GetLoadingCards(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            LoadingCard loadingCard = new LoadingCard();
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                loadingCard.LoadingCardNo1 = userParameters.LoadingCardNo1;
                loadingCard.LoadingCardNo1Name = userParameters.LoadingCardNo1Name;
                loadingCard.LoadingCardNo1Order = userParameters.LoadingCardNo1Order;
                loadingCard.LoadingCardNo2 = userParameters.LoadingCardNo2;
                loadingCard.LoadingCardNo2Name = userParameters.LoadingCardNo2Name;
                loadingCard.LoadingCardNo2Order = userParameters.LoadingCardNo2Order;
                loadingCard.LoadingCardNo3 = userParameters.LoadingCardNo3;
                loadingCard.LoadingCardNo3Name = userParameters.LoadingCardNo3Name;
                loadingCard.LoadingCardNo3Order = userParameters.LoadingCardNo3Order;
                loadingCard.LoadingCardNo4 = userParameters.LoadingCardNo4;
                loadingCard.LoadingCardNo4Name = userParameters.LoadingCardNo4Name;
                loadingCard.LoadingCardNo4Order = userParameters.LoadingCardNo4Order;
                loadingCard.LoadingCardNo5 = userParameters.LoadingCardNo5;
                loadingCard.LoadingCardNo5Name = userParameters.LoadingCardNo5Name;
                loadingCard.LoadingCardNo5Order = userParameters.LoadingCardNo5Order;
                loadingCard.LoadingCardNo6 = userParameters.LoadingCardNo6;
                loadingCard.LoadingCardNo6Name = userParameters.LoadingCardNo6Name;
                loadingCard.LoadingCardNo6Order = userParameters.LoadingCardNo6Order;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return loadingCard;
        }

        internal static OrderM[] GetOrderM(HotSaleServiceTables.Token token, IDbConnection conn, string[] entityCodes, string siparisSevkTarihi, string loadingCardCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OrderM> orderMList = new List<OrderM>();
            CultureInfo cultureInfo = new CultureInfo("TR-tr");
            try
            {
                int currentLoadingCardId = Helper.GetCurrentLoadingCardId(token, conn, loadingCardCode);
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (entityCodes.Length == 0)
                    throw new Exception("Entity Codes can not be empty");
                DateTime dateTime = DateTime.Today;
                if (!string.IsNullOrEmpty(siparisSevkTarihi))
                    dateTime = ConvertToDateTime(dateTime);

                //string inFilter = Helper._GenerateInFilter(entityCodes, "FE.ENTITY_CODE");
                //Helper._GenerateNotInFilter(entityCodes, "FE.ENTITY_CODE");
                //string str = string.Format("SELECT DISTINCT * FROM (SELECT DISTINCT\r\n                                                        M.ORDER_M_ID                                AS MASTER_NO,\r\n                                                        M.AMT_VAT                                   AS KDV_TUTAR,\r\n                                                        M.AMT                                       AS MAL_TUTAR,\r\n                                                        M.AMT_DISC_TOTAL                            AS ISKONTO_TUTAR,\r\n                                                        M.DOC_DATE                                  AS SIPARIS_TARİH,\r\n                                                        M.DOC_NO                                    AS SIPARIS_NO,\r\n                                                        FE.ENTITY_CODE                              AS CARI_KOD,\r\n                                                        M.DUE_DATE                                  AS VADE_TARIH,\r\n                                                        M.DUE_DAY                                   AS VADE_GUN,\r\n                                                        M.SHIPPING_DATE                             AS YUKLEME_TARIH,\r\n                                                        M.LATITUDE                                  AS LATITUDE,\r\n                                                        M.LONGITUDE                                 AS LONGITUDE,\r\n                                                        ''                                          AS DEPO_KOD,\r\n                                                        CASE M.PURCHASE_SALES WHEN 1 THEN 'Alis' WHEN 2 THEN 'Satis' WHEN 3 THEN 'SatisIade' ELSE 'AlisIade' END AS ALIS_SATIS,\r\n                                                        M.AMT_RECEIPT,\r\n                                                        CASE NVL(DT.IS_CONSIGNED,0)  WHEN 2 THEN 1 ELSE 0 END AS IS_CONSIGNED\r\n\r\n                                                           FROM       UYUMSOFT.PSMT_ORDER_M         M \r\n                                                           LEFT JOIN UYUMSOFT.PSMT_ORDER_D         D   ON D.ORDER_M_ID      = M.ORDER_M_ID\r\n                                                           INNER JOIN UYUMSOFT.GNLD_BRANCH          BR  ON BR.BRANCH_ID      = M.BRANCH_ID\r\n                                                           INNER JOIN UYUMSOFT.FIND_ENTITY          FE  ON FE.ENTITY_ID      = M.ENTITY_ID\r\n                                                           LEFT JOIN UYUMSOFT.FIND_SALES_PERSON    S   ON S.SALES_PERSON_ID = M.SALES_PERSON_ID      AND M.CO_ID      = S.CO_ID   \r\n                                                           LEFT JOIN UYUMSOFT.GNLD_DOC_TRA DT ON DT.DOC_TRA_ID = M.DOC_TRA_ID\r\n   \r\n                                                           WHERE     M.PURCHASE_SALES IN (2,3)\r\n                                                                AND  M.REQUEST_STATUS IN (0,4)    \r\n                                                                AND (CASE WHEN NVL(D.LOADING_CARD_ID,0)<>0 THEN NVL(D.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE))\r\n                                                                     ELSE   NVL(M.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE)) END ) <= TO_DATE('{0}','dd/MM/yyyy')\r\n                                                                AND  (CASE WHEN NVL(D.LOADING_CARD_ID,0)<>0 THEN D.LOADING_CARD_ID ELSE 1 END) = case when NVL(D.LOADING_CARD_ID,0)<>0 then  {1} else 1 end\r\n                                                                AND  M.ORDER_STATUS = 1\r\n                                                                AND  BR.BRANCH_CODE          = '{2}'\r\n                                                                AND  {3}\r\n                                                                AND  S.SALES_PERSON_CODE     = '{4}'         \r\n                                                       UNION ALL\r\n                                                    SELECT DISTINCT\r\n                                                    M.ORDER_M_ID                                AS MASTER_NO,\r\n                                                        M.AMT_VAT                                   AS KDV_TUTAR,\r\n                                                        M.AMT                                       AS MAL_TUTAR,\r\n                                                        M.AMT_DISC_TOTAL                            AS ISKONTO_TUTAR,\r\n                                                        M.DOC_DATE                                  AS SIPARIS_TARİH,\r\n                                                        M.DOC_NO                                    AS SIPARIS_NO,\r\n                                                        FE.ENTITY_CODE                              AS CARI_KOD,\r\n                                                        M.DUE_DATE                                  AS VADE_TARIH,\r\n                                                        M.DUE_DAY                                   AS VADE_GUN,\r\n                                                        M.SHIPPING_DATE                             AS YUKLEME_TARIH,\r\n                                                        M.LATITUDE                                  AS LATITUDE,\r\n                                                        M.LONGITUDE                                 AS LONGITUDE,\r\n                                                        ''                                          AS DEPO_KOD,\r\n                                                        CASE M.PURCHASE_SALES WHEN 1 THEN 'Alis' WHEN 2 THEN 'Satis' WHEN 3 THEN 'SatisIade' ELSE 'AlisIade' END  AS ALIS_SATIS,\r\n                                                        M.AMT_RECEIPT,\r\n                                                        CASE NVL(DT.IS_CONSIGNED,0)  WHEN 2 THEN 1 ELSE 0 END AS IS_CONSIGNED\r\n  \r\n                                               FROM       HSMT_LOADING_INSTRUCTION  LI\r\n                                               INNER JOIN HSMT_LOADING_INSTRUCTION_D LID ON LI.LOADING_INSTRUCTION_ID = LID.LOADING_INSTRUCTION_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = LID.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR ON BR.BRANCH_ID = LI.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = LI.SALES_PERSON_ID AND LI.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = LI.CO_ID\r\n                                               INNER JOIN PSMT_ORDER_M M ON M.ORDER_M_ID = LID.ORDER_M_ID\r\n                                               LEFT JOIN UYUMSOFT.PSMT_ORDER_D         OD2   ON OD2.ORDER_M_ID      = M.ORDER_M_ID\r\n                                               LEFT JOIN UYUMSOFT.GNLD_DOC_TRA DT ON DT.DOC_TRA_ID = M.DOC_TRA_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{5}','dd/MM/yyyy') AND TO_DATE('{6}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{7}','dd/MM/yyyy') AND TO_DATE('{8}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID              AND UP.USR_ID = 14\r\n                                            WHERE      BR.BRANCH_CODE      = '{9}'\r\n                                                      AND S.SALES_PERSON_CODE = '{10}'\r\n                                                      AND (CASE WHEN NVL(OD2.LOADING_CARD_ID,0)<>0 THEN NVL(OD2.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE))\r\n                                                            ELSE   NVL(M.SHIPPING_DATE,M.DOC_DATE) END ) <= TO_DATE('{11}','dd/MM/yyyy')\r\n                                                      AND TO_DATE(TO_CHAR(LI.LOADING_INSTRUCTION_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{12}','dd/MM/yyyy'))", (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)currentLoadingCardId, (object)token.BranchCode, (object)inFilter, (object)userParameters.SalesPersonCode, (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)token.BranchCode, (object)userParameters.SalesPersonCode, (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo));
                string inFilter = Helper._GenerateInFilter(entityCodes, "FE.ENTITY_CODE");
                Helper._GenerateNotInFilter(entityCodes, "FE.ENTITY_CODE");
                string str = string.Format("SELECT DISTINCT MASTER_NO, KDV_TUTAR, MAL_TUTAR, ISKONTO_TUTAR, SIPARIS_TARİH, SIPARIS_NO, CARI_KOD, VADE_TARIH, VADE_GUN, YUKLEME_TARIH, LATITUDE, LONGITUDE, DEPO_KOD, ALIS_SATIS, AMT_RECEIPT, IS_CONSIGNED, OPEN_CLOSE_STATUS FROM (SELECT DISTINCT\r\n      M.ORDER_M_ID                                AS MASTER_NO,\r\n                                                        M.AMT_VAT                                   AS KDV_TUTAR,\r\n                                                        M.AMT                                       AS MAL_TUTAR,\r\n                                                        M.AMT_DISC_TOTAL                            AS ISKONTO_TUTAR,\r\n                                                        M.DOC_DATE                                  AS SIPARIS_TARİH,\r\n                                                        M.DOC_NO                                    AS SIPARIS_NO,\r\n                                                        FE.ENTITY_CODE                              AS CARI_KOD,\r\n                                                        M.DUE_DATE                                  AS VADE_TARIH,\r\n                                                        M.DUE_DAY                                   AS VADE_GUN,\r\n                                                        M.SHIPPING_DATE                             AS YUKLEME_TARIH,\r\n                                                        M.LATITUDE                                  AS LATITUDE,\r\n                                                        M.LONGITUDE                                 AS LONGITUDE,\r\n                                                        ''                                          AS DEPO_KOD,\r\n                                                        CASE M.PURCHASE_SALES WHEN 1 THEN 'Alis' WHEN 2 THEN 'Satis' WHEN 3 THEN 'SatisIade' ELSE 'AlisIade' END AS ALIS_SATIS,\r\n                                                        M.AMT_RECEIPT,\r\n                                                        CASE NVL(DT.IS_CONSIGNED,0)  WHEN 2 THEN 1 ELSE 0 END AS IS_CONSIGNED,\r\n  HM.OPEN_CLOSE_STATUS\r\n                                                           FROM       UYUMSOFT.PSMT_ORDER_M         M \r\n                                                           LEFT JOIN UYUMSOFT.PSMT_ORDER_D         D   ON D.ORDER_M_ID      = M.ORDER_M_ID\r\n  INNER JOIN HSMT_LOADING_INSTRUCTION_D HD ON HD.ORDER_M_ID = M.ORDER_M_ID\r\n INNER JOIN HSMT_LOADING_INSTRUCTION HM ON HM.LOADING_INSTRUCTION_ID = HD.LOADING_INSTRUCTION_ID\r\n  INNER JOIN UYUMSOFT.GNLD_BRANCH          BR  ON BR.BRANCH_ID      = M.BRANCH_ID\r\n                                                           INNER JOIN UYUMSOFT.FIND_ENTITY          FE  ON FE.ENTITY_ID      = M.ENTITY_ID\r\n                                                           LEFT JOIN UYUMSOFT.FIND_SALES_PERSON    S   ON S.SALES_PERSON_ID = M.SALES_PERSON_ID      AND M.CO_ID      = S.CO_ID   \r\n                                                           LEFT JOIN UYUMSOFT.GNLD_DOC_TRA DT ON DT.DOC_TRA_ID = M.DOC_TRA_ID\r\n   \r\n                                                           WHERE     M.PURCHASE_SALES IN (2,3)\r\n                                                                AND  M.REQUEST_STATUS IN (0,4)    \r\n                                                                AND (CASE WHEN NVL(D.LOADING_CARD_ID,0)<>0 THEN NVL(D.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE))\r\n                                                                     ELSE   NVL(M.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE)) END ) <= TO_DATE('{0}','dd/MM/yyyy')\r\n                                                                AND  (CASE WHEN NVL(D.LOADING_CARD_ID,0)<>0 THEN D.LOADING_CARD_ID ELSE 1 END) = case when NVL(D.LOADING_CARD_ID,0)<>0 then  {1} else 1 end\r\n                                                                AND  M.ORDER_STATUS = 1\r\n                                                                AND  BR.BRANCH_CODE          = '{2}'\r\n                                                                AND  {3}\r\n                                                                AND  S.SALES_PERSON_CODE     = '{4}'         \r\n                                                       UNION ALL\r\n                                                    SELECT DISTINCT\r\n                                                    M.ORDER_M_ID                                AS MASTER_NO,\r\n                                                        M.AMT_VAT                                   AS KDV_TUTAR,\r\n                                                        M.AMT                                       AS MAL_TUTAR,\r\n                                                        M.AMT_DISC_TOTAL                            AS ISKONTO_TUTAR,\r\n                                                        M.DOC_DATE                                  AS SIPARIS_TARİH,\r\n                                                        M.DOC_NO                                    AS SIPARIS_NO,\r\n                                                        FE.ENTITY_CODE                              AS CARI_KOD,\r\n                                                        M.DUE_DATE                                  AS VADE_TARIH,\r\n                                                        M.DUE_DAY                                   AS VADE_GUN,\r\n                                                        M.SHIPPING_DATE                             AS YUKLEME_TARIH,\r\n                                                        M.LATITUDE                                  AS LATITUDE,\r\n                                                        M.LONGITUDE                                 AS LONGITUDE,\r\n                                                        ''                                          AS DEPO_KOD,\r\n                                                        CASE M.PURCHASE_SALES WHEN 1 THEN 'Alis' WHEN 2 THEN 'Satis' WHEN 3 THEN 'SatisIade' ELSE 'AlisIade' END  AS ALIS_SATIS,\r\n                                                        M.AMT_RECEIPT,\r\n                                                        CASE NVL(DT.IS_CONSIGNED,0)  WHEN 2 THEN 1 ELSE 0 END AS IS_CONSIGNED,\r\n LI.OPEN_CLOSE_STATUS\r\n                                               FROM       HSMT_LOADING_INSTRUCTION  LI\r\n                                               INNER JOIN HSMT_LOADING_INSTRUCTION_D LID ON LI.LOADING_INSTRUCTION_ID = LID.LOADING_INSTRUCTION_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_ENTITY              FE  ON FE.ENTITY_ID                = LID.ENTITY_ID\r\n                                               INNER JOIN UYUMSOFT.GNLD_BRANCH              BR ON BR.BRANCH_ID = LI.BRANCH_ID\r\n                                               INNER JOIN UYUMSOFT.FIND_SALES_PERSON        S   ON S.SALES_PERSON_ID           = LI.SALES_PERSON_ID AND LI.CO_ID      = S.CO_ID \r\n                                               INNER JOIN UYUMSOFT.FIND_CO_ENTITY   FCE    ON FCE.ENTITY_ID         = FE.ENTITY_ID AND FCE.CO_ID = LI.CO_ID\r\n                                               INNER JOIN PSMT_ORDER_M M ON M.ORDER_M_ID = LID.ORDER_M_ID\r\n                                               LEFT JOIN UYUMSOFT.PSMT_ORDER_D         OD2   ON OD2.ORDER_M_ID      = M.ORDER_M_ID\r\n                                               LEFT JOIN UYUMSOFT.GNLD_DOC_TRA DT ON DT.DOC_TRA_ID = M.DOC_TRA_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_CITY        C      ON C.CITY_ID             = FE.CITY_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_TOWN        T      ON T.TOWN_ID             = FE.TOWN_ID\r\n                                               LEFT  JOIN UYUMSOFT.GNLD_COUNTRY     U      ON U.COUNTRY_ID          = FE.COUNTRY_ID\r\n                                               LEFT  JOIN UYUMSOFT.FIND_TAX_OFFICE  V      ON V.TAX_OFFICE_ID       = FE.TAX_OFFICE_ID\r\n                                               LEFT JOIN UYUMSOFT.FIND_ENTITY_PRICE_GRP P ON P.ENTITY_PRICE_GRP_ID = FCE.ENTITY_PRICE_GRP_S_ID\r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC   I ON I.CO_ENTITY_ID        = FCE.CO_ENTITY_ID          AND I.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{5}','dd/MM/yyyy') AND TO_DATE('{6}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC             D ON D.DISC_ID             = I.DISC1_ID               \r\n                                               LEFT JOIN  UYUMSOFT.FIND_CO_ENTITY_DISC  I2 ON I2.CO_ENTITY_ID       = FCE.CO_ENTITY_ID          AND I2.PURCHASE_SALES = 2 AND TO_DATE(TO_CHAR(I2.START_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')<=TO_DATE('{7}','dd/MM/yyyy') AND TO_DATE('{8}','dd/MM/yyyy')<=TO_DATE(TO_CHAR(I2.END_DATE,'dd/MM/yyyy'),'dd/MM/yyyy')\r\n                                               LEFT JOIN  UYUMSOFT.FIND_DISC            D2 ON D2.DISC_ID            = I2.DISC2_ID \r\n                                               LEFT JOIN  UYUMSOFT.HSMD_USERS_PARAMETER UP ON UP.BRANCH_ID          = BR.BRANCH_ID              AND UP.USR_ID = 14\r\n                                            WHERE      BR.BRANCH_CODE      = '{9}'\r\n                                                      AND S.SALES_PERSON_CODE = '{10}'\r\n                                                      AND (CASE WHEN NVL(OD2.LOADING_CARD_ID,0)<>0 THEN NVL(OD2.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE))\r\n                                                            ELSE   NVL(M.SHIPPING_DATE,M.DOC_DATE) END ) <= TO_DATE('{11}','dd/MM/yyyy')\r\n                                                      AND TO_DATE(TO_CHAR(LI.LOADING_INSTRUCTION_DATE,'dd/MM/yyyy'), 'dd/MM/yyyy') = TO_DATE('{12}','dd/MM/yyyy')) t WHERE t.OPEN_CLOSE_STATUS = 2", (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)currentLoadingCardId, (object)token.BranchCode, (object)inFilter, (object)userParameters.SalesPersonCode, (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)token.BranchCode, (object)userParameters.SalesPersonCode, (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo));

                if (str.Length > 32000)
                    EventLog.WriteEntry("Application", str.Substring(0, 32000), EventLogEntryType.Information, 152);
                else
                    EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 152);
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, str);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OrderM orderM = new OrderM()
                    {
                        ERPOrderMId = ConvertToInt32(oracleDataReader[0]),
                        AmtVat = ConvertToDecimal(oracleDataReader[1]),
                        Amt = ConvertToDecimal(oracleDataReader[2]),
                        AmtDisc = ConvertToDecimal(oracleDataReader[3]),
                        DocDate = ConvertToDateTime(oracleDataReader[4]),
                        DocNo = ConvertToString(oracleDataReader[5]),
                        EntityCode = ConvertToString(oracleDataReader[6]),
                        DueDate = ConvertToDateTime(oracleDataReader[7]),
                        DueDay = ConvertToInt32(oracleDataReader[8]),
                        ShippingDate = ConvertToDateTime(oracleDataReader[9]),
                        Latitude = ConvertToDecimal(oracleDataReader[10]),
                        Longitude = ConvertToDecimal(oracleDataReader[11]),
                        WhouseCode = ConvertToString(oracleDataReader[12]),
                        PurchaseSales = ConvertToString(oracleDataReader[13]),
                        BranchCode = token.BranchCode,
                        AmtOrder = ConvertToDecimal(oracleDataReader[14]),
                        IsConsigned = ConvertToBoolean(oracleDataReader[15])
                    };
                    orderMList.Add(orderM);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return orderMList.ToArray();
        }

        private static int GetCurrentLoadingCardId(HotSaleServiceTables.Token token, IDbConnection conn, string loadingCardCode)
        {
            int num = 0;
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            string cmdText = string.Empty;
            try
            {
                oracleCommand = conn.CreateCommand();
                cmdText = string.Format("select t.loading_card_id from hsmd_loading_card t inner join gnld_branch b on b.branch_id = t.branch_id\r\n                        where t.loading_card_code = '{0}' and b.branch_code = '{1}'", (object)loadingCardCode, (object)token.BranchCode);
                oracleCommand.CommandText = cmdText;
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    num = ConvertToInt32(oracleDataReader[0]);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", "Ex : " + ex.Message + " SQL : " + cmdText, EventLogEntryType.Error, 2001);
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return num;
        }

        internal static OrderD[] GetOrderD(HotSaleServiceTables.Token token, IDbConnection conn, int[] orderMasterNos, string siparisSevkTarihi, string loadingCardCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OrderD> orderDList = new List<OrderD>();
            string str = string.Empty;
            CultureInfo cultureInfo = new CultureInfo("TR-tr");
            try
            {
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                DateTime dateTime = DateTime.Today;
                if (!string.IsNullOrEmpty(siparisSevkTarihi))
                    dateTime = ConvertToDateTime(dateTime);
                int currentLoadingCardId = Helper.GetCurrentLoadingCardId(token, conn, loadingCardCode);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (orderMasterNos.Length == 0)
                    return orderDList.ToArray();

                //string inFilter = Helper._GenerateInFilter(orderMasterNos, "D.ORDER_M_ID");
                //str string.Format(string.Format("SELECT DISTINCT\r\n                                                        D.ORDER_M_ID                                AS MASTER_NO,\r\n                                                        D.ORDER_D_ID                                AS DETAY_NO,\r\n                                                        D.QTY-D.QTY_SHIPPING                        AS KALAN_MIKTAR,\r\n                                                        D.UNIT_PRICE                                AS BIRIM_FIYAT,\r\n                                                        D.DISC1_RATE                                AS ISK1_YUZDE,\r\n                                                        D.DISC2_RATE                                AS ISK2_YUZDE,\r\n                                                        W.WHOUSE_CODE                               AS DEPO_KOD,\r\n                                                        I.ITEM_CODE                                 AS STOK_KODU,\r\n                                                        U.UNIT_CODE,\r\n                                                        ROUND((D.QTY - D.QTY_SHIPPING) * (D.QTY_PRM/D.QTY),8) AS KALAN_MIKTAR_PRM,\r\n                                                        NVL(LIX.LOADING_QTY,0) AS LOADING_QTY,\r\n                                                        NVL(LIX.LOADING_QTY_PRM,0) AS LOADING_QTY_PRM,\r\n                                                        NVL(D.SOURCE_D2_ID,0) as SOURCE_D2_ID,\r\n                                                        R.REASON_CODE                               AS NEDEN_KOD\r\n\r\n                                                           FROM       UYUMSOFT.PSMT_ORDER_D         D \r\n                                                           INNER JOIN UYUMSOFT.PSMT_ORDER_M         M   ON M.ORDER_M_ID      = D.ORDER_M_ID  \r\n                                                           INNER JOIN UYUMSOFT.GNLD_BRANCH          BR  ON BR.BRANCH_ID      = D.BRANCH_ID\r\n                                                           INNER JOIN UYUMSOFT.INVD_ITEM            I   ON I.ITEM_ID         = D.ITEM_ID\r\n                                                           INNER JOIN UYUMSOFT.INVD_WHOUSE          W   ON W.WHOUSE_ID       = D.Whouse_Id\r\n                                                           INNER JOIN UYUMSOFT.INVD_UNIT            U   ON U.UNIT_ID         = D.UNIT_ID\r\n                                                           LEFT JOIN UYUMSOFT.GNLD_REASON           R   ON R.REASON_ID       = D.REASON_ID\r\n                                                           LEFT OUTER JOIN (SELECT LI.ORDER_M_ID,LI.ORDER_D_ID,SUM(LI.QTY) AS LOADING_QTY, SUM(LI.QTY_PRM) AS LOADING_QTY_PRM\r\n                                                                              FROM UYUMSOFT.HSMT_LOADING_INSTRUCTION_D LI \r\n                                                                             GROUP BY LI.ORDER_M_ID,LI.ORDER_D_ID) LIX ON LIX.ORDER_M_ID = M.ORDER_M_ID AND LIX.ORDER_D_ID = D.ORDER_D_ID\r\n \r\n                                                           WHERE     D.ORDER_STATUS = 1\r\n                                                                \r\n                                                                AND (CASE WHEN NVL(D.LOADING_CARD_ID,0)<>0 THEN NVL(D.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE))\r\n                                                                        ELSE   NVL(M.SHIPPING_DATE, NVL(M.SHIPPING_DATE, M.DOC_DATE)) END) <= TO_DATE('{0}', 'dd/MM/yyyy')\r\n                                                                AND (CASE WHEN NVL(D.LOADING_CARD_ID, 0) <> 0 THEN D.LOADING_CARD_ID ELSE 1 END) = case when NVL(D.LOADING_CARD_ID, 0)<> 0 then  {1} else 1 end\r\n                                                                AND D.QTY-D.QTY_SHIPPING > 0        \r\n                                                                AND  BR.BRANCH_CODE           = '{2}'\r\n                                                                AND {3}\r\n                                                            ORDER BY D.ORDER_D_ID\r\n                                                             ", (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)currentLoadingCardId, (object)token.BranchCode, (object)inFilter));

                string inFilter = Helper._GenerateInFilter(orderMasterNos, "D.ORDER_M_ID");
                str = string.Format(string.Format("SELECT DISTINCT\r\n                                                        D.ORDER_M_ID                                AS MASTER_NO,\r\n                                                        D.ORDER_D_ID                                AS DETAY_NO,\r\n                                                        D.QTY-D.QTY_SHIPPING                        AS KALAN_MIKTAR,\r\n                                                        D.UNIT_PRICE                                AS BIRIM_FIYAT,\r\n                                                        D.DISC1_RATE                                AS ISK1_YUZDE,\r\n                                                        D.DISC2_RATE                                AS ISK2_YUZDE,\r\n                                                        W.WHOUSE_CODE                               AS DEPO_KOD,\r\n                                                        I.ITEM_CODE                                 AS STOK_KODU,\r\n                                                        U.UNIT_CODE,\r\n                                                        ROUND((D.QTY - D.QTY_SHIPPING) * (D.QTY_PRM/D.QTY),8) AS KALAN_MIKTAR_PRM,\r\n                                                        NVL(LIX.LOADING_QTY,0) AS LOADING_QTY,\r\n                                                        NVL(LIX.LOADING_QTY_PRM,0) AS LOADING_QTY_PRM,\r\n                                                        NVL(D.SOURCE_D2_ID,0) as SOURCE_D2_ID,\r\n                                                        R.REASON_CODE                               AS NEDEN_KOD\r\n\r\n                                                           FROM       UYUMSOFT.PSMT_ORDER_D         D \r\n                                                           INNER JOIN UYUMSOFT.PSMT_ORDER_M         M   ON M.ORDER_M_ID      = D.ORDER_M_ID  \r\n                                                           INNER JOIN UYUMSOFT.GNLD_BRANCH          BR  ON BR.BRANCH_ID      = D.BRANCH_ID\r\n                                                           INNER JOIN UYUMSOFT.INVD_ITEM            I   ON I.ITEM_ID         = D.ITEM_ID\r\n                                                           INNER JOIN UYUMSOFT.INVD_WHOUSE          W   ON W.WHOUSE_ID       = D.Whouse_Id\r\n                                                           INNER JOIN UYUMSOFT.INVD_UNIT            U   ON U.UNIT_ID         = D.UNIT_ID\r\n                                                           LEFT JOIN UYUMSOFT.GNLD_REASON           R   ON R.REASON_ID       = D.REASON_ID\r\n                                                           LEFT OUTER JOIN (SELECT LI.ORDER_M_ID,LI.ORDER_D_ID,SUM(LI.QTY) AS LOADING_QTY, SUM(LI.QTY_PRM) AS LOADING_QTY_PRM\r\n                                                                              FROM UYUMSOFT.HSMT_LOADING_INSTRUCTION_D LI \r\n                                                                             GROUP BY LI.ORDER_M_ID,LI.ORDER_D_ID) LIX ON LIX.ORDER_M_ID = M.ORDER_M_ID AND LIX.ORDER_D_ID = D.ORDER_D_ID\r\n \r\n                                                           WHERE     D.ORDER_STATUS = 1\r\n                                                                \r\n                                                                AND (CASE WHEN NVL(D.LOADING_CARD_ID,0)<>0 THEN NVL(D.SHIPPING_DATE,NVL(M.SHIPPING_DATE,M.DOC_DATE))\r\n                                                                        ELSE   NVL(M.SHIPPING_DATE, NVL(M.SHIPPING_DATE, M.DOC_DATE)) END) <= TO_DATE('{0}', 'dd/MM/yyyy')\r\n                                                                AND (CASE WHEN NVL(D.LOADING_CARD_ID, 0) <> 0 THEN D.LOADING_CARD_ID ELSE 1 END) = case when NVL(D.LOADING_CARD_ID, 0)<> 0 then  {1} else 1 end\r\n                                                                AND D.QTY-D.QTY_SHIPPING > 0        \r\n                                                                AND  BR.BRANCH_CODE           = '{2}'\r\n                                                                AND {3}\r\n                                                            ORDER BY D.ORDER_D_ID\r\n                                                             ", (object)dateTime.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)currentLoadingCardId, (object)token.BranchCode, (object)inFilter));
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, str);
                oracleDataReader = oracleCommand.ExecuteReader();
                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 2201);
                while (oracleDataReader.Read())
                {
                    if (!userParameters.IsGetQtyFromLoadingIns || !(ConvertToDecimal(oracleDataReader[10]) == Decimal.Zero))
                        orderDList.Add(new OrderD()
                        {
                            ERPOrderMId = ConvertToInt32(oracleDataReader[0]),
                            ERPOrderDId = ConvertToInt32(oracleDataReader[1]),

                            Qty = userParameters.IsGetQtyFromLoadingIns ? ConvertToDecimal(oracleDataReader[10]) : ConvertToDecimal(oracleDataReader[2]),
                            Price = ConvertToDecimal(oracleDataReader[3]),
                            DiscRate1 = ConvertToDecimal(oracleDataReader[4]),
                            DiscRate2 = ConvertToDecimal(oracleDataReader[5]),
                            WhouseCode = ConvertToString(oracleDataReader[6]),
                            ItemCode = ConvertToString(oracleDataReader[7]),
                            UnitCode = ConvertToString(oracleDataReader[8]),
                            BranchCode = token.BranchCode,
                            QtyPrm = userParameters.IsGetQtyFromLoadingIns ? ConvertToDecimal(oracleDataReader[11]) : ConvertToDecimal(oracleDataReader[9]),
                            CampaignId = ConvertToInt32(oracleDataReader[12]),
                            ReasonCode = ConvertToString(oracleDataReader[13]),
                        });
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", "Ex : " + ex.Message + " SQL : " + str, EventLogEntryType.Error, 2001);
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return orderDList.ToArray();
        }

        internal static OnlineItem[] GetOnlineItems(HotSaleServiceTables.Token token, IDbConnection conn, string whouseCode, string[] itemCodes)
        {
            bool sansurle = false;
            try
            {
                var WhouseList = Helper.GetOtherDepos(token, conn);
                if (WhouseList != null)
                {
                    sansurle = WhouseList.Count(e => e.WhouseCode == whouseCode && e.WhouseDesc.Contains("*")) > 0;
                }
            }
            catch (Exception e)
            {

            }

            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OnlineItem> onlineItemList = new List<OnlineItem>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                string inFilter = Helper._GenerateInFilter(itemCodes, "I.ITEM_CODE");
                oracleCommand = conn.CreateCommand();
                string tmp = string.Format(@"SELECT
	                    W.WHOUSE_CODE AS WHOUSE_CODE,
	                    I.ITEM_CODE AS ITEM_NAME,
	                    WI.QTY_PRM AS QTY_PRM,
	                    WI.QTY_PO AS PURCHASE_ORDER_QTY,
	                    WI.QTY_SO AS SALES_ORDER_QTY,
	                    WI.QTY_PRM - WI.QTY_SO + WI.QTY_PO AS USE_QTY,
	                    NVL(
		                    (
		                    SELECT
			                    SUM( R.QTY_REMAIN_PRM ) AS QTY_REMAIN_PRM 
		                    FROM
			                    INVT_RESERVE_M R
			                    INNER JOIN GNLD_BRANCH BR2 ON BR2.BRANCH_ID = R.BRANCH_ID
			                    INNER JOIN INVD_WHOUSE W2 ON W2.WHOUSE_ID = R.WHOUSE_ID
			                    INNER JOIN INVD_ITEM I2 ON I2.ITEM_ID = R.ITEM_ID 
		                    WHERE
                                R.OPEN_CLOSE != 2 AND
			                    BR2.BRANCH_CODE = BR.BRANCH_CODE 
			                    AND W2.WHOUSE_CODE = W.WHOUSE_CODE 
			                    AND I2.ITEM_CODE = I.ITEM_CODE 
		                    GROUP BY
			                    BR.BRANCH_CODE,
			                    W.WHOUSE_CODE,
			                    I.ITEM_CODE 
		                    ),
		                    0 
	                    ) AS QTY_REMAIN_PRM,
	                    WI.QTY_PRM - NVL(
		                    (
		                    SELECT
			                    SUM( R.QTY_REMAIN_PRM ) AS QTY_REMAIN_PRM 
		                    FROM
			                    INVT_RESERVE_M R
			                    INNER JOIN GNLD_BRANCH BR2 ON BR2.BRANCH_ID = R.BRANCH_ID
			                    INNER JOIN INVD_WHOUSE W2 ON W2.WHOUSE_ID = R.WHOUSE_ID
			                    INNER JOIN INVD_ITEM I2 ON I2.ITEM_ID = R.ITEM_ID 
		                    WHERE
			                    BR2.BRANCH_CODE = BR.BRANCH_CODE 
			                    AND W2.WHOUSE_CODE = W.WHOUSE_CODE 
			                    AND I2.ITEM_CODE = I.ITEM_CODE 
		                    GROUP BY
			                    BR.BRANCH_CODE,
			                    W.WHOUSE_CODE,
			                    I.ITEM_CODE 
		                    ),
		                    0 
	                    ) AS QTY_FREE_PRM 
                    FROM
	                    UYUMSOFT.INVD_BWH_ITEM WI
	                    INNER JOIN UYUMSOFT.INVD_ITEM I ON I.ITEM_ID = WI.ITEM_ID
	                    INNER JOIN UYUMSOFT.GNLD_BRANCH BR ON BR.BRANCH_ID = WI.BRANCH_ID
	                    INNER JOIN UYUMSOFT.INVD_WHOUSE W ON W.WHOUSE_ID = WI.WHOUSE_ID 
                    WHERE
	                    BR.BRANCH_CODE = '{0}' 
	                    AND W.WHOUSE_CODE = '{1}' 
	                    AND {2}", (object)token.BranchCode, (object)whouseCode, (object)inFilter);
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, tmp);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    try
                    {
                        onlineItemList.Add(new OnlineItem()
                        {
                            WhouseCode = ConvertToString(oracleDataReader[0] == System.DBNull.Value ? "" : oracleDataReader[0]),
                            ItemCode = ConvertToString(oracleDataReader[1] == System.DBNull.Value ? "" : oracleDataReader[1]),
                            QtyPrime = sansurle ? 999999 : ConvertToDecimal(oracleDataReader[2] == System.DBNull.Value ? "0" : oracleDataReader[2]),
                            PurchaseOrderQty = sansurle ? 999999 : ConvertToDecimal(oracleDataReader[3] == System.DBNull.Value ? "0" : oracleDataReader[3]),
                            SalesOrderQty = sansurle ? 999999 : ConvertToDecimal(oracleDataReader[4] == System.DBNull.Value ? "0" : oracleDataReader[4]),
                            UseQty = sansurle ? 999999 : ConvertToDecimal(oracleDataReader[5] == System.DBNull.Value ? "0" : oracleDataReader[5]),
                            ReservationQty = sansurle ? 999999 : ConvertToDecimal(oracleDataReader[6] == System.DBNull.Value ? "0" : oracleDataReader[6]),
                            FreeQty = sansurle ? 999999 : ConvertToDecimal(oracleDataReader[7] == System.DBNull.Value ? "0" : oracleDataReader[7]),
                            BranchCode = token.BranchCode
                        });
                    }
                    catch (Exception ex)
                    {
                        String err = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", "Online Item : " + ex.Message, EventLogEntryType.Error, 2001);
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return onlineItemList.ToArray();
        }

        internal static OnlineEntityBalance GetOnlineEntityBalances(HotSaleServiceTables.Token token, IDbConnection conn, string entityCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            OnlineEntityBalance onlineEntityBalance = new OnlineEntityBalance();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format(" SELECT FE.ENTITY_CODE AS ENTITY_CODE, SUM(FD.PLUS_MINUS * FD.AMT) AS BAKIYE FROM UYUMSOFT.FINT_FIN_D FD INNER JOIN UYUMSOFT.FIND_ENTITY FE ON FE.ENTITY_ID = FD.ENTITY_ID INNER JOIN UYUMSOFT.GNLD_BRANCH BR ON BR.BRANCH_ID = FD.BRANCH_ID INNER JOIN FINP_FIN_PARAMETER FP ON FP.CO_ID = FD.CO_ID WHERE BR.BRANCH_CODE = '{0}' AND FE.ENTITY_CODE = '{1}' AND TO_DATE(TO_CHAR(FP.ENTITY_PERIOD_START_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy') <= TO_DATE(TO_CHAR(FD.DOC_DATE, 'dd/MM/yyyy'), 'dd/MM/yyyy') GROUP BY BR.BRANCH_CODE, FE.ENTITY_CODE", (object)token.BranchCode, (object)entityCode);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    onlineEntityBalance.EntityCode = ConvertToString(oracleDataReader[0]);
                    onlineEntityBalance.Balance = ConvertToDecimal(oracleDataReader[1]);
                    onlineEntityBalance.BranchCode = token.BranchCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return onlineEntityBalance;
        }

        internal static OnlineEntityExtract[] GetOnlineEntityExtracts(HotSaleServiceTables.Token token, IDbConnection conn, string entityCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OnlineEntityExtract> onlineEntityExtractList = new List<OnlineEntityExtract>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT\r\n                                                    FE.ENTITY_CODE            AS ENTITY_CODE,\r\n                                                    FD.DOC_DATE               AS DOC_DATE,\r\n                                                    FD.DOC_NO                 AS DOC_NO,\r\n                                                    CASE WHEN FD.PLUS_MINUS=1 THEN FD.AMT ELSE 0 END AS AMT_DEBIT,\r\n                                                    CASE WHEN FD.PLUS_MINUS=-1 THEN FD.AMT ELSE 0 END AS AMT_CREDIT\r\n\r\n\r\n                                                     FROM       UYUMSOFT.FINT_FIN_D      FD\r\n                                                     INNER JOIN UYUMSOFT.FIND_ENTITY     FE ON FE.ENTITY_ID   = FD.ENTITY_ID\r\n                                                     INNER JOIN UYUMSOFT.GNLD_BRANCH     BR ON BR.BRANCH_ID   = FD.BRANCH_ID\r\n\r\n                                                     WHERE BR.BRANCH_CODE = '{0}'  AND\r\n                                                           FE.ENTITY_CODE = '{1}'  \r\nAND FD.DOC_DATE >= (SELECT P.ENTITY_PERIOD_START_DATE FROM FINP_FIN_PARAMETER P WHERE P.CO_ID = FD.CO_ID)\r\n                                                 ORDER BY FD.DOC_DATE\r\n                                                        ", (object)token.BranchCode, (object)entityCode);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OnlineEntityExtract onlineEntityExtract = new OnlineEntityExtract()
                    {
                        EntityCode = ConvertToString(oracleDataReader[0]),
                        DocDate = ConvertToDateTime(oracleDataReader[1]),
                        DocNo = ConvertToString(oracleDataReader[2]),
                        AmtDebit = ConvertToDecimal(oracleDataReader[3]),
                        AmtCredit = ConvertToDecimal(oracleDataReader[4]),
                        BranchCode = token.BranchCode
                    };
                    onlineEntityExtractList.Add(onlineEntityExtract);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return onlineEntityExtractList.ToArray();
        }

        internal static OnlineOrderD[] GetOnlineOrderDs(HotSaleServiceTables.Token token, IDbConnection conn, string entityCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OnlineOrderD> onlineOrderDList = new List<OnlineOrderD>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT\r\n                                                        D.DOC_DATE               AS DOC_DATE,\r\n                                                        M.DOC_NO                 AS DOC_NO,\r\n                                                        I.ITEM_CODE              AS ITEM_CODE,\r\n                                                        I.ITEM_NAME              AS ITEM_NAME,\r\n                                                        U.UNIT_CODE              AS UNIT_CODE,\r\n                                                        CASE WHEN D.QTY>0 THEN (D.QTY_PRM-(D.QTY_PRM/D.QTY)*D.QTY_SHIPPING) ELSE 0 END AS QTY\r\n\r\n\r\n\r\n                                                         FROM       UYUMSOFT.PSMT_ORDER_D    D\r\n                                                         INNER JOIN UYUMSOFT.PSMT_ORDER_M    M  ON M.ORDER_M_ID   = D.ORDER_M_ID\r\n                                                         INNER JOIN UYUMSOFT.FIND_ENTITY     FE ON FE.ENTITY_ID   = M.ENTITY_ID\r\n                                                         INNER JOIN UYUMSOFT.INVD_ITEM       I  ON I.ITEM_ID      = D.ITEM_ID\r\n                                                         INNER JOIN UYUMSOFT.INVD_UNIT       U  ON I.UNIT_ID      = U.UNIT_ID\r\n                                                         INNER JOIN UYUMSOFT.GNLD_BRANCH     BR ON BR.BRANCH_ID   = D.BRANCH_ID\r\n\r\n                                                         WHERE BR.BRANCH_CODE = '{0}'  AND\r\n                                                               FE.ENTITY_CODE = '{1}'  \r\n  \r\n                                                        ", (object)token.BranchCode, (object)entityCode);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    OnlineOrderD onlineOrderD = new OnlineOrderD()
                    {
                        DocDate = ConvertToDateTime(oracleDataReader[0]),
                        DocNo = ConvertToString(oracleDataReader[1]),
                        ItemCode = ConvertToString(oracleDataReader[2]),
                        ItemName = ConvertToString(oracleDataReader[3]),
                        UnitCode = ConvertToString(oracleDataReader[4]),
                        Qty = ConvertToDecimal(oracleDataReader[5]),
                        BranchCode = token.BranchCode
                    };
                    onlineOrderDList.Add(onlineOrderD);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return onlineOrderDList.ToArray();
        }

        internal static OrderM[] GetOnlineOrders(HotSaleServiceTables.Token token, IDbConnection conn, string entityCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<OrderM> orderMList = new List<OrderM>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                string tmp = string.Format("SELECT M.ORDER_M_ID,\r\n                                                    M.DOC_DATE AS DOC_DATE,\r\n                                                    M.DOC_NO AS DOC_NO,\r\n                                                    SUM(NVL(INV.AMT_WITH_DISC, 0) + NVL(INV.AMT_VAT, 0)) AS AMT_RECEIPT,\r\n                                                    CASE DR.PURCHASE_SALES\r\n                                                        WHEN 1 THEN\r\n                                                        'Satis'\r\n                                                        WHEN 2 THEN\r\n                                                        'Alis'\r\n                                                        WHEN 3 THEN\r\n                                                        'AlisIade'\r\n                                                        WHEN 4 THEN\r\n                                                        'SatisIade'\r\n                                                    END AS PURCHASE_SALES\r\n                                                    FROM PSMT_ORDER_D D\r\n                                                INNER JOIN PSMT_ORDER_M M             ON M.ORDER_M_ID = D.ORDER_M_ID\r\n                                                INNER JOIN UYUMSOFT.FIND_ENTITY FE    ON FE.ENTITY_ID = M.ENTITY_ID\r\n                                                INNER JOIN UYUMSOFT.GNLD_BRANCH BR    ON BR.BRANCH_ID = M.BRANCH_ID\r\n                                                INNER JOIN UYUMSOFT.GNLD_DOC_TRA DR   ON DR.DOC_TRA_ID = M.DOC_TRA_ID\r\n                                                LEFT JOIN PSMT_INVOICE_D INV ON INV.SOURCE_APP = 122 AND INV.SOURCE_M_ID = D.ORDER_M_ID AND INV.SOURCE_D_ID = D.ORDER_D_ID\r\n                                                WHERE BR.BRANCH_CODE = '{0}'\r\n                                                AND FE.ENTITY_CODE = '{1}'\r\n                                                AND M.DOC_DATE >= TRUNC(CURRENT_DATE - 90)\r\n                                                GROUP BY M.ORDER_M_ID, M.DOC_DATE, M.DOC_NO, DR.PURCHASE_SALES\r\n                                                ORDER BY M.DOC_DATE DESC\r\n                                                        ", (object)token.BranchCode, (object)entityCode);
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, tmp);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    orderMList.Add(new OrderM()
                    {
                        ERPOrderMId = ConvertToInt32(oracleDataReader[0]),
                        DocDate = ConvertToDateTime(oracleDataReader[1]),
                        DocNo = oracleDataReader[2].ToString(),
                        AmtOrder = ConvertToDecimal(oracleDataReader[3]),
                        PurchaseSales = ConvertToString(oracleDataReader[4]), // 15.03.2022
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return orderMList.ToArray();
        }

        internal static OrderM GetOnlineOrderWithDetails(HotSaleServiceTables.Token token, IDbConnection conn, int orderMId)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            OrderM orderM = new OrderM();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                string tmp = string.Format("SELECT M.ORDER_M_ID,\r\n                                                    M.DOC_DATE AS DOC_DATE,\r\n                                                    M.DOC_NO AS DOC_NO,\r\n                                                    SUM(NVL(INV.AMT_WITH_DISC,0) + NVL(INV.AMT_VAT,0)) AS AMT_RECEIPT,\r\n                                                    SUM(NVL(INV.AMT_DISC0,0) + NVL(INV.AMT_DISC1,0) + NVL(INV.AMT_DISC2,0) + NVL(INV.AMT_DISC3,0) + NVL(INV.AMT_DISC4,0) + NVL(INV.AMT_DISC5,0) + NVL(INV.AMT_DISC6,0)) AS AMT_DISC_TOTAL,\r\n                                                    SUM(NVL(INV.AMT_VAT,0)) AS AMT_VAT,\r\n                                                    SUM(NVL(INV.AMT,0)) AS AMT,\r\n                                                    M.DISC0_RATE,\r\n                                                    BR.BRANCH_CODE,\r\n                                                    FE.ENTITY_CODE\r\n                                                    FROM PSMT_ORDER_D D\r\n                                                INNER JOIN PSMT_ORDER_M M             ON M.ORDER_M_ID = D.ORDER_M_ID\r\n                                                INNER JOIN UYUMSOFT.FIND_ENTITY FE    ON FE.ENTITY_ID = M.ENTITY_ID\r\n                                                INNER JOIN UYUMSOFT.GNLD_BRANCH BR    ON BR.BRANCH_ID = M.BRANCH_ID\r\n                                                INNER JOIN UYUMSOFT.GNLD_DOC_TRA DR   ON DR.DOC_TRA_ID = M.DOC_TRA_ID\r\n                                                LEFT JOIN PSMT_INVOICE_D INV ON INV.SOURCE_APP = 122 AND INV.SOURCE_M_ID = D.ORDER_M_ID AND INV.SOURCE_D_ID = D.ORDER_D_ID\r\n                                                WHERE M.ORDER_M_ID = {0}\r\n                                                GROUP BY M.ORDER_M_ID,M.DOC_DATE,M.DOC_NO,DR.PURCHASE_SALES,M.DISC0_RATE,BR.BRANCH_CODE,FE.ENTITY_CODE\r\n                                                ORDER BY M.DOC_DATE DESC", (object)orderMId.ToString());
                oracleCommand.CommandText = GetIsNullCommandReplace(conn, tmp);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    orderM.ERPOrderMId = ConvertToInt32(oracleDataReader[0]);
                    orderM.DocDate = ConvertToDateTime(oracleDataReader[1]);
                    orderM.DocNo = oracleDataReader[2].ToString();
                    orderM.AmtOrder = ConvertToDecimal(oracleDataReader[3]);
                    orderM.AmtDisc = ConvertToDecimal(oracleDataReader[4]);
                    orderM.AmtVat = ConvertToDecimal(oracleDataReader[5]);
                    orderM.Amt = ConvertToDecimal(oracleDataReader[6]);
                    orderM.MasterDiscRate = ConvertToDecimal(oracleDataReader[7]);
                    orderM.BranchCode = oracleDataReader[8].ToString();
                    orderM.EntityCode = oracleDataReader[9].ToString();
                }
                oracleCommand.CommandText = string.Format("SELECT D.ORDER_M_ID,\r\n                                                                D.ORDER_D_ID,\r\n                                                                ITM.ITEM_CODE,\r\n                                                                D.UNIT_PRICE,\r\n                                                                D.QTY_SHIPPING,\r\n                                                                D.QTY_SHIPPING,\r\n                                                                U.UNIT_CODE,\r\n                                                                D.VAT_RATE,\r\n                                                                W.WHOUSE_CODE,\r\n                                                                D.Disc1_Rate,\r\n                                                                D.DISC2_RATE,\r\n                                                                D.DISC3_RATE\r\n                                                                FROM UYUMSOFT.PSMT_ORDER_D D\r\n                                                                INNER JOIN INVD_ITEM ITM ON ITM.ITEM_ID = D.ITEM_ID\r\n                                                                INNER JOIN INVD_WHOUSE W ON W.WHOUSE_ID = D.WHOUSE_ID\r\n                                                                INNER JOIN INVD_UNIT U ON U.UNIT_ID = D.UNIT_ID\r\n                                                                WHERE D.ORDER_M_ID = {0}", (object)orderM.ERPOrderMId);
                oracleDataReader = oracleCommand.ExecuteReader();
                List<OrderD> orderDList = new List<OrderD>();
                while (oracleDataReader.Read())
                    orderDList.Add(new OrderD()
                    {
                        BranchCode = orderM.BranchCode,
                        ERPOrderMId = ConvertToInt32(oracleDataReader[0]),
                        ERPOrderDId = ConvertToInt32(oracleDataReader[1]),
                        ItemCode = oracleDataReader[2].ToString(),
                        Price = ConvertToDecimal(oracleDataReader[3]),
                        Qty = ConvertToDecimal(oracleDataReader[4]),
                        QtyPrm = ConvertToDecimal(oracleDataReader[5]),
                        UnitCode = oracleDataReader[6].ToString(),
                        VatRate = ConvertToDecimal(oracleDataReader[7]),
                        WhouseCode = oracleDataReader[8].ToString(),
                        //
                        DiscRate1 = ConvertToDecimal(oracleDataReader[9]),
                        DiscRate2 = ConvertToDecimal(oracleDataReader[10]),
                        DiscRate3 = ConvertToDecimal(oracleDataReader[11])
                    });
                orderM.OrderDList = orderDList.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return orderM;
        }

        internal static HotSaleServiceTables.Category[] GetCategories(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<HotSaleServiceTables.Category> categoryList = new List<HotSaleServiceTables.Category>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT \r\n                                                        A.CATEGORY_ID,\r\n                                                        A.DESCRIPTION     AS  CATEGORY_DESC,\r\n                                                        A.CATEGORY_LEVEL,\r\n                                                        A.PARENT_CATEGORY_ID\r\n                                                        \r\n\r\n                                                        FROM {0} A\r\n                                                        ", (object)"UYUMSOFT.CRMD_CASE_CATEGORY");
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    HotSaleServiceTables.Category category = new HotSaleServiceTables.Category()
                    {
                        CategoryId = ConvertToInt32(oracleDataReader[0]),
                        CategoryDesc = ConvertToString(oracleDataReader[1]),
                        CategoryLevel = ConvertToInt32(oracleDataReader[2]),
                        ParentCategoryId = ConvertToInt32(oracleDataReader[3])
                    };
                    categoryList.Add(category);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return categoryList.ToArray();
        }

        internal static ItemUnit[] GetItemUnits(HotSaleServiceTables.Token token, string[] itemCodes, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<ItemUnit> itemUnitList = new List<ItemUnit>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT I.ITEM_CODE,\r\n                                                        U.UNIT_CODE,\r\n                                                        U2.UNIT_CODE AS UNIT_CODE2,\r\n                                                        IU.RATE,\r\n                                                        IU.RATE2\r\n                                                          FROM INVD_ITEM_UNIT IU\r\n                                                         INNER JOIN INVD_ITEM I\r\n                                                            ON I.ITEM_ID = IU.ITEM_ID\r\n                                                         INNER JOIN INVD_UNIT U\r\n                                                            ON U.UNIT_ID = IU.UNIT_ID\r\n                                                         INNER JOIN INVD_UNIT U2\r\n                                                            ON U2.UNIT_ID = IU.UNIT2_ID\r\n                                                            WHERE {0}\r\n                                                        ", (object)Helper._GenerateInFilter(itemCodes, "I.ITEM_CODE"));
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    ItemUnit itemUnit = new ItemUnit()
                    {
                        ItemCode = ConvertToString(oracleDataReader[0]),
                        UnitCode = ConvertToString(oracleDataReader[1]),
                        UnitCode2 = ConvertToString(oracleDataReader[2]),
                        Rate = ConvertToDecimal(oracleDataReader[3]),
                        Rate2 = ConvertToDecimal(oracleDataReader[4])
                    };
                    itemUnitList.Add(itemUnit);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return itemUnitList.ToArray();
        }

        private static String getValue(int secilen)
        {
            String name = "";

            if (secilen == 1000)
            {
                name = "Irsaliye";
            }

            if (secilen == 2)
            {
                name = "Fatura";
            }

            if (secilen == 220)
            {
                name = "SatisSiparis";
            }

            if (secilen == 122)
            {
                name = "SatisSiparis";
            }

            return name;
        }

        internal static IadeNedeni[] GetIadeNedeniDefs(HotSaleServiceTables.Token token, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            List<IadeNedeni> iadeNedeniList = new List<IadeNedeni>();
            try
            {
                if (Helper.GetUserParameters(token, conn) == null)
                    throw new Exception("Invalid token");
                oracleCommand = conn.CreateCommand();
                // old String sql = "select t.reason_code,t.description, t.source_app, t.reason_id from gnld_reason t where t.source_app in (1000, 2, 220, 122, 102)";
                String sql = "select t.reason_code,t.description, CASE WHEN t.source_app = 1000 THEN 'Irsaliye'  WHEN t.source_app = 122 THEN 'SatisSiparis'  WHEN t.source_app = 2 THEN 'Fatura' END AS SourceApp from gnld_reason t where t.source_app IN( 1000,122,2)";
                oracleCommand.CommandText = sql;
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                {
                    IadeNedeni iadeNedeni = new IadeNedeni()
                    {
                        NedenKod = ConvertToString(oracleDataReader[0]),
                        NedenAd = ConvertToString(oracleDataReader[1]),
                        SourceApp = ConvertToString(oracleDataReader[2])//getValue(Int32.Parse(ConvertToString(oracleDataReader[2])))//"Irsaliye",
                    };
                    iadeNedeniList.Add(iadeNedeni);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return iadeNedeniList.ToArray();
        }

        internal static string SavePrintDesign(HotSaleServiceTables.Token token, IDbConnection conn, PrintDesign[] designList)
        {
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                oracleTransaction = conn.BeginTransaction();

                dbCommand = conn.CreateCommand();
                dbCommand.CommandText = "DELETE FROM UYUMSOFT.HSMT_PRINT_DESIGN WHERE BRANCH_ID = " + (object)coBranchId1;
                dbCommand.Transaction = oracleTransaction;
                //using (dbCommand = (IDbCommand)new OracleCommand("DELETE FROM UYUMSOFT.HSMT_PRINT_DESIGN WHERE BRANCH_ID = " + (object)coBranchId1, conn))
                {
                    dbCommand.Transaction = (IDbTransaction)oracleTransaction;
                    dbCommand.ExecuteNonQuery();
                }
                if (designList != null)
                {
                    for (int index = 0; index < designList.Length; ++index)
                    {
                        string format = "INSERT INTO {0}\r\n                                                                (CO_ID, BRANCH_ID, SECTION, ROW_INCREMENT, COLUMN_INCREMENT, ALIGN, LENGTH, DOC_TYPE, CREATE_DATE, CREATE_USER_ID) \r\n                                                         VALUES ({1}       ,{2}     ,{3}      ,{4}           ,{5}            ,{6}   , {7}   , {8}    ,TO_DATE('{9}','dd/MM/yyyy')  ,{10}     )";
                        object[] objArray = new object[11]
                        {
              (object) "UYUMSOFT.HSMT_PRINT_DESIGN",
              (object) coBranchId2,
              (object) coBranchId1,
              (object) designList[index].Section,
              (object) designList[index].RowIncrement,
              (object) designList[index].ColumnIncrement,
              (object) designList[index].Align,
              (object) designList[index].Length,
              (object) ConvertToInt32(designList[index].Type),
              (object) DateTime.Now.ToString("dd/MM/yyyy", (IFormatProvider) cultureInfo),
              (object) coBranchId3
                        };
                        //using (dbCommand = (IDbCommand)new OracleCommand(string.Format(format, objArray), conn))
                        {
                            dbCommand.CommandText = string.Format(format, objArray);
                            dbCommand.Transaction = (IDbTransaction)oracleTransaction;
                            if (dbCommand.ExecuteNonQuery() != 1)
                                return "Print section kaydolamadı";
                        }
                    }
                }
                oracleTransaction.Commit();
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                return ex.Message;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
        }


        internal static string SaveSystemLog(HotSaleServiceTables.Token token, IDbConnection conn, SystemLog[] logList)
        {
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coid = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int userId = Helper.GetCoBranchId(token, conn, userParameters, "U");
                oracleTransaction = conn.BeginTransaction();
                if (logList != null)
                {
                    dbCommand = conn.CreateCommand();
                    for (int index = 0; index < logList.Length; ++index)
                    {
                        string format = "INSERT INTO UYUMSOFT.HSMT_HISTORY (HISTORY_SOURCE, CREATE_DATE, USER_CODE, HISTORY_NUMBER, DESCRIPTION, HISTORY_TYPE, BRANCH_ID, CO_ID, HISTORY_DATE, CREATE_USER_ID) " +
                            "VALUES ('El Terminali', TO_DATE('{0}','dd/MM/yyyy hh24:mi:ss') ,'{1}', '{2}', '{3}', '{4}', {5}, {6}, TO_DATE('{7}','dd/MM/yyyy hh24:mi:ss'), {8})";
                        object[] objArray = new object[9]
                        {
                              (object) DateTime.Now.ToString(),
                              (object) userParameters.UserName.Replace("'",""),
                              (object) logList[index].MessageNumber.Replace("'",""),
                              (object) logList[index].MessageText.Replace("'",""),
                              (object) logList[index].LogType.Replace("'",""),
                              (object) coBranchId,
                              (object) coid,
                              (object) logList[index].LogDate.Replace("'","").Replace(".","/") + ":00",
                              (object) userId,

                        };
                        try
                        {
                            dbCommand.CommandText = string.Format(format, objArray);
                            dbCommand.Transaction = (IDbTransaction)oracleTransaction;
                            if (dbCommand.ExecuteNonQuery() != 1)
                                return "Log kaydolamadı";
                        }
                        catch { }
                       
                    }
                }
                oracleTransaction.Commit();
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                return ex.Message;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
        }

        private static string SaveDepositTransaction(HotSaleServiceTables.Token token, IDbConnection conn, DepositTransaction[] depositTransaction)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (depositTransaction.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                Helper.GetCoBranchId(token, conn, userParameters, "DIT");
                Helper.GetCatCodeId(conn, userParameters.CatCode1);
                Helper.GetCatCodeId(conn, userParameters.CatCode2);
                List<SelectedParameters> selectedParamsForEntity = Helper.GetSelectedParamsForEntity(((IEnumerable<DepositTransaction>)depositTransaction).Select<DepositTransaction, string>((Func<DepositTransaction, string>)(x => x.EntityCode)).Distinct<string>().ToArray<string>(), conn);
                List<SelectedParameters> paramsForDepositCard = Helper.GetSelectedParamsForDepositCard(((IEnumerable<DepositTransaction>)depositTransaction).Select<DepositTransaction, string>((Func<DepositTransaction, string>)(x => x.DepositCode)).Distinct<string>().ToArray<string>(), conn);
                List<SelectedParameters> paramsForLoadingCard = Helper.GetSelectedParamsForLoadingCard(((IEnumerable<DepositTransaction>)depositTransaction).Select<DepositTransaction, string>((Func<DepositTransaction, string>)(x => x.LoadingCard)).Distinct<string>().ToArray<string>(), conn);
                foreach (DepositTransaction depositTransaction1 in depositTransaction)
                {
                    DepositTransaction depositTrans = depositTransaction1;
                    if (!Helper.IsDepositTransExists(depositTrans, conn))
                    {
                        SelectedParameters selectedParameters1 = selectedParamsForEntity.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == depositTrans.EntityCode)).FirstOrDefault<SelectedParameters>();
                        if (selectedParameters1 == null)
                            throw new Exception("Cari Bulunamadı. Cari Kodu : " + depositTrans.EntityCode);
                        SelectedParameters selectedParameters2 = paramsForDepositCard.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == depositTrans.DepositCode)).FirstOrDefault<SelectedParameters>();
                        if (selectedParameters2 == null)
                            throw new Exception("Depozito kartı bulunamadı. Deposizto Kodu : " + depositTrans.DepositCode);
                        SelectedParameters selectedParameters3 = paramsForLoadingCard.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == depositTrans.LoadingCard)).FirstOrDefault<SelectedParameters>();
                        if (selectedParameters3 == null)
                            throw new Exception("Yükleme Kartı Buluanamdı. Yükleme Kodu : " + depositTrans.LoadingCard);
                        oracleTransaction = conn.BeginTransaction();
                        command = conn.CreateCommand();
                        command.CommandText = string.Format("INSERT INTO HSMT_DEPOSIT_TRANSACTION (SOURCE_GUID, CREATE_USER_ID,CREATE_DATE,DEPOSIT_CARD_ID,CO_ID,BRANCH_ID,ENTITY_ID,SALES_PERSON_ID,DOC_DATE,LOADING_CARD_ID,QTY_IN,QTY_OUT,QTY_WASTE)\r\n                                                                                   VALUES (:SourceGuid, :CreateUserId ,:CreateDate,:DepositCardId ,:CoId,:BranchId,:EntityId,:SalesPersonId ,:DocDate,:LoadingCardId ,:QtyIn,:QtyOut,:QtyWaste) returning DEPOSIT_TRANSACTION_ID into :myOutputParameter");
                        command.Transaction = (IDbTransaction)oracleTransaction;
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)depositTrans.SourceGuid));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)DateTime.Now));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DepositCardId", (object)selectedParameters2.Id));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)selectedParameters1.Id));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "DocDate", (object)DateTime.Today));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "LoadingCardId", (object)selectedParameters3.Id));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "QtyIn", (object)depositTrans.QtyDrop));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "QtyOut", (object)depositTrans.QtyTake));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "QtyWaste", (object)0));
                        IDataParameter parameter = (IDataParameter)command.CreateParameter();
                        parameter.DbType = DbType.Int32;
                        parameter.Direction = ParameterDirection.Output;
                        parameter.ParameterName = "myOutputParameter";
                        command.Parameters.Add((object)parameter);
                        if (command.ExecuteNonQuery() != 1)
                            return "Depozito Hareket kaydolamadı.";
                        ConvertToInt32(parameter.Value);
                        oracleTransaction.Commit();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        private static bool IsDepositTransExists(DepositTransaction depositTrans, IDbConnection conn)
        {
            IDbCommand oracleCommand = conn.CreateCommand();
            oracleCommand.CommandText = string.Format("SELECT 1 FROM UYUMSOFT.HSMT_DEPOSIT_TRANSACTION DT WHERE DT.SOURCE_GUID = '{0}'", (object)depositTrans.SourceGuid);
            using (var dr = oracleCommand.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT 1 FROM UYUMSOFT.HSMT_DEPOSIT_TRANSACTION DT WHERE DT.SOURCE_GUID = '{0}'", (object)depositTrans.SourceGuid), conn).ExecuteReader().Read();
        }

        internal static string SaveOneToOne(HotSaleServiceTables.Token token, IDbConnection conn, OneToOneM[] oneToOneList, bool notOnline)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (oneToOneList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (userParameters.IsSaveRealOneToOne && !notOnline)
                    return Helper.SaveOnlineOneToOne(token, conn, oneToOneList);
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "DIT");
                int coBranchId6 = Helper.GetCoBranchId(token, conn, userParameters, "DITC2"); // DITC idi 27.03.2023
                int docTraId = Helper.GetDocTraId(conn, userParameters.ConsigneReturnDocTraCode);
                int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                List<SelectedParameters> paramsForOneToOne1 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "E");
                List<SelectedParameters> paramsForOneToOne2 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "W");
                List<SelectedParameters> paramsForOneToOne3 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "ID");
                List<SelectedParameters> paramsForOneToOne4 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "U");
                foreach (OneToOneM oneToOne1 in oneToOneList)
                {
                    OneToOneM oneToOne = oneToOne1;
                    if (!Helper.IsOneToOneExists(oneToOne, conn))
                    {
                        SelectedParameters selectedParameters1 = paramsForOneToOne1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == oneToOne.EntityCode)).FirstOrDefault<SelectedParameters>();
                        SelectedParameters selectedParameters2 = paramsForOneToOne2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == oneToOne.WhouseCodeOut)).FirstOrDefault<SelectedParameters>();
                        SelectedParameters selectedParameters3 = paramsForOneToOne2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == oneToOne.WhouseCodeIn)).FirstOrDefault<SelectedParameters>();
                        oracleTransaction = conn.BeginTransaction();
                        GNL.SourceApplication sourceApplication1 = GNL.SourceApplication.Stok;
                        int num1 = coBranchId5;
                        if (oneToOne.SourceApp == "Konsinye")
                        {
                            sourceApplication1 = GNL.SourceApplication.Konsinye;
                            num1 = coBranchId6;
                        }
                        else if (oneToOne.SourceApp == "KonsinyeIade")
                        {
                            sourceApplication1 = GNL.SourceApplication.KonsinyeIade;
                            num1 = docTraId;
                        }
                        EventLog.WriteEntry("Application", "oneToOne.SourceApp : " + oneToOne.SourceApp, EventLogEntryType.Information, 20109);

                        command = conn.CreateCommand();

                        // DOCTRA_ID eksik?
                        // old command.CommandText = "INSERT INTO UYUMSOFT.HSMT_ITEM_M  (DOC_NO, DOC_DATE, WHOUSE_ID, WHOUSE_ID2, CO_ID, BRANCH_ID, SOURCE_APP, ENTITY_ID, SALES_PERSON_ID, CREATE_DATE, CREATE_USER_ID, LATITUDE , LONGITUDE , DOC_TRA_ID, IS_UPLOAD,CAT_CODE1_ID,CAT_CODE2_ID,SOURCE_GUID,IS_DELETED)\r\n                                                                            values (:DocNo,:DocDate , :WhouseId, :WhouseId2, :CoId, :BranchId, :SourceApp, :EntityId, :SalesPersonId , :CreateDate, :CreateUserId , :Latitude, :Longitude, :DocTraId , :IsUpload,:CatCode1Id ,:CatCode2Id ,:SourceGuid,:IsDeleted) returning HSM_ITEM_M_ID into :myOutputParameter";
                        command.CommandText = "INSERT INTO UYUMSOFT.HSMT_ITEM_M  (DOC_NO, DOC_DATE, WHOUSE_ID, WHOUSE_ID2, CO_ID, BRANCH_ID, SOURCE_APP, ENTITY_ID, SALES_PERSON_ID, CREATE_DATE, CREATE_USER_ID, LATITUDE , LONGITUDE , DOC_TRA_ID, IS_UPLOAD,CAT_CODE1_ID,CAT_CODE2_ID,SOURCE_GUID,IS_DELETED)\r\n                                                                            values (:DocNo,:DocDate , :WhouseId, :WhouseId2, :CoId, :BranchId, :SourceApp, :EntityId, :SalesPersonId , :CreateDate, :CreateUserId , :Latitude, :Longitude, :DocTraId , :IsUpload,:CatCode1Id ,:CatCode2Id ,:SourceGuid,:IsDeleted)";

                        command.Transaction = (IDbTransaction)oracleTransaction;
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "DocNo", (object)oneToOne.DocNo));

                        command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "DocDate", (object)oneToOne.DocDate.Date.ToLocalTime())); // 07.04.2023

                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "WhouseId", (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "WhouseId2", (object)(selectedParameters3 == null ? 0 : selectedParameters3.Id)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)ConvertToInt32((object)sourceApplication1)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));

                        command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)oneToOne.DocDate.ToLocalTime())); // 07.04.2023

                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)oneToOne.Latitude));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)oneToOne.Longitude));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DocTraId", (object)num1));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode1Id", (object)catCodeId1));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode2Id", (object)catCodeId2));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)oneToOne.SourceGuid));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsDeleted", (object)0));

                        if (command.ExecuteNonQuery() != 1)
                            return "Gün sonu stokları kaydolamadı.";

                        #region iptal çalışmıyor!..
                        /*IDataParameter parameter = (IDataParameter)command.CreateParameter();
                        parameter.DbType = DbType.Int32;
                        parameter.Direction = ParameterDirection.Output;
                        parameter.ParameterName = "myOutputParameter";
                        command.Parameters.Add((object)parameter);
                        if (command.ExecuteNonQuery() != 1)
                            return "Gün sonu stokları kaydolamadı.";
                        int int32 = ConvertToInt32(parameter.Value); */
                        #endregion

                        int int32 = 0;
                        {
                            if (conn is OracleConnection)
                            {
                                command.CommandText = "select HSM_ITEM_M_ID_HSMT_ITEM_M.currval from dual";
                                int32 = ConvertToInt32(command.ExecuteScalar());
                            }
                            else
                            {
                                command.CommandText = "select currval('hsmt_item_m_hsm_item_m_id_seq'::regclass)";
                                int32 = ConvertToInt32(command.ExecuteScalar());
                            }
                        }


                        int num2 = 10;
                        int index = 0;
                        while (index < oneToOne.OneToOneDetailList.Length)
                        {
                            OneToOneD detail = oneToOne.OneToOneDetailList[index];
                            SelectedParameters selectedParameters4 = paramsForOneToOne3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters5 = paramsForOneToOne4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();
                            Decimal num3 = detail.Price * detail.Qty;
                            GNL.SourceApplication sourceApplication2 = (GNL.SourceApplication)0;
                            if (detail.SourceApp == "Siparis")
                                sourceApplication2 = GNL.SourceApplication.SatışSiparişi;
                            string cmdText = "INSERT INTO UYUMSOFT.HSMT_ITEM_D  (ITEM_ID, QTY , UNIT_ID, BRANCH_ID, CO_ID, LINE_NO, SOURCE_APP, WHOUSE_ID, WHOUSE_ID2, UNIT_PRICE, AMT , DISC_RATE1, DISC_RATE2, HSM_ITEM_M_ID, DISC_ID1, DISC_ID2, COST_CENTER_ID, SOURCE_M_ID, SOURCE_D_ID) \r\n                                                                                 values (:ItemId, :Qty, :UnitId, :BranchId, :CoId, :LineNo , :SourceApp , :WhouseId, :WhouseId2, :UnitPrice , :Amt, :DiscRate1, :DiscRate2, :HsmItemMId   , :DiscId1, :DiscId2, :CostCenterId , :SourceMId , :SourceDId )";
                            command.Dispose();

                            command = conn.CreateCommand();
                            command.CommandText = cmdText;

                            //command = (IDbCommand)new OracleCommand(cmdText, conn);
                            command.Transaction = (IDbTransaction)oracleTransaction;
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "ItemId", (object)(selectedParameters4 == null ? 0 : selectedParameters4.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Qty", (object)detail.Qty));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "UnitId", (object)(selectedParameters5 == null ? 0 : selectedParameters5.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "LineNo", (object)num2));
                            if ((uint)sourceApplication2 > 0U)
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)ConvertToInt32((object)sourceApplication2)));
                            else
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)ConvertToInt32((object)sourceApplication1)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "WhouseId", (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "WhouseId2", (object)(selectedParameters3 == null ? 0 : selectedParameters3.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "UnitPrice", (object)detail.Price));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Amt", (object)num3));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "DiscRate1", (object)detail.DiscRate1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "DiscRate2", (object)detail.DiscRate2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "HsmItemMId", (object)int32));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DiscId1", (object)(detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DiscId2", (object)(detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CostCenterId", (object)costCenterId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SourceMId", (object)detail.ERPSourceMId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SourceDId", (object)detail.ERPSourceDId));
                            if (command.ExecuteNonQuery() != 1)
                                throw new Exception("Could not save items");
                            ++index;
                            num2 += 10;
                        }
                        oracleTransaction.Commit();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        private static int GetDocTraId(IDbConnection conn, string consigneReturnDocTraCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            int num = 0;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.DOC_TRA_ID                                                \r\n                                                FROM UYUMSOFT.GNLD_DOC_TRA A\r\n\r\n                                                 WHERE   A.DOC_TRA_CODE  = '{0}'\r\n                                                 ", (object)consigneReturnDocTraCode);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    num = ConvertToInt32(oracleDataReader[0]);
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return num;
        }

        private static bool IsOneToOneExists(OneToOneM oneToOne, IDbConnection conn)
        {
            IDbCommand oracleCommand = conn.CreateCommand();
            oracleCommand.CommandText = string.Format("SELECT ITM.HSM_ITEM_M_ID FROM UYUMSOFT.HSMT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)oneToOne.SourceGuid);
            using (var dr = oracleCommand.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT ITM.HSM_ITEM_M_ID FROM UYUMSOFT.HSMT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)oneToOne.SourceGuid), conn).ExecuteReader().Read();
        }

        internal static string SaveOnlineOneToOne(HotSaleServiceTables.Token token, IDbConnection conn, OneToOneM[] oneToOneList)
        {
            string str = string.Empty;
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (oneToOneList.Length == 0)
                    return str;
                GeneralSenfoniService generalSenfoniService = GetGeneralSenfoniWebService();
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (!userParameters.IsSaveRealOneToOne)
                {
                    str = Helper.SaveOneToOne(token, conn, oneToOneList, false);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "DIT");
                    int coBranchId6 = Helper.GetCoBranchId(token, conn, userParameters, "DITC");
                    int docTraId = Helper.GetDocTraId(conn, userParameters.ConsigneReturnDocTraCode);
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                    int coCurId = Helper.GetCoCurId(token, conn, coBranchId2);
                    List<SelectedParameters> paramsForOneToOne1 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "E");
                    List<SelectedParameters> paramsForOneToOne2 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "W");
                    List<SelectedParameters> paramsForOneToOne3 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "ID");
                    List<SelectedParameters> paramsForOneToOne4 = Helper.GetSelectedParamsForOneToOne(oneToOneList, conn, "U");
                    List<ItemDef> itemDefList = new List<ItemDef>();
                    foreach (OneToOneM oneToOneM1 in ((IEnumerable<OneToOneM>)oneToOneList).Where<OneToOneM>((Func<OneToOneM, bool>)(x => (uint)((IEnumerable<OneToOneD>)x.OneToOneDetailList).Count<OneToOneD>() > 0U)).ToList<OneToOneM>())
                    {
                        OneToOneM oneToOneM = oneToOneM1;
                        if (!Helper.IsOnlineOneToOneExists(oneToOneM, conn))
                        {
                            ItemDef itemDef = new ItemDef();
                            HotSaleSenfoniAppServer.Senfoni.SourceApplication sourceApplication = HotSaleSenfoniAppServer.Senfoni.SourceApplication.Stok;
                            int num1 = coBranchId5;
                            if (oneToOneM.SourceApp == "Konsinye")
                            {
                                sourceApplication = HotSaleSenfoniAppServer.Senfoni.SourceApplication.Konsinye;
                                num1 = coBranchId6;
                            }
                            else if (oneToOneM.SourceApp == "KonsinyeIade")
                            {
                                sourceApplication = HotSaleSenfoniAppServer.Senfoni.SourceApplication.KonsinyeIade;
                                num1 = docTraId;
                            }
                            SelectedParameters selectedParameters1 = paramsForOneToOne1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == oneToOneM.EntityCode)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters2 = paramsForOneToOne2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == oneToOneM.WhouseCodeOut)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters3 = paramsForOneToOne2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == oneToOneM.WhouseCodeIn)).FirstOrDefault<SelectedParameters>();
                            itemDef.DocNo = oneToOneM.DocNo;

                            itemDef.DocDate = oneToOneM.DocDate.ToLocalTime().Date; // 07.04.2023

                            itemDef.WhouseId = selectedParameters2 == null ? 0 : selectedParameters2.Id;
                            itemDef.WhouseId2 = selectedParameters3 == null ? 0 : selectedParameters3.Id;
                            itemDef.CoId = coBranchId2;
                            itemDef.CoCode = userParameters.CoCode;
                            itemDef.BranchId = coBranchId1;
                            itemDef.BranchCode = userParameters.BranchCode;
                            itemDef.SourceApp = sourceApplication;
                            itemDef.SourceApp2 = HotSaleSenfoniAppServer.Senfoni.SourceApplication.SicakSatis;

                            itemDef.SourceApp3 = itemDef.SourceApp;

                            itemDef.EntityId = selectedParameters1 == null ? 0 : selectedParameters1.Id;
                            itemDef.SalesPersonId = coBranchId3;
                            itemDef.SalesPersonCode = userParameters.SalesPersonCode;
                            itemDef.CreateUserId = coBranchId4;
                            itemDef.Latitude = oneToOneM.Latitude;
                            itemDef.Longitude = oneToOneM.Longitude;
                            itemDef.DocTraId = num1;
                            itemDef.CatCode1Id = catCodeId1;
                            itemDef.CatCode1 = userParameters.CatCode1;
                            itemDef.CatCode2Id = catCodeId2;
                            itemDef.CatCode2 = userParameters.CatCode2;
                            itemDef.SourceGuid = oneToOneM.SourceGuid;
                            itemDef.CurId = coCurId;
                            itemDef.CurTra = Decimal.One;
                            itemDef.CurRateTypeId = 0;
                            List<ItemDetailDef> itemDetailDefList = new List<ItemDetailDef>();
                            int num2 = 10;
                            int index = 0;
                            while (index < oneToOneM.OneToOneDetailList.Length)
                            {
                                OneToOneD detail = oneToOneM.OneToOneDetailList[index];
                                ItemDetailDef itemDetailDef = new ItemDetailDef();
                                SelectedParameters selectedParameters4 = paramsForOneToOne3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters5 = paramsForOneToOne4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();
                                Decimal num3 = detail.Price * detail.Qty;
                                itemDetailDef.DcardId = selectedParameters4 == null ? 0 : selectedParameters4.Id;

                                itemDetailDef.SourceApp = itemDef.SourceApp;

                                itemDetailDef.Qty = detail.Qty;
                                itemDetailDef.QtyPrm = detail.QtyPrm;
                                itemDetailDef.LineNo = num2;
                                itemDetailDef.WhouseId = selectedParameters2 == null ? 0 : selectedParameters2.Id;
                                itemDetailDef.UnitPrice = detail.Price;
                                itemDetailDef.UnitPriceTra = detail.Price;
                                itemDetailDef.Amt = num3;
                                itemDetailDef.Disc1Rate = detail.DiscRate1;
                                itemDetailDef.Disc1Id = detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1;
                                itemDetailDef.Disc2Rate = detail.DiscRate2;
                                itemDetailDef.Disc2Id = detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2;
                                itemDetailDef.UnitId = selectedParameters5 == null ? 0 : selectedParameters5.Id;
                                itemDetailDef.CurTraId = coCurId;
                                itemDetailDef.CurRateTra = Decimal.One;
                                itemDetailDef.CurRateTypeId = 0;
                                itemDetailDef.CostCenterId = costCenterId;
                                itemDetailDefList.Add(itemDetailDef);
                                ++index;
                                num2 += 10;
                            }
                            itemDef.Details = itemDetailDefList.ToArray();
                            itemDefList.Add(itemDef);
                        }
                    }
                    List<OneToOneM> list = ((IEnumerable<OneToOneM>)oneToOneList).Where<OneToOneM>((Func<OneToOneM, bool>)(x => ((IEnumerable<OneToOneD>)x.OneToOneDetailList).Count<OneToOneD>() == 0)).ToList<OneToOneM>();
                    if ((uint)list.Count > 0U)
                        str = Helper.SaveOneToOne(token, conn, list.ToArray(), true);
                    HotSaleSenfoniAppServer.Senfoni.Token senfoniServiceToken = Helper.CreateSenfoniServiceToken(coBranchId2, coBranchId1, token.Username, conn);
                    if ((uint)itemDefList.Count > 0U)
                    {
                        EventLog.WriteEntry("Application", "BranchCode : " + itemDefList[0].BranchCode + " BrahcId : " + itemDefList[0].BranchId.ToString(), EventLogEntryType.Information, 19000);
                        ServiceResultOfBoolean serviceResultOfBoolean = generalSenfoniService.SaveItemMMulti(senfoniServiceToken, itemDefList.ToArray());
                        if (serviceResultOfBoolean.Result)
                            return string.Empty;
                        return serviceResultOfBoolean.Message;
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
        }

        private static bool IsOnlineOneToOneExists(OneToOneM oneToOneM, IDbConnection conn)
        {
            IDbCommand oracleCommand = conn.CreateCommand();
            oracleCommand.CommandText = string.Format("SELECT ITM.ITEM_M_ID FROM UYUMSOFT.INVT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)oneToOneM.SourceGuid);
            using (var dr = oracleCommand.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT ITM.ITEM_M_ID FROM UYUMSOFT.INVT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)oneToOneM.SourceGuid), conn).ExecuteReader().Read();
        }

        internal static string SaveInvoice(HotSaleServiceTables.Token token, IDbConnection conn, InvoiceM[] invoiceMList, bool dontConsiderSaveReal)
        {
            IDbCommand command1 = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (invoiceMList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (userParameters.IsSaveRealInvoice && !dontConsiderSaveReal)
                    return Helper.SaveOnlineInvoice(token, conn, invoiceMList);
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "DSI");
                int coBranchId6 = Helper.GetCoBranchId(token, conn, userParameters, "DSIR");
                int num1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                int catCodeId = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                List<SelectedParameters> paramsForInvoiceM1 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "E");
                List<SelectedParameters> paramsForInvoiceM2 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "W");
                List<SelectedParameters> paramsForInvoiceM3 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "ID");
                List<SelectedParameters> paramsForInvoiceM4 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "U");
                List<ItemTax> vatIdForInvoiceM = Helper.GetVatIdForInvoiceM(invoiceMList, conn, "VAT");
                List<SelectedParameters> catCodeIdList = Helper.GetCatCodeIdList(invoiceMList, conn);
                foreach (InvoiceM invoiceM1 in invoiceMList)
                {
                    InvoiceM invoiceM = invoiceM1;
                    if (!Helper.IsInvoiceExists(invoiceM, conn))
                    {
                        SelectedParameters selectedParameters1 = paramsForInvoiceM1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == invoiceM.EntityCode)).FirstOrDefault<SelectedParameters>();
                        SelectedParameters selectedParameters2 = catCodeIdList.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == invoiceM.CatCode1));
                        if (selectedParameters2 != null)
                            num1 = selectedParameters2.Id;
                        if (catCodeIdList.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == invoiceM.CatCode2)) != null)
                            num1 = selectedParameters2.Id;
                        oracleTransaction = conn.BeginTransaction();
                        int num2 = !(invoiceM.SourceApp == "SatisFaturasi") ? (!(invoiceM.SourceApp == "Siparis") ? 1000 : 122) : 2;
                        command1 = conn.CreateCommand();
                        command1.CommandText = @"begin INSERT INTO UYUMSOFT.HSMT_INVOICE_M  (DOC_TRA_ID, DOC_NO, DOC_DATE, ENTITY_ID, BRANCH_ID, CO_ID, SALES_PERSON_ID, 
                                                    DUE_DATE, CREATE_DATE, SOURCE_APP, SOURCE_M_ID, LATITUDE , LONGITUDE , CREATE_USER_ID, IS_UPLOAD,DISC0_ID,DISC0_RATE,CAT_CODE1_ID,CAT_CODE2_ID,
                                                    IS_DELETED, AMT , AMT_DISC_TOTAL, AMT_RECEIPT, AMT_VAT,SOURCE_GUID)
                                                    values (:DocTraId , :DocNo, :DocDate, :EntityId, :BranchId, :CoId, :SalesPersonId , :DueDate, :CreateDate, :SourceApp, :SourceMId , :Latitude, 
                                                    :Longitude, :CreateUserId , :IsUpload,:Disc0Id,:Disc0Rate,:CatCode1Id ,:CatCode2Id ,:IsDeleted, :Amt, :AmtDiscTotal , :AmtReceipt, :AmtVat,
                                                    :SourceGuid) returning HSM_INVOICE_M_ID into :HSM_INVOICE_M_ID; end;";
                        command1.Transaction = (IDbTransaction)oracleTransaction;
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DocTraId", (object)(invoiceM.PurchaseSales == "Satis" ? coBranchId5 : coBranchId6)));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.String, ParameterDirection.Input, "DocNo", (object)invoiceM.DocNo));
                        IDataParameterCollection parameters1 = command1.Parameters;
                        IDbCommand command2 = command1;
                        int num3 = 5;
                        int num4 = 1;
                        string parameterName1 = "DocDate";

                        DateTime dateTime = invoiceM.DocDate.ToLocalTime(); // 07.04.2023

                        // ISSUE: variable of a boxed type
                        DateTime date1 = dateTime.Date;
                        //__Boxed<DateTime> date1 = (System.ValueType) dateTime.Date;
                        object obj1 = Helper.CreateParam(command2, (DbType)num3, (ParameterDirection)num4, parameterName1, (object)date1);
                        parameters1.Add(obj1);
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id)));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                        IDataParameterCollection parameters2 = command1.Parameters;
                        IDbCommand command3 = command1;
                        int num5 = 5;
                        int num6 = 1;
                        string parameterName2 = "DueDate";

                        dateTime = invoiceM.DueDate.ToLocalTime(); // 07.04.2023

                        // ISSUE: variable of a boxed type
                        //__Boxed<DateTime> date2 = (System.ValueType) dateTime.Date;
                        DateTime date2 = dateTime.Date;
                        object obj2 = Helper.CreateParam(command3, (DbType)num5, (ParameterDirection)num6, parameterName2, (object)date2);
                        parameters2.Add(obj2);

                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)invoiceM.DocDate.ToLocalTime())); // 07.04.2023

                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)num2));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SourceMId", (object)invoiceM.SourceMId));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)invoiceM.Latitude));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)invoiceM.Longitude));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "Disc0Id", (object)(invoiceM.MasterDiscRate != Decimal.Zero ? userParameters.DiscId0 : 0)));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Disc0Rate", (object)invoiceM.MasterDiscRate));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CatCode1Id", (object)num1));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CatCode2Id", (object)catCodeId));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "IsDeleted", (object)ConvertToInt32(invoiceM.IsDeleted)));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Amt", (object)invoiceM.Amt));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "AmtDiscTotal", (object)invoiceM.AmtDisc));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "AmtReceipt", (object)invoiceM.AmtReceipt));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "AmtVat", (object)invoiceM.AmtVat));
                        command1.Parameters.Add(Helper.CreateParam(command1, DbType.String, ParameterDirection.Input, "SourceGuid", (object)invoiceM.SourceGuid));
                        command1.Parameters.Add(new OracleParameter("HSM_INVOICE_M_ID", OracleDbType.Int32, ParameterDirection.Output));

                        //IDataParameter parameter = (IDataParameter)command1.CreateParameter();
                        //parameter.DbType = DbType.Int32;
                        //parameter.Direction = ParameterDirection.Output;
                        //parameter.ParameterName = "myOutputParameter";
                        //command1.Parameters.Add((object)parameter);
                        int a = command1.ExecuteNonQuery();
                        int int32 = int.Parse((command1.Parameters["HSM_INVOICE_M_ID"] as OracleParameter).Value.ToString());
                        int num7 = 10;
                        int index = 0;
                        while (index < invoiceM.InvoiceDList.Length)
                        {
                            InvoiceD detail = invoiceM.InvoiceDList[index];
                            SelectedParameters selectedParameters3 = paramsForInvoiceM2.Where(x => x.Code == invoiceM.WhouseCode).FirstOrDefault();
                            //SelectedParameters selectedParameters3 = paramsForInvoiceM2.Cast<SelectedParameters>().Where<SelectedParameters>(closure_0 ?? (closure_0 = (Func<SelectedParameters, bool>)(x => x.Code == invoiceM.WhouseCode))).FirstOrDefault<SelectedParameters>();
                            SelectedParameters tmpItem = paramsForInvoiceM3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters4 = paramsForInvoiceM4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();
                            ItemTax itemTax = vatIdForInvoiceM.Cast<ItemTax>().Where<ItemTax>((Func<ItemTax, bool>)(x =>
                           {
                               if (x.ItemId == tmpItem.Id && x.StartDate <= invoiceM.DocDate)
                                   return x.EndDate >= invoiceM.DocDate;
                               return false;
                           })).FirstOrDefault<ItemTax>();
                            string cmdText = "INSERT INTO UYUMSOFT.HSMT_INVOICE_D  (WHOUSE_ID, ITEM_ID, QTY , UNIT_ID, UNIT_PRICE, DISC_RATE1, DISC_RATE2, TAX_RATE, VAT_STATUS, BRANCH_ID, CO_ID, LINE_NO, SOURCE_D_ID, SOURCE_M_ID, SOURCE_APP, HSM_INVOICE_M_ID, DISC_ID1, DISC_ID2, VAT_ID, QTY_PRM, DUE_DAY, CAMPAIGN_ID, COST_CENTER_ID) \r\n                                                                                     values (:WhouseId, :ItemId, :Qty, :UnitId, :UnitPrice, :DiscRate1, :DiscRate2, :TaxRate, :VatStatus, :BranchId, :CoId, :LineNo, :SourceDId , :SourceMId , :SourceApp, :HsmInvoiceMId  , :DiscId1, :DiscId2, :VatId, :QtyPrm, :DueDay, :CampaignId, :CostCenterId )";
                            command1.Dispose();
                            command1 = conn.CreateCommand();
                            command1.CommandText = cmdText;
                            command1.Transaction = (IDbTransaction)oracleTransaction;
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "WhouseId", (object)(selectedParameters3 == null ? 0 : selectedParameters3.Id)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "ItemId", (object)(tmpItem == null ? 0 : tmpItem.Id)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Qty", (object)detail.Qty));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "UnitId", (object)(selectedParameters4 == null ? 0 : selectedParameters4.Id)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "UnitPrice", (object)detail.Price));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "DiscRate1", (object)detail.DiscRate1));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "DiscRate2", (object)detail.DiscRate2));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "TaxRate", (object)detail.VatRate));
                            if (!detail.VatStatus)
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "VatStatus", (object)ConvertToInt32((object)GNL.VatStatus.Hariç)));
                            else
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "VatStatus", (object)ConvertToInt32((object)GNL.VatStatus.Dahil)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "LineNo", (object)num7));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SourceDId", (object)detail.SourceDId));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SourceMId", (object)detail.SourceMId));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)num2));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "HsmInvoiceMId", (object)int32));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DiscId1", (object)(detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DiscId2", (object)(detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "VatId", (object)(itemTax == null ? 0 : itemTax.TaxId)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "QtyPrm", (object)detail.QtyPrm));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DueDay", (object)detail.DueDay));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CampaignId", (object)detail.CampaignId));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CostCenterId", (object)costCenterId));
                            if (command1.ExecuteNonQuery() != 1)
                                throw new Exception("Could not save items");
                            ++index;
                            num7 += 10;
                        }
                        oracleTransaction.Commit();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command1 != null)
                    command1.Dispose();
            }
        }

        private static int GetCostCenterId(IDbConnection conn, string costCenterCode)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            int num = 0;
            if (string.IsNullOrEmpty(costCenterCode))
                return 0;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT T.COST_CENTER_ID FROM FIND_COST_CENTER T WHERE T.COST_CENTER_CODE = '{0}'", (object)costCenterCode);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    num = ConvertToInt32(oracleDataReader[0]);
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return num;
        }

        internal static string SaveOnlineInvoice(HotSaleServiceTables.Token token, IDbConnection conn, InvoiceM[] invoiceMList)
        {
            string str = string.Empty;
            //IDbCommand dbCommand = (IDbCommand)null;
            try
            {
                if (invoiceMList.Length == 0)
                    return str;
                GeneralSenfoniService generalSenfoniService = GetGeneralSenfoniWebService();
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (!userParameters.IsSaveRealInvoice)
                {
                    str = Helper.SaveInvoice(token, conn, invoiceMList, true);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "DSI");
                    int coBranchId6 = Helper.GetCoBranchId(token, conn, userParameters, "DSIR");
                    int num1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    int coCurId = Helper.GetCoCurId(token, conn, coBranchId2);
                    int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                    List<SelectedParameters> paramsForInvoiceM1 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "E");
                    List<SelectedParameters> paramsForInvoiceM2 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "W");
                    List<SelectedParameters> paramsForInvoiceM3 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "ID");
                    List<SelectedParameters> paramsForInvoiceM4 = Helper.GetSelectedParamsForInvoiceM(invoiceMList, conn, "U");
                    List<ItemTax> vatIdForInvoiceM = Helper.GetVatIdForInvoiceM(invoiceMList, conn, "VAT");

                    List<SelectedParameters> reasonIdsFromOrder = Helper.GetReasonIdsFromInvoice(invoiceMList, conn); // 23.03.2022

                    List<SelectedParameters> catCodeIdList = Helper.GetCatCodeIdList(invoiceMList, conn);
                    List<InvoiceDef> invoiceDefList = new List<InvoiceDef>();
                    foreach (InvoiceM invoiceM1 in ((IEnumerable<InvoiceM>)invoiceMList).Where<InvoiceM>((Func<InvoiceM, bool>)(x => (uint)((IEnumerable<InvoiceD>)x.InvoiceDList).Count<InvoiceD>() > 0U)).ToList<InvoiceM>())
                    {
                        InvoiceM invoiceM = invoiceM1;
                        if (!Helper.IsOnlineInvoiceExists(invoiceM, conn))
                        {
                            SelectedParameters selectedParameters1 = paramsForInvoiceM1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == invoiceM.EntityCode)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters2 = catCodeIdList.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == invoiceM.CatCode1));
                            if (selectedParameters2 != null)
                                num1 = selectedParameters2.Id;
                            if (catCodeIdList.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == invoiceM.CatCode2)) != null)
                                num1 = selectedParameters2.Id;

                            // bu nedir ? saçmalanın zirvesi
                            // orj SourceApplication sourceApplication = !(invoiceM.SourceApp == "SatisFaturasi") ? (!(invoiceM.SourceApp == "Siparis") ? SourceApplication.İrsaliye : SourceApplication.SatışSiparişi ) : SourceApplication.Fatura;

                            // 28.03.2022 n'oluyo
                            SourceApplication sourceApplication = !(invoiceM.SourceApp == "SatisFaturasi") ? (!(invoiceM.SourceApp == "Siparis") ? SourceApplication.İrsaliye : (!(invoiceM.PurchaseSales == "Satis") ? SourceApplication.SatınalmaSiparişi : SourceApplication.SatışSiparişi)) : SourceApplication.Fatura;

                            InvoiceDef invoiceDef1 = new InvoiceDef();

                            invoiceDef1.DocTraId = invoiceM.PurchaseSales == "Satis" ? coBranchId5 : coBranchId6;
                            invoiceDef1.DocNo = invoiceM.DocNo;
                            InvoiceDef invoiceDef2 = invoiceDef1;

                            DateTime dateTime1 = invoiceM.DocDate.ToLocalTime(); // 07.04.2023
                            int year1 = dateTime1.Year;

                            dateTime1 = invoiceM.DocDate.ToLocalTime(); // 07.04.2023
                            int month1 = dateTime1.Month;

                            dateTime1 = invoiceM.DocDate.ToLocalTime(); // 07.04.2023
                            int day1 = dateTime1.Day;

                            DateTime dateTime2 = new DateTime(year1, month1, day1);
                            invoiceDef2.DocDate = dateTime2;
                            invoiceDef1.MCardId = selectedParameters1 == null ? 0 : selectedParameters1.Id;
                            invoiceDef1.CardType = HotSaleSenfoniAppServer.Senfoni.CardType.Cari;
                            invoiceDef1.EntityId = selectedParameters1 == null ? 0 : selectedParameters1.Id;
                            invoiceDef1.BranchId = coBranchId1;
                            invoiceDef1.CoId = coBranchId2;
                            invoiceDef1.SalesPersonCode = userParameters.SalesPersonCode;
                            invoiceDef1.SalesPersonId = coBranchId3;
                            InvoiceDef invoiceDef3 = invoiceDef1;

                            dateTime1 = invoiceM.DueDate.ToLocalTime(); // 07.04.2023
                            int year2 = dateTime1.Year;

                            dateTime1 = invoiceM.DueDate.ToLocalTime(); // 07.04.2023
                            int month2 = dateTime1.Month;

                            dateTime1 = invoiceM.DueDate.ToLocalTime(); // 07.04.2023
                            int day2 = dateTime1.Day;

                            DateTime dateTime3 = new DateTime(year2, month2, day2);
                            invoiceDef3.DueDate = dateTime3;

                            invoiceDef1.SourceApp = sourceApplication; // this xxx
                            invoiceDef1.DontControlnegative = true; // 31.05.2022

                            // 09.03.2023 terminal tarihi
                            try
                            {
                                invoiceDef1.Note2 = string.Format("Terminal zamanı: {0}", dateTime1.ToString("dd.MM.yyyy HH:mm"));
                            } catch { } 

                            invoiceDef1.SourceMId = invoiceM.SourceMId;
                            invoiceDef1.Latitude = invoiceM.Latitude;
                            invoiceDef1.Longitude = invoiceM.Longitude;
                            invoiceDef1.CreateUserId = coBranchId4;
                            invoiceDef1.Disc0Id = invoiceM.MasterDiscRate != Decimal.Zero ? userParameters.DiscId0 : 0;
                            invoiceDef1.Disc0Rate = invoiceM.MasterDiscRate;
                            invoiceDef1.CatCode1Id = num1;
                            invoiceDef1.CatCode2Id = catCodeId;
                            invoiceDef1.Amt = invoiceM.Amt;
                            invoiceDef1.AmtDiscTotal = invoiceM.AmtDisc;
                            invoiceDef1.AmtReceipt = invoiceM.AmtReceipt;
                            invoiceDef1.AmtReceiptTra = invoiceM.AmtReceipt;
                            invoiceDef1.AmtVat = invoiceM.AmtVat;
                            invoiceDef1.AmtVatTra = invoiceM.AmtVat;
                            invoiceDef1.SourceGuid = invoiceM.SourceGuid;
                            invoiceDef1.CurId = coCurId;
                            invoiceDef1.CurCode = userParameters.CoCurCode;
                            invoiceDef1.CurRateTypeId = 0;
                            invoiceDef1.CurTra = Decimal.One;
                            List<InvoiceDetailDef> invoiceDetailDefList = new List<InvoiceDetailDef>();
                            int num2 = 10;
                            int index = 0;
                            while (index < invoiceM.InvoiceDList.Length)
                            {
                                InvoiceD detail = invoiceM.InvoiceDList[index];
                                SelectedParameters selectedParameters3 = paramsForInvoiceM2.Where(x => x.Code == invoiceM.WhouseCode).FirstOrDefault();
                                //SelectedParameters selectedParameters3 = paramsForInvoiceM2.Cast<SelectedParameters>().Where<SelectedParameters>(closure_0 ?? (closure_0 = (Func<SelectedParameters, bool>)(x => x.Code == invoiceM.WhouseCode))).FirstOrDefault<SelectedParameters>();
                                SelectedParameters tmpItem = paramsForInvoiceM3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters4 = paramsForInvoiceM4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();

                                ItemTax itemTax = vatIdForInvoiceM.Cast<ItemTax>().Where<ItemTax>((Func<ItemTax, bool>)(x =>
                               {
                                   if (x.ItemId == tmpItem.Id && x.StartDate <= invoiceM.DocDate)
                                       return x.EndDate >= invoiceM.DocDate;
                                   return false;
                               })).FirstOrDefault<ItemTax>();
                                InvoiceDetailDef invoiceDetailDef = new InvoiceDetailDef();
                                invoiceDetailDef.WhouseId = selectedParameters3 == null ? 0 : selectedParameters3.Id;
                                invoiceDetailDef.DcardId = tmpItem == null ? 0 : tmpItem.Id;
                                invoiceDetailDef.LineType = HotSaleSenfoniAppServer.Senfoni.LineType.S;
                                invoiceDetailDef.CurCode = userParameters.CoCurCode;
                                invoiceDetailDef.CurTraId = coCurId;
                                invoiceDetailDef.CurRateTra = Decimal.One;
                                invoiceDetailDef.CurRateTypeId = 0;
                                invoiceDetailDef.Qty = detail.Qty;
                                invoiceDetailDef.UnitId = selectedParameters4 == null ? 0 : selectedParameters4.Id;
                                invoiceDetailDef.UnitPrice = detail.Price;
                                invoiceDetailDef.UnitPriceTra = detail.Price;
                                invoiceDetailDef.Disc1Rate = detail.DiscRate1;
                                invoiceDetailDef.Disc2Rate = detail.DiscRate2;
                                invoiceDetailDef.Disc1Id = detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1;
                                invoiceDetailDef.Disc2Id = detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2;
                                invoiceDetailDef.VatRate = (Decimal)detail.VatRate;
                                if (!detail.VatStatus)
                                    invoiceDetailDef.VatStatus = HotSaleSenfoniAppServer.Senfoni.VatStatus.Hariç;
                                else
                                    invoiceDetailDef.VatStatus = HotSaleSenfoniAppServer.Senfoni.VatStatus.Dahil;
                                invoiceDetailDef.LineNo = num2;
                                invoiceDetailDef.SourceDId = detail.SourceDId;
                                invoiceDetailDef.SourceMId = detail.SourceMId;
                                invoiceDetailDef.Disc1Id = detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1;
                                invoiceDetailDef.Disc2Id = detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2;
                                invoiceDetailDef.VatId = itemTax == null ? 0 : itemTax.TaxId;
                                invoiceDetailDef.QtyPrm = detail.QtyPrm;
                                invoiceDetailDef.DueDay = detail.DueDay;
                                invoiceDetailDef.CampaignId = detail.CampaignId;
                                invoiceDetailDef.CostCenterId = costCenterId;

                                try
                                {
                                    // 23.03.2022
                                    SelectedParameters selectedParameters5 = reasonIdsFromOrder.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ReasonCode)).FirstOrDefault<SelectedParameters>();
                                    invoiceDetailDef.ReasonId = selectedParameters5 == null ? 0 : selectedParameters5.Id;
                                }
                                catch { }

                                invoiceDetailDefList.Add(invoiceDetailDef);
                                ++index;
                                num2 += 10;
                            }
                            invoiceDef1.Details = invoiceDetailDefList.ToArray();
                            invoiceDefList.Add(invoiceDef1);
                        }
                    }

                    List<InvoiceM> list = ((IEnumerable<InvoiceM>)invoiceMList).Where<InvoiceM>((Func<InvoiceM, bool>)(x => ((IEnumerable<InvoiceD>)x.InvoiceDList).Count<InvoiceD>() == 0)).ToList<InvoiceM>();
                    if ((uint)list.Count > 0U)
                        str = Helper.SaveInvoice(token, conn, list.ToArray(), true);
                    HotSaleSenfoniAppServer.Senfoni.Token senfoniServiceToken = Helper.CreateSenfoniServiceToken(coBranchId2, coBranchId1, token.Username, conn);
                    if ((uint)invoiceDefList.Count > 0U)
                    {
                        ServiceResultOfBoolean serviceResultOfBoolean = generalSenfoniService.SaveInvoiceMulti(senfoniServiceToken, invoiceDefList.ToArray());
                        if (serviceResultOfBoolean.Result)
                            return string.Empty;
                        return serviceResultOfBoolean.Message;
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool IsInvoiceExists(InvoiceM invoiceM, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT INV.HSM_INVOICE_M_ID FROM HSMT_INVOICE_M INV WHERE INV.SOURCE_GUID = '{0}'", (object)invoiceM.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT INV.HSM_INVOICE_M_ID FROM HSMT_INVOICE_M INV WHERE INV.SOURCE_GUID = '{0}'", (object)invoiceM.SourceGuid), conn).ExecuteReader().Read();
        }

        private static bool IsOnlineInvoiceExists(InvoiceM invoiceM, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT INV.INVOICE_M_ID FROM PSMT_INVOICE_M INV WHERE INV.SOURCE_GUID = '{0}'", (object)invoiceM.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT INV.INVOICE_M_ID FROM PSMT_INVOICE_M INV WHERE INV.SOURCE_GUID = '{0}'", (object)invoiceM.SourceGuid), conn).ExecuteReader().Read();
        }

        private static List<SelectedParameters> GetCatCodeIdList(InvoiceM[] invoiceMList, IDbConnection conn)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            List<string> stringList = new List<string>();
            foreach (InvoiceM invoiceM in invoiceMList)
            {
                if (!string.IsNullOrEmpty(invoiceM.CatCode1) && stringList.Contains(invoiceM.CatCode1))
                    stringList.Add(invoiceM.CatCode1);
                if (!string.IsNullOrEmpty(invoiceM.CatCode2) && stringList.Contains(invoiceM.CatCode2))
                    stringList.Add(invoiceM.CatCode2);
            }
            if (stringList.Count == 0)
                return selectedParametersList;
            string inFilter = Helper._GenerateInFilter(stringList.ToArray(), " C.CAT_CODE ");

            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("select C.CAT_CODE_ID,C.CAT_CODE from GNLD_CATEGORY C WHERE {0}", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        internal static string SaveWaybillM(HotSaleServiceTables.Token token, IDbConnection conn, WaybillM[] waybillMList, bool dontConsiderSaveReal)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            string str1 = string.Empty;
            try
            {
                if (waybillMList.Length == 0)
                    return str1;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                EventLog.WriteEntry("Application", "İrsaliye", EventLogEntryType.Information, 10071);
                if (userParameters.IsSaveRealWaybill && !dontConsiderSaveReal)
                {
                    str1 = Helper.SaveOnlineWaybillM(token, conn, waybillMList);
                }
                else
                {
                    EventLog.WriteEntry("Application", "İrsaliye ara tablo", EventLogEntryType.Information, 10072);
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                    EventLog.WriteEntry("Application", "İrsaliye parametreler", EventLogEntryType.Information, 10073);
                    List<SelectedParameters> paramsForWaybillM1 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "E");
                    List<SelectedParameters> paramsForWaybillM2 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "W");
                    List<SelectedParameters> paramsForWaybillM3 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "ID");
                    List<SelectedParameters> paramsForWaybillM4 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "U");
                    List<ItemTax> vatIdForWaybillM = Helper.GetVatIdForWaybillM(waybillMList, conn, "VAT");
                    List<SelectedParameters> reasonIdsFromWaybill = Helper.GetReasonIdsFromWaybill(waybillMList, conn);
                    EventLog.WriteEntry("Application", "İrsaliye listler", EventLogEntryType.Information, 10074);
                    foreach (WaybillM waybillM1 in waybillMList)
                    {
                        WaybillM waybillM = waybillM1;
                        EventLog.WriteEntry("Application", "İrsaliye: " + waybillM.DocNo, EventLogEntryType.Information, 10075);
                        if (!Helper.IsWaybillExists(waybillM, conn))
                        {
                            int num1 = !(waybillM.PurchaseSales == "Satis") ? Helper.GetCoBranchId(token, conn, userParameters, "DSWR") : Helper.GetCoBranchId(token, conn, userParameters, "DSW");
                            EventLog.WriteEntry("Application", "İrsaliye: " + waybillM.DocNo + " doctra", EventLogEntryType.Information, 10076);
                            SelectedParameters selectedParameters1 = paramsForWaybillM1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == waybillM.EntityCode)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters2 = paramsForWaybillM2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == waybillM.WhouseCode)).FirstOrDefault<SelectedParameters>();
                            oracleTransaction = conn.BeginTransaction();
                            string cmdText = @"begin
                                  INSERT INTO UYUMSOFT.HSMT_ITEM_M
                                    (DOC_NO, DOC_DATE, WHOUSE_ID, CO_ID,BRANCH_ID, SOURCE_APP, ENTITY_ID, SALES_PERSON_ID, DUE_DATE, CREATE_DATE,
                                     CREATE_USER_ID, LATITUDE, LONGITUDE, DOC_TRA_ID, IS_UPLOAD, DISC0_ID, DISC0_RATE, CAT_CODE1_ID, CAT_CODE2_ID,
                                     IS_DELETED, SOURCE_GUID, AMT, AMT_DISC_TOTAL, AMT_VAT, AMT_RECEIPT)
                                  VALUES
                                    (:DocNo, :DocDate, :WhouseId, :CoId, :BranchId, :SourceApp, :EntityId, :SalesPersonId, :DueDate, :CreateDate,
                                     :CreateUserId, :Latitude, :Longitude, :DocTraId, :IsUpload, :Disc0Id, :Disc0Rate, :CatCode1Id, :CatCode2Id,
                                     :IsDeleted, :SourceGuid, :Amt, :AmtDiscTotal, :AmtVat, :AmtReceipt)
                                  returning HSM_ITEM_M_ID into :HSM_ITEM_M_ID;
                                end;";
                            EventLog.WriteEntry("Application", "İrsaliye: " + cmdText, EventLogEntryType.Information, 10077);
                            command = conn.CreateCommand();
                            command.CommandText = cmdText;
                            command.Transaction = (IDbTransaction)oracleTransaction;
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "DocNo", (object)waybillM.DocNo));

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "DocDate", (object)waybillM.DocDate.ToLocalTime().Date)); // 07.04.2023

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "WhouseId", (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)ConvertToInt32((object)GNL.SourceApplication.İrsaliye)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "DueDate", (object)waybillM.DueDate.ToLocalTime().Date)); // 07.04.2023
                            command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)waybillM.DocDate.ToLocalTime())); // 07.04.2023

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)waybillM.Latitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)waybillM.Longitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DocTraId", (object)num1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "Disc0Id", (object)(waybillM.MasterDiscRate != Decimal.Zero ? userParameters.DiscId0 : 0)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Disc0Rate", (object)waybillM.MasterDiscRate));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode1Id", (object)catCodeId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode2Id", (object)catCodeId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsDeleted", (object)ConvertToInt32(waybillM.IsDeleted)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)waybillM.SourceGuid));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Amt", (object)waybillM.Amt));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "AmtDiscTotal", (object)waybillM.AmtDisc));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "AmtVat", (object)waybillM.AmtVat));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "AmtReceipt", (object)waybillM.AmtWaybill));
                            command.Parameters.Add(new OracleParameter("HSM_ITEM_M_ID", OracleDbType.Int32, ParameterDirection.Output));

                            //if (command.ExecuteNonQuery() != 1)
                            //    throw new Exception("Could not save items");
                            int a = command.ExecuteNonQuery();
                            int int32 = int.Parse((command.Parameters["HSM_ITEM_M_ID"] as OracleParameter).Value.ToString());
                            EventLog.WriteEntry("Application", "İrsaliye Id: " + int32.ToString(), EventLogEntryType.Information, 10078);
                            int num2 = 10;
                            int index = 0;
                            while (index < waybillM.WaybillDList.Length)
                            {
                                WaybillD detail = waybillM.WaybillDList[index];
                                EventLog.WriteEntry("Application", "İrsaliye detail", EventLogEntryType.Information, 10079);
                                SelectedParameters tmpItem = paramsForWaybillM3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters3 = paramsForWaybillM4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters4 = reasonIdsFromWaybill.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.NedenKod)).FirstOrDefault<SelectedParameters>();
                                ItemTax itemTax = vatIdForWaybillM.Cast<ItemTax>().Where<ItemTax>((Func<ItemTax, bool>)(x =>
                               {
                                   if (x.ItemId == tmpItem.Id && x.StartDate <= waybillM.DocDate)
                                       return x.EndDate >= waybillM.DocDate;
                                   return false;
                               })).FirstOrDefault<ItemTax>();
                                Decimal num3 = detail.Price * detail.Qty;
                                string str2 = string.Format("INSERT INTO UYUMSOFT.HSMT_ITEM_D (ITEM_ID, QTY , UNIT_ID, BRANCH_ID, CO_ID, LINE_NO, SOURCE_APP, WHOUSE_ID, UNIT_PRICE, AMT , DISC_RATE1, DISC_RATE2, TAX_RATE, HSM_ITEM_M_ID, VAT_ID, VAT_STATUS, DISC_ID1, DISC_ID2, QTY_PRM, REASON_ID, DUE_DAY, CAMPAIGN_ID, COST_CENTER_ID) \r\n                                                                                         values (:ItemId, :Qty, :UnitId, :BranchId, :CoId, :LineNo, :SourceApp, :WhouseId, :UnitPrice, :Amt, :DiscRate1, :DiscRate2, :TaxRate, :HsmItemMId  , :VatId, :VatStatus, :DiscId1, :DiscId2, :QtyPrm, :ReasonId, :DueDay, :CampaignId, :CostCenterId )");
                                EventLog.WriteEntry("Application", "İrsaliye detail sql : " + str2, EventLogEntryType.Information, 10080);
                                command.Dispose();
                                command = (IDbCommand)conn.CreateCommand();
                                command.CommandText = str2;
                                command.Transaction = (IDbTransaction)oracleTransaction;
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "ItemId", (object)(tmpItem == null ? 0 : tmpItem.Id)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Qty", (object)detail.Qty));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "UnitId", (object)(selectedParameters3 == null ? 0 : selectedParameters3.Id)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "LineNo", (object)num2));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)ConvertToInt32((object)GNL.SourceApplication.İrsaliye)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "WhouseId", (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "UnitPrice", (object)detail.Price));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Amt", (object)num3));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "DiscRate1", (object)detail.DiscRate1));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "DiscRate2", (object)detail.DiscRate2));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "TaxRate", (object)detail.SVatRate));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "HsmItemMId", (object)int32));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "VatId", (object)(itemTax == null ? 0 : itemTax.TaxId)));
                                if (!detail.VatStatus)
                                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "VatStatus", (object)ConvertToInt32((object)GNL.VatStatus.Hariç)));
                                else
                                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "VatStatus", (object)ConvertToInt32((object)GNL.VatStatus.Dahil)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DiscId1", (object)(detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DiscId2", (object)(detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "QtyPrm", (object)detail.QtyPrm));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "ReasonId", (object)(selectedParameters4 == null ? 0 : selectedParameters4.Id)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "DueDay", (object)detail.DueDay));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CampaignId", (object)detail.CampaignId));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CostCenterId", (object)costCenterId));
                                if (command.ExecuteNonQuery() != 1)
                                    throw new Exception("Could not save items");
                                ++index;
                                num2 += 10;
                            }
                            oracleTransaction.Commit();
                        }
                    }
                }
                return str1;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        private static object CreateParam(IDbCommand command, DbType dbType, ParameterDirection direction, string parameterName, object value)
        {
            IDataParameter parameter = (IDataParameter)command.CreateParameter();
            parameter.DbType = dbType;
            parameter.Direction = direction;
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            return (object)parameter;
        }

        private static bool IsWaybillExists(WaybillM waybillM, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT ITM.HSM_ITEM_M_ID FROM UYUMSOFT.HSMT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)waybillM.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT ITM.HSM_ITEM_M_ID FROM UYUMSOFT.HSMT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)waybillM.SourceGuid), conn).ExecuteReader().Read();
        }

        private static bool IsOnlineWaybillExists(WaybillM waybillM, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT ITM.ITEM_M_ID FROM UYUMSOFT.INVT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)waybillM.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT ITM.ITEM_M_ID FROM UYUMSOFT.INVT_ITEM_M ITM WHERE ITM.SOURCE_GUID = '{0}'", (object)waybillM.SourceGuid), conn).ExecuteReader().Read();
        }

        internal static string SaveOnlineWaybillM(HotSaleServiceTables.Token token, IDbConnection conn, WaybillM[] waybillMList)
        {
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            string str = string.Empty;
            try
            {
                if (waybillMList.Length == 0)
                    return string.Empty;
                GeneralSenfoniService generalSenfoniService = GetGeneralSenfoniWebService();
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (!userParameters.IsSaveRealWaybill)
                {
                    str = Helper.SaveWaybillM(token, conn, waybillMList, true);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    int coCurId = Helper.GetCoCurId(token, conn, coBranchId2);
                    int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                    List<SelectedParameters> paramsForWaybillM1 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "E");
                    List<SelectedParameters> paramsForWaybillM2 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "W");
                    List<SelectedParameters> paramsForWaybillM3 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "ID");
                    List<SelectedParameters> paramsForWaybillM4 = Helper.GetSelectedParamsForWaybillM(waybillMList, conn, "U");
                    List<ItemTax> vatIdForWaybillM = Helper.GetVatIdForWaybillM(waybillMList, conn, "VAT");
                    List<SelectedParameters> reasonIdsFromWaybill = Helper.GetReasonIdsFromWaybill(waybillMList, conn);
                    List<ItemDef> itemDefList = new List<ItemDef>();
                    foreach (WaybillM waybillM1 in ((IEnumerable<WaybillM>)waybillMList).Where<WaybillM>((Func<WaybillM, bool>)(x => (uint)((IEnumerable<WaybillD>)x.WaybillDList).Count<WaybillD>() > 0U)).ToList<WaybillM>())
                    {
                        WaybillM waybillM = waybillM1;
                        EventLog.WriteEntry("Application", "waybillM.DocNo : " + waybillM.DocNo + " waybillM.SourceGuid : " + waybillM.SourceGuid, EventLogEntryType.Information, 21122);
                        if (!Helper.IsOnlineWaybillExists(waybillM, conn))
                        {
                            ItemDef itemDef1 = new ItemDef();
                            int num1 = !(waybillM.PurchaseSales == "Satis") ? Helper.GetCoBranchId(token, conn, userParameters, "DSWR") : Helper.GetCoBranchId(token, conn, userParameters, "DSW");
                            SelectedParameters selectedParameters1 = paramsForWaybillM1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == waybillM.EntityCode)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters2 = paramsForWaybillM2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == waybillM.WhouseCode)).FirstOrDefault<SelectedParameters>();
                            itemDef1.DocNo = waybillM.DocNo;

                            // 27.02.2023
                            try
                            {
                                //itemDef1.Note2 = "Terminal Zamanı : " + waybillM1.DocDate.ToString("dd.MM.yyyy HH:mm");
                            }
                            catch { }

                            ItemDef itemDef2 = itemDef1;
                            DateTime dateTime = waybillM.DocDate.ToLocalTime(); // 07.04.2023

                            DateTime date1 = dateTime.Date;
                            itemDef2.ReceiptDate = date1;

                            itemDef1.WhouseId = selectedParameters2 == null ? 0 : selectedParameters2.Id;
                            itemDef1.CoId = coBranchId2;
                            itemDef1.BranchId = coBranchId1;
                            itemDef1.SourceApp = HotSaleSenfoniAppServer.Senfoni.SourceApplication.İrsaliye;
                            itemDef1.SourceApp2 = HotSaleSenfoniAppServer.Senfoni.SourceApplication.SicakSatis;
                            itemDef1.EntityId = selectedParameters1 == null ? 0 : selectedParameters1.Id;
                            itemDef1.SalesPersonId = coBranchId3;
                            itemDef1.SalesPersonCode = userParameters.SalesPersonCode;

                            ItemDef itemDef3 = itemDef1;
                            dateTime = waybillM.DueDate.ToLocalTime(); // 07.04.2023

                            // 27.02.2023
                            //DateTime date2 = dateTime.Date;
                            //itemDef3.DueDate = date2;

                            itemDef1.CreateUserId = coBranchId4;
                            itemDef1.Latitude = waybillM.Latitude;
                            itemDef1.Longitude = waybillM.Longitude;
                            itemDef1.DocTraId = num1;
                            itemDef1.Disc0Id = waybillM.MasterDiscRate != Decimal.Zero ? userParameters.DiscId0 : 0;
                            itemDef1.Disc0Rate = waybillM.MasterDiscRate;
                            itemDef1.CatCode1Id = catCodeId1;
                            itemDef1.CatCode1 = userParameters.CatCode1;
                            itemDef1.CatCode2Id = catCodeId2;
                            itemDef1.CatCode2 = userParameters.CatCode2;
                            itemDef1.SourceGuid = waybillM.SourceGuid;
                            itemDef1.CurId = coCurId;
                            itemDef1.CurTra = Decimal.One;
                            itemDef1.CurRateTypeId = 0;
                            if (!userParameters.IsMobileDnoteGibNo)
                            {
                                itemDef1.DespatchAdviceTypeCode = EDespatchAdviceType.MATBUDAN;
                                itemDef1.MatbuSerial = waybillM.DocNo.Substring(0, 3);
                                itemDef1.MatbuNo = waybillM.DocNo.Substring(3);
                                itemDef1.MatbuDate = date1;
                            }
                            else
                            {
                                itemDef1.DespatchAdviceTypeCode = EDespatchAdviceType.SEVK;
                                itemDef1.EDocNo = waybillM.DocNo;
                            }
                            List<ItemDetailDef> itemDetailDefList = new List<ItemDetailDef>();
                            int num2 = 10;
                            int index = 0;
                            while (index < waybillM.WaybillDList.Length)
                            {
                                WaybillD detail = waybillM.WaybillDList[index];
                                ItemDetailDef itemDetailDef = new ItemDetailDef();
                                SelectedParameters tmpItem = paramsForWaybillM3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters3 = paramsForWaybillM4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters4 = reasonIdsFromWaybill.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.NedenKod)).FirstOrDefault<SelectedParameters>();
                                ItemTax itemTax = vatIdForWaybillM.Cast<ItemTax>().Where<ItemTax>((Func<ItemTax, bool>)(x =>
                               {
                                   if (x.ItemId == tmpItem.Id && x.StartDate <= waybillM.DocDate)
                                       return x.EndDate >= waybillM.DocDate;
                                   return false;
                               })).FirstOrDefault<ItemTax>();
                                Decimal num3 = detail.Price * detail.Qty;
                                itemDetailDef.DcardId = tmpItem == null ? 0 : tmpItem.Id;
                                itemDetailDef.Qty = detail.Qty;
                                itemDetailDef.QtyPrm = detail.QtyPrm;
                                itemDetailDef.LineNo = num2;
                                itemDetailDef.WhouseId = selectedParameters2 == null ? 0 : selectedParameters2.Id;
                                itemDetailDef.UnitPrice = detail.Price;
                                itemDetailDef.UnitPriceTra = detail.Price;
                                itemDetailDef.Amt = num3;
                                itemDetailDef.Disc1Rate = detail.DiscRate1;
                                itemDetailDef.Disc1Id = detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1;
                                itemDetailDef.Disc2Rate = detail.DiscRate2;
                                itemDetailDef.Disc2Id = detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2;
                                itemDetailDef.VatRate = detail.SVatRate;
                                itemDetailDef.VatId = itemTax == null ? 0 : itemTax.TaxId;
                                if (!detail.VatStatus)
                                    itemDetailDef.VatStatus = HotSaleSenfoniAppServer.Senfoni.VatStatus.Hariç;
                                else
                                    itemDetailDef.VatStatus = HotSaleSenfoniAppServer.Senfoni.VatStatus.Dahil;
                                itemDetailDef.ReasonId = selectedParameters4 == null ? 0 : selectedParameters4.Id;
                                itemDetailDef.UnitId = selectedParameters3 == null ? 0 : selectedParameters3.Id;
                                itemDetailDef.DueDay = detail.DueDay;
                                itemDetailDef.CampaignId = detail.CampaignId;
                                itemDetailDef.CurTraId = coCurId;
                                itemDetailDef.CurRateTra = Decimal.One;
                                itemDetailDef.CurRateTypeId = 0;
                                itemDetailDef.CostCenterId = costCenterId;
                                itemDetailDefList.Add(itemDetailDef);
                                ++index;
                                num2 += 10;
                            }
                            itemDef1.Details = itemDetailDefList.ToArray();
                            itemDefList.Add(itemDef1);
                        }
                    }
                    List<WaybillM> list = ((IEnumerable<WaybillM>)waybillMList).Where<WaybillM>((Func<WaybillM, bool>)(x => ((IEnumerable<WaybillD>)x.WaybillDList).Count<WaybillD>() == 0)).ToList<WaybillM>();
                    if ((uint)list.Count > 0U)
                        str = Helper.SaveWaybillM(token, conn, list.ToArray(), true);
                    HotSaleSenfoniAppServer.Senfoni.Token senfoniServiceToken = Helper.CreateSenfoniServiceToken(coBranchId2, coBranchId1, token.Username, conn);
                    if ((uint)itemDefList.Count > 0U)
                    {
                        ServiceResultOfBoolean serviceResultOfBoolean = generalSenfoniService.SaveWaybillMulti(senfoniServiceToken, itemDefList.ToArray());
                        if (serviceResultOfBoolean.Result)
                            return string.Empty;
                        return serviceResultOfBoolean.Message;
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
        }

        private static List<SelectedParameters> GetReasonIdsFromWaybill(WaybillM[] waybillMList, IDbConnection conn)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            List<string> stringList = new List<string>();
            foreach (WaybillM waybillM in waybillMList)
            {
                List<string> list = ((IEnumerable<WaybillD>)waybillM.WaybillDList).Select<WaybillD, string>((Func<WaybillD, string>)(x => x.NedenKod)).ToList<string>();
                stringList.AddRange((IEnumerable<string>)list);
            }
            string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "t.reason_code");
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("select t.reason_id,t.reason_code from gnld_reason t where t.source_app = 1000\r\n                                                 AND {0}\r\n                                                 ", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1]),
                        SourceApp = "Irsaliye"
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static List<SelectedParameters> GetReasonIdsFromOrder(OrderM[] waybillMList, IDbConnection conn) // yeni eklendi
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            List<string> stringList = new List<string>();
            foreach (OrderM waybillM in waybillMList)
            {
                List<string> list = ((IEnumerable<OrderD>)waybillM.OrderDList).Select<OrderD, string>((Func<OrderD, string>)(x => x.ReasonCode)).ToList<string>();
                stringList.AddRange((IEnumerable<string>)list);
            }
            string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "t.reason_code");
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("select t.reason_id,t.reason_code from gnld_reason t where t.source_app in (1000,2,220,122)\r\n                                                 AND {0}\r\n                                                 ", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1]),
                        SourceApp = "SatisSiparis"
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static List<SelectedParameters> GetReasonIdsFromInvoice(InvoiceM[] invoiceMList, IDbConnection conn) // yeni eklendi 23.03.2022
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            List<string> stringList = new List<string>();
            foreach (InvoiceM waybillM in invoiceMList)
            {
                List<string> list = ((IEnumerable<InvoiceD>)waybillM.InvoiceDList).Select<InvoiceD, string>((Func<InvoiceD, string>)(x => x.ReasonCode)).ToList<string>();
                stringList.AddRange((IEnumerable<string>)list);
            }
            string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "t.reason_code");
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("select t.reason_id,t.reason_code from gnld_reason t where t.source_app in (2,102,122)\r\n                                                 AND {0}\r\n                                                 ", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1]),
                        SourceApp = "SatisFatgura"
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        internal static string SaveCreditPayment(HotSaleServiceTables.Token token, IDbConnection conn, Payment[] paymentList)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            string str1 = string.Empty;
            try
            {
                if (paymentList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (userParameters.IsSaveRealPayment)
                {
                    str1 = Helper.SaveRealCreditPayment(token, conn, paymentList);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "CB");
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    List<SelectedParameters> paramsForCashPayment = Helper.GetSelectedParamsForCashPayment(paymentList, conn, "E");
                    foreach (Payment payment1 in paymentList)
                    {
                        Payment payment = payment1;
                        if (!Helper.IsCashPaymentExists(payment, conn))
                        {
                            SelectedParameters selectedParameters = paramsForCashPayment.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == payment.EntityCode)).FirstOrDefault<SelectedParameters>();
                            oracleTransaction = conn.BeginTransaction();
                            string str2 = "INSERT INTO UYUMSOFT.HSMT_CASH_PAYMENT  (RECEIPT_TYPE_ID, DOC_DATE, TRA_TYPE_ID, ENTITY_ID, CASH_BOX_ID, BANK_ACC_ID, BRANCH_ID, CO_ID, DOC_NO, CREATE_DATE, CREATE_USER_ID, SALES_PERSON_ID, LATITUDE , LONGITUDE , AMT , IS_UPLOAD, CAT_CODE1_ID, CAT_CODE2_ID, SOURCE_GUID, IS_DELETED) \r\n values (:ReceiptTypeId , :DocDate, :TraTypeId , :EntityId, :CashBoxId , :BankAccId , :BranchId, :CoId, :DocNo, :CreateDate, :CreateUserId , :SalesPersonId , :Latitude, :Longitude, :Amt, :IsUpload, :CatCode1Id , :CatCode2Id , :SourceGuid, :IsDeleted)";
                            command = (IDbCommand)conn.CreateCommand();
                            command.CommandText = str2;
                            command.Transaction = (IDbTransaction)oracleTransaction;
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "ReceiptTypeId", (object)userParameters.CashReceiptTypeId));

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "DocDate", (object)payment.DocDate.Date.ToLocalTime())); // 07.04.2023

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "TraTypeId", (object)userParameters.TraTypeId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters == null ? 0 : selectedParameters.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CashBoxId", (object)coBranchId5));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BankAccId", (object)0));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "DocNo", (object)payment.DocNo));

                            command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)payment.DocDate.ToLocalTime())); // 07.04.2023

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)payment.Latitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)payment.Longitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Amt", (object)payment.Amt));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode1Id", (object)catCodeId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode2Id", (object)catCodeId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)payment.SourceGuid));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsDeleted", (object)0));
                            if (command.ExecuteNonQuery() != 1)
                                return "Kasa master kayıt olmadı";
                            oracleTransaction.Commit();
                        }
                    }
                }
                return str1;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        internal static string SaveCashPayment(HotSaleServiceTables.Token token, IDbConnection conn, Payment[] paymentList)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            string str1 = string.Empty;
            try
            {
                if (paymentList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (userParameters.IsSaveRealPayment)
                {
                    str1 = Helper.SaveRealCashPayment(token, conn, paymentList);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "CB");
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    List<SelectedParameters> paramsForCashPayment = Helper.GetSelectedParamsForCashPayment(paymentList, conn, "E");
                    foreach (Payment payment1 in paymentList)
                    {
                        Payment payment = payment1;
                        if (!Helper.IsCashPaymentExists(payment, conn))
                        {
                            SelectedParameters selectedParameters = paramsForCashPayment.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == payment.EntityCode)).FirstOrDefault<SelectedParameters>();
                            oracleTransaction = conn.BeginTransaction();
                            string str2 = "INSERT INTO UYUMSOFT.HSMT_CASH_PAYMENT  (RECEIPT_TYPE_ID, DOC_DATE, TRA_TYPE_ID, ENTITY_ID, CASH_BOX_ID, BANK_ACC_ID, BRANCH_ID, CO_ID, DOC_NO, CREATE_DATE, CREATE_USER_ID, SALES_PERSON_ID, LATITUDE , LONGITUDE , AMT , IS_UPLOAD, CAT_CODE1_ID, CAT_CODE2_ID, SOURCE_GUID, IS_DELETED) \r\n                                                                                    values (:ReceiptTypeId , :DocDate, :TraTypeId , :EntityId, :CashBoxId , :BankAccId , :BranchId, :CoId, :DocNo, :CreateDate, :CreateUserId , :SalesPersonId , :Latitude, :Longitude, :Amt, :IsUpload, :CatCode1Id , :CatCode2Id , :SourceGuid, :IsDeleted)";
                            command = (IDbCommand)conn.CreateCommand();
                            command.CommandText = str2;
                            command.Transaction = (IDbTransaction)oracleTransaction;
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "ReceiptTypeId", (object)userParameters.CashReceiptTypeId));

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "DocDate", (object)payment.DocDate.Date.ToLocalTime())); // 07.04.2023

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "TraTypeId", (object)userParameters.TraTypeId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters == null ? 0 : selectedParameters.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CashBoxId", (object)coBranchId5));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BankAccId", (object)0));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "DocNo", (object)payment.DocNo));

                            command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)payment.DocDate.ToLocalTime())); // 07.04.2023

                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)payment.Latitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)payment.Longitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Amt", (object)payment.Amt));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode1Id", (object)catCodeId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode2Id", (object)catCodeId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)payment.SourceGuid));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsDeleted", (object)0));
                            if (command.ExecuteNonQuery() != 1)
                                return "Kasa master kayıt olmadı";
                            oracleTransaction.Commit();
                        }
                    }
                }
                return str1;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        private static bool IsCashPaymentExists(Payment payment, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT 1 FROM HSMT_CASH_PAYMENT CSH WHERE CSH.SOURCE_GUID = '{0}'", (object)payment.SourceGuid);
            using (var dr =command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(/*string.Format("SELECT 1 FROM HSMT_CASH_PAYMENT CSH WHERE CSH.SOURCE_GUID = '{0}'", (object)payment.SourceGuid)*/, conn).ExecuteReader().Read();
        }

        private static int GetCatCodeId(IDbConnection conn, string catCode1)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            int num = 0;
            if (string.IsNullOrEmpty(catCode1))
                return 0;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("select C.CAT_CODE_ID from GNLD_CATEGORY C WHERE C.CAT_CODE = '{0}'", (object)catCode1);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    num = ConvertToInt32(oracleDataReader[0]);
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return num;
        }

        internal static string SaveCreditCardPayment(HotSaleServiceTables.Token token, IDbConnection conn, Payment[] paymentList)
        {
            //IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (paymentList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                List<SelectedParameters> paramsForCashPayment1 = Helper.GetSelectedParamsForCashPayment(paymentList, conn, "E");
                List<SelectedParameters> paramsForCashPayment2 = Helper.GetSelectedParamsForCashPayment(paymentList, conn, "BA");
                
                oracleTransaction = conn.BeginTransaction();
                foreach (Payment payment1 in paymentList)
                {
                    Payment payment = payment1;
                    if (!Helper.IsCashPaymentExists(payment, conn))
                    {
                        SelectedParameters selectedParameters1 = paramsForCashPayment1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == payment.EntityCode)).FirstOrDefault<SelectedParameters>();
                        SelectedParameters selectedParameters2 = paramsForCashPayment2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == payment.BankCode)).FirstOrDefault<SelectedParameters>();
                        using (IDbCommand command = conn.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO UYUMSOFT.HSMT_CASH_PAYMENT  (RECEIPT_TYPE_ID, DOC_DATE, TRA_TYPE_ID, ENTITY_ID, CASH_BOX_ID, BANK_ACC_ID, BRANCH_ID, CO_ID, DOC_NO, CREATE_DATE, CREATE_USER_ID, SALES_PERSON_ID, LATITUDE , LONGITUDE , AMT , IS_UPLOAD, CAT_CODE1_ID, CAT_CODE2_ID,SOURCE_GUID, IS_DELETED) \r\n                                                                                         values (:ReceiptTypeId , :DocDate, :TraTypeId , :EntityId, :CashBoxId , :BankAccId , :BranchId, :CoId, :DocNo, :CreateDate, :CreateUserId , :SalesPersonId , :Latitude, :Longitude, :Amt, :IsUpload, :CatCode1Id , :CatCode2Id ,:SourceGuid, 0)";
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "ReceiptTypeId", (object)userParameters.CashReceiptTypeId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "DocDate", (object)payment.DocDate.Date));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "TraTypeId", (object)userParameters.TraTypeId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CashBoxId", (object)0));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BankAccId", (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "DocNo", (object)payment.DocNo));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)payment.DocDate));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)payment.Latitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)payment.Longitude));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Amt", (object)payment.Amt));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode1Id", (object)catCodeId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CatCode2Id", (object)catCodeId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)payment.SourceGuid));
                            if (command.ExecuteNonQuery() != 1)
                            {
                                if (oracleTransaction != null)
                                    oracleTransaction.Rollback();

                                return "Banka master kaydolmadı";
                            }
                        }
                    }
                }
                oracleTransaction.Commit();
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                return "Banka master kaydolmadı(2)";
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                //if (command != null)
                //    command.Dispose();
            }
        }

        internal static string SaveEndOfDayItems(HotSaleServiceTables.Token token, IDbConnection conn, EndOfDayItems[] endOfDayItems)
        {
            IDbTransaction tr = null;
            try
            {
                if (endOfDayItems.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                CultureInfo cultureInfo2 = new CultureInfo("en-US");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "WV");
                tr = conn.BeginTransaction();
                IDbCommand dbCommand = conn.CreateCommand();
                dbCommand.CommandText = string.Format("DELETE FROM {0} T WHERE T.END_OF_DAY_M_ID IN ( SELECT M.END_OF_DAY_M_ID FROM {1} M WHERE TO_DATE(TO_CHAR(M.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') = TO_DATE('{2}','dd/MM/yyyy') AND M.SALES_PERSON_ID = {3}) ", 
                    (object)"UYUMSOFT.HSMT_END_OF_DAY_D", 
                    (object)"UYUMSOFT.HSMT_END_OF_DAY_M", 
                    (object)DateTime.Today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), 
                    (object)coBranchId3);
                //using (IDbCommand dbCommand = (IDbCommand)new OracleCommand(string.Format("DELETE FROM {0} T WHERE T.END_OF_DAY_M_ID IN ( SELECT M.END_OF_DAY_M_ID FROM {1} M WHERE TO_DATE(TO_CHAR(M.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') = TO_DATE('{2}','dd/MM/yyyy') AND M.SALES_PERSON_ID = {3}) ", (object)"UYUMSOFT.HSMT_END_OF_DAY_D", (object)"UYUMSOFT.HSMT_END_OF_DAY_M", (object)DateTime.Today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)coBranchId3), conn))
                {
                    dbCommand.Transaction = (IDbTransaction)tr;
                    dbCommand.ExecuteNonQuery();
                }
                dbCommand = conn.CreateCommand();
                dbCommand.CommandText = string.Format("DELETE FROM {0} M WHERE TO_DATE(TO_CHAR(M.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') = TO_DATE('{1}','dd/MM/yyyy') AND M.SALES_PERSON_ID = {2} ", (object)"UYUMSOFT.HSMT_END_OF_DAY_M", (object)DateTime.Today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)coBranchId3);
                //using (IDbCommand dbCommand = (IDbCommand)new OracleCommand(string.Format("DELETE FROM {0} M WHERE TO_DATE(TO_CHAR(M.DOC_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') = TO_DATE('{1}','dd/MM/yyyy') AND M.SALES_PERSON_ID = {2} ", (object)"UYUMSOFT.HSMT_END_OF_DAY_M", (object)DateTime.Today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), (object)coBranchId3), conn))
                {
                    dbCommand.Transaction = (IDbTransaction)tr;
                    dbCommand.ExecuteNonQuery();
                }
                if (endOfDayItems != null)
                {
                    List<SelectedParameters> paramsForEndOfDayM = Helper.GetSelectedParamsForEndOfDayM(endOfDayItems, conn, tr, "ID");
                    List<SelectedParameters> selectedParamsForUnit = Helper.GetSelectedParamsForUnit(((IEnumerable<EndOfDayItems>)endOfDayItems).Select<EndOfDayItems, string>((Func<EndOfDayItems, string>)(x => x.UnitCode)).ToArray<string>(), conn, tr);
                    string cmdText = string.Format("INSERT INTO {0} (CO_ID, BRANCH_ID, DOC_DATE, SALES_PERSON_ID, CREATE_USER_ID) values ('{1}',{2},TO_DATE('{3}','dd/MM/yyyy'),{4} ,{5})", 
                            (object)"UYUMSOFT.HSMT_END_OF_DAY_M", 
                            (object)coBranchId2, 
                            (object)coBranchId1, 
                            (object)DateTime.Today.ToString("dd/MM/yyyy", (IFormatProvider)cultureInfo), 
                            (object)coBranchId3, 
                            (object)coBranchId4);
                    int num = 0;
                    dbCommand = conn.CreateCommand();
                    dbCommand.CommandText = cmdText;

                    //using (IDbCommand dbCommand = (IDbCommand)new OracleCommand(cmdText, conn))
                    {
                        dbCommand.Transaction = (IDbTransaction)tr;

                        #region iptal
                        /*IDataParameter parameter = (IDataParameter)dbCommand.CreateParameter();
                        parameter.DbType = DbType.Int32;
                        parameter.Direction = ParameterDirection.Output;
                        parameter.ParameterName = "myOutputParameter";
                        dbCommand.Parameters.Add((object)parameter);
                        if (dbCommand.ExecuteNonQuery() != 1)
                            throw new Exception("Could not save items");
                        num = ConvertToInt32(parameter.Value);*/
                        #endregion
                        dbCommand.ExecuteNonQuery();
                        if (conn is OracleConnection)
                        {
                            dbCommand.CommandText = "select END_OF_DAY_M_ID_HSMT_END_OF_D.currval from dual";
                            num = ConvertToInt32(dbCommand.ExecuteScalar());
                        } else
                        {
                            dbCommand.CommandText = "select currval('hsmt_end_of_day_m_end_of_day_m_id_seq'::regclass)";
                            num = ConvertToInt32(dbCommand.ExecuteScalar());
                        }
                    }
                    for (int index = 0; index < endOfDayItems.Length; ++index)
                    {
                        EndOfDayItems endOfDayItem = endOfDayItems[index];
                        SelectedParameters selectedParameters1 = paramsForEndOfDayM.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == endOfDayItem.ItemCode)).FirstOrDefault<SelectedParameters>();
                        SelectedParameters selectedParameters2 = selectedParamsForUnit.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == endOfDayItem.UnitCode)).FirstOrDefault<SelectedParameters>();
                        dbCommand = conn.CreateCommand();

                        #region MyRegion
                        /*
                        dbCommand.CommandText = string.Format("INSERT INTO {0}  (END_OF_DAY_M_ID, CO_ID, BRANCH_ID, WHOUSE_ID, ITEM_ID,UNIT_ID, QTY, QTY_PRM)  \r\n  values ({1}            ,{2}   ,{3}       ,{4}       , {5}    ,{6}    , {7}, {8}    )", 
                            (object)"UYUMSOFT.HSMT_END_OF_DAY_D", 
                            (object)num, 
                            (object)coBranchId2, 
                            (object)coBranchId1, 
                            (object)coBranchId5, 
                            (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id), 
                            (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id), 
                            (object)endOfDayItem.Qty.ToString(cultureInfo2), 
                            (object)endOfDayItem.QtyPrm.ToString(cultureInfo2));
                        */
                        #endregion

                        // 30.12.2022 - 23.41
                        dbCommand.CommandText = string.Format("INSERT INTO HSMT_END_OF_DAY_D (END_OF_DAY_M_ID, CO_ID, BRANCH_ID, WHOUSE_ID, ITEM_ID,UNIT_ID, QTY, QTY_PRM) values ( :END_OF_DAY_M_ID, :CO_ID, :BRANCH_ID, :WHOUSE_ID, :ITEM_ID, :UNIT_ID, :QTY, :QTY_PRM)");
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Int32, ParameterDirection.Input, "END_OF_DAY_M_ID", (object)num));
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Int32, ParameterDirection.Input, "CO_ID", (object)coBranchId2));
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Int32, ParameterDirection.Input, "BRANCH_ID", (object)coBranchId1));
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Int32, ParameterDirection.Input, "WHOUSE_ID", (object)coBranchId5));
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Int32, ParameterDirection.Input, "ITEM_ID", (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id)));
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Int32, ParameterDirection.Input, "UNIT_ID", (object)(object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Decimal, ParameterDirection.Input, "QTY", (object)endOfDayItem.Qty));
                        dbCommand.Parameters.Add(Helper.CreateParam(dbCommand, DbType.Decimal, ParameterDirection.Input, "QTY_PRM", (object)endOfDayItem.QtyPrm));
    
                        //using (IDbCommand dbCommand = (IDbCommand)new OracleCommand(string.Format("INSERT INTO {0}  (END_OF_DAY_M_ID, CO_ID, BRANCH_ID, WHOUSE_ID, ITEM_ID,UNIT_ID, QTY, QTY_PRM)  \r\n                                                                            values ({1}            ,{2}   ,{3}       ,{4}       , {5}    ,{6}    , {7}, {8}    )", (object)"UYUMSOFT.HSMT_END_OF_DAY_D", (object)num, (object)coBranchId2, (object)coBranchId1, (object)coBranchId5, (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id), (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id), (object)endOfDayItem.Qty, (object)endOfDayItem.QtyPrm), conn))
                        {
                            dbCommand.Transaction = (IDbTransaction)tr;
                            if (dbCommand.ExecuteNonQuery() != 1)
                                throw new Exception("Could not save items");
                        }
                    }
                }
                if (tr != null)
                    tr.Commit();
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (tr != null)
                    tr.Rollback();
                throw ex;
            }
            finally
            {
                if (tr != null)
                    tr.Dispose();
            }
        }

        internal static string SaveActivities(HotSaleServiceTables.Token token, IDbConnection conn, Activity[] activityList)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (activityList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                List<SelectedParameters> paramsForActivity1 = Helper.GetSelectedParamsForActivity(activityList, conn, "E");
                List<SelectedParameters> paramsForActivity2 = Helper.GetSelectedParamsForActivity(activityList, conn, "C");
                foreach (Activity activity1 in activityList)
                {
                    Activity activity = activity1;
                    if (!Helper.IsActivityExists(activity, conn))
                    {
                        SelectedParameters selectedParameters1 = paramsForActivity1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == activity.EntityCode)).FirstOrDefault<SelectedParameters>();
                        SelectedParameters selectedParameters2 = paramsForActivity2.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == activity.Category1Code));
                        SelectedParameters selectedParameters3 = paramsForActivity2.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == activity.Category2Code));
                        SelectedParameters selectedParameters4 = paramsForActivity2.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == activity.Category3Code));
                        oracleTransaction = conn.BeginTransaction();
                        command = conn.CreateCommand();

                        command.CommandText = @"begin INSERT INTO HSMT_HSM_ACTIVITY  (CREATE_USER_ID, CREATE_DATE, CO_ID, BRANCH_ID, ENTITY_ID, TOPIC , NOTE ,
                                                                    CATEGORY_ID1, CATEGORY_ID2, CATEGORY_ID3, START_DATE, END_DATE, LATITUDE , LONGITUDE , IS_UPLOAD, SALES_PERSON_ID, SOURCE_GUID)
                                                                    values (:CreateUserId , :CreateDate, :CoId, :BranchId, :EntityId, :Topic, :Note, :CategoryId1, :CategoryId2, :CategoryId3, :StartDate, 
                                                                    :EndDate, :Latitude, :Longitude, :IsUpload, :SalesPersonId , :SourceGuid) returning HSM_ACTIVITY_ID into :myOutputParameter; end;";
                        command.Transaction = (IDbTransaction)oracleTransaction;
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)DateTime.Now));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Topic", (object)activity.Topic));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Note", (object)activity.Note));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CategoryId1", (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CategoryId2", (object)(selectedParameters3 == null ? 0 : selectedParameters3.Id)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CategoryId3", (object)(selectedParameters4 == null ? 0 : selectedParameters4.Id)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "StartDate", (object)activity.StartDate));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "EndDate", (object)activity.EndDate));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)activity.Latitude));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)activity.Longitude));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)activity.SourceGuid));
                        IDataParameter parameter1 = (IDataParameter)command.CreateParameter();
                        parameter1.DbType = DbType.Int32;
                        parameter1.Direction = ParameterDirection.Output;
                        parameter1.ParameterName = "myOutputParameter";
                        command.Parameters.Add((object)parameter1);
                        int a = command.ExecuteNonQuery();
                        int int32 = ConvertToInt32(parameter1.Value);
                        if (activity.DetailList == null)
                        {
                            throw new Exception("Could not find detail.");
                        }
                        for (int index = 0; index < activity.DetailList.Length; ++index)
                        {
                            ActivityDetail detail = activity.DetailList[index];
                            IDbCommand dbCommand = conn.CreateCommand();
                            dbCommand.CommandText = "INSERT INTO HSMT_HSM_ACTIVITY_DETAIL(CREATE_USER_ID, CREATE_DATE, HSM_ACTIVITY_ID, IMAGE_FILE)\r\n                                                                    VALUES(:UserId,:CreateDate,:HsmActivityId,:ImageFile)";
                            dbCommand.Transaction = (IDbTransaction)oracleTransaction;
                            IDataParameter parameter2 = (IDataParameter)command.CreateParameter();
                            parameter2.DbType = DbType.Int32;
                            parameter2.Direction = ParameterDirection.Input;
                            parameter2.ParameterName = "UserId";
                            parameter2.Value = (object)coBranchId4;
                            IDataParameter parameter3 = (IDataParameter)command.CreateParameter();
                            parameter3.DbType = DbType.Date;
                            parameter3.Direction = ParameterDirection.Input;
                            parameter3.ParameterName = "CreateDate";
                            parameter3.Value = (object)new DateTime();
                            IDataParameter parameter4 = (IDataParameter)command.CreateParameter();
                            parameter4.DbType = DbType.Int32;
                            parameter4.Direction = ParameterDirection.Input;
                            parameter4.ParameterName = "HsmActivityId";
                            parameter4.Value = (object)int32;
                            IDataParameter dataParameter = (IDataParameter)new OracleParameter("ImageFile", OracleDbType.Blob, (object)detail.ImageFile, ParameterDirection.Input);
                            dbCommand.Parameters.Add((object)parameter2);
                            dbCommand.Parameters.Add((object)parameter3);
                            dbCommand.Parameters.Add((object)parameter4);
                            dbCommand.Parameters.Add((object)dataParameter);
                            if (dbCommand.ExecuteNonQuery() != 1)
                                throw new Exception("Could not save detail");
                        }
                        oracleTransaction.Commit();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        private static bool IsActivityExists(Activity activity, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT ACT.HSM_ACTIVITY_ID FROM HSMT_HSM_ACTIVITY ACT WHERE ACT.SOURCE_GUID = '{0}'", (object)activity.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT ACT.HSM_ACTIVITY_ID FROM HSMT_HSM_ACTIVITY ACT WHERE ACT.SOURCE_GUID = '{0}'", (object)activity.SourceGuid), conn).ExecuteReader().Read();
        }

        internal static string SaveEntityCards(HotSaleServiceTables.Token token, IDbConnection conn, EntityCard[] entityCardList)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (entityCardList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                oracleTransaction = conn.BeginTransaction();
                command = conn.CreateCommand();
                command.Transaction = (IDbTransaction)oracleTransaction;
                foreach (EntityCard entityCard in entityCardList)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO HSMD_HSM_ENTITY  (CREATE_USER_ID, CREATE_DATE, CO_ID, BRANCH_ID, ENTITY_NAME, ENTITY_SHORT_NAME, TAX_OFFICE, TAX_NO, ADDRESS1 , ADDRESS2 , ADDRESS3 , SHIPPING_ADDRESS, TOWN_NAME, CITY_NAME, COUNTRY_NAME, TEL1 , TEL2 , FAX , PRICE_GROUP_CODE, NOTE , LATITUDE , LONGITUDE )\r\n                                                                 values (:CreateUserId ,:CreateDate , :CoId, :BranchId, :EntityName, :EntityShortName , :TaxOffice, :TaxNo, :Address1, :Address2, :Address3, :ShippingAddress, :TownName, :CityName, :CountryName, :Tel1, :Tel2, :Fax, :PriceGroupCode , :Note, :Latitude, :Longitude)";
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId3));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)DateTime.Now));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "EntityName", (object)entityCard.EntityName));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "EntityShortName", (object)entityCard.EntityShortName));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "TaxOffice", (object)entityCard.TaxOffice));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "TaxNo", (object)entityCard.TaxNo));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Address1", (object)entityCard.Address1));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Address2", (object)entityCard.Address2));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Address3", (object)entityCard.Address3));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "ShippingAddress", (object)entityCard.ShippingAddress));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "TownName", (object)entityCard.TownName));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "CityName", (object)entityCard.CityName));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Tel1", (object)entityCard.Tel1));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Tel2", (object)entityCard.Tel2));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Fax", (object)entityCard.Fax));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "PriceGroupCode", (object)entityCard.PriceGroupCode));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Note", (object)entityCard.Note));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Latitude", (object)entityCard.Latitude.ToString()));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Longitude", (object)entityCard.Longitude.ToString()));
                    if (command.ExecuteNonQuery() != 1)
                        throw new Exception("Could not save items");
                }

                oracleTransaction.Commit();
            }
            catch (Exception ex)
            {
                // bu tablo kullanılmıyor artık!..
                //if (oracleTransaction != null)
                //    oracleTransaction.Rollback();
                //if (command != null)
                //    command.Dispose();
                //throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }

            return string.Empty;
        }

        internal static string SaveOrders(HotSaleServiceTables.Token token, IDbConnection conn, OrderM[] orderMList, bool dontConsiderSaveReal)
        {
            IDbCommand command1 = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            string str = string.Empty;
            try
            {
                EventLog.WriteEntry("Application", "Save order : ", EventLogEntryType.Information, 101);
                if (orderMList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    str = string.Format("Invalid Token. BranchCode: {0} UserName : {1} Password: {2}", (object)token.BranchCode, (object)token.Username, (object)token.Password);
                if (userParameters.IsSaveRealOrder && !dontConsiderSaveReal)
                {
                    EventLog.WriteEntry("Application", "Save order : ", EventLogEntryType.Information, 102);
                    str = Helper.SaveOnlineOrdersForEndOfDay(token, conn, orderMList);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "DOS");
                    Helper.GetCoBranchId(token, conn, userParameters, "DOSR");
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                    List<SelectedParameters> paramsForOrderSales1 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "E");
                    List<SelectedParameters> paramsForOrderSales2 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "W");
                    List<SelectedParameters> paramsForOrderSales3 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "ID");
                    List<string> source = new List<string>();
                    foreach (OrderM orderM in orderMList)
                        source.AddRange(((IEnumerable<OrderD>)orderM.OrderDList).Select<OrderD, string>((Func<OrderD, string>)(x => x.UnitCode)));
                    List<SelectedParameters> selectedParamsForUnit = Helper.GetSelectedParamsForUnit(source.Distinct<string>().ToArray<string>(), conn, null);
                    List<SelectedParameters> paramsForOrderSales4 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "LC");
                    List<ItemTax> vatIdForOrderSales = Helper.GetVatIdForOrderSales(orderMList, conn, "VAT");
                    foreach (OrderM orderM1 in orderMList)
                    {
                        OrderM orderM = orderM1;
                        if (!Helper.IsOrderExists(orderM, conn))
                        {
                            SelectedParameters selectedParameters1 = paramsForOrderSales1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.EntityCode)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters2 = paramsForOrderSales1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.EntityCode2)).FirstOrDefault<SelectedParameters>();
                            SelectedParameters selectedParameters3 = paramsForOrderSales4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.LoadingCard)).FirstOrDefault<SelectedParameters>();
                            oracleTransaction = conn.BeginTransaction();

                            command1 = conn.CreateCommand();
                            command1.CommandText = @"begin INSERT INTO UYUMSOFT.HSMT_ORDER_M (CREATE_USER_ID, CREATE_DATE, CO_ID, BRANCH_ID, ENTITY_ID, ENTITY_ID2, SALES_PERSON_ID, 
                                                                    DOC_TRA_ID, DOC_DATE, DUE_DATE, DOC_NO, AMT , AMT_DISC_TOTAL, AMT_VAT, AMT_RECEIPT, SOURCE_APP, LOADING_CARD_ID, 
                                                                    LATITUDE , LONGITUDE, IS_UPLOAD, DISC0_ID, DISC0_RATE, CAT_CODE1_ID, CAT_CODE2_ID,IS_DELETED,SOURCE_GUID)                                                                                values (:CreateUserId , :CreateDate, :CoId, :BranchId, :EntityId, :EntityId2, :SalesPersonId , :DocTraId , :DocDate, :DueDate, :DocNo, :Amt, :AmtDiscTotal  , :AmtVat, :AmtReceipt , :SourceApp, :LoadignCardId , 
                                                                    :Latitude, :Longitude, :IsUpload, :Disc0Id, :Disc0Rate, :CatCode1Id , :CatCode2Id ,:IsDeleted,:SourceGuid) returning HSM_ORDER_M_ID into :myOutputParameter; end;";
                            command1.Transaction = (IDbTransaction)oracleTransaction;
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId4));

                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)orderM.DocDate.ToLocalTime())); // 07.04.2023

                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters1 == null ? 0 : selectedParameters1.Id)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "EntityId2", (object)(selectedParameters2 == null ? 0 : selectedParameters2.Id)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DocTraId", (object)coBranchId5));
                            IDataParameterCollection parameters1 = command1.Parameters;
                            IDbCommand command2 = command1;
                            int num1 = 5;
                            int num2 = 1;

                            string parameterName1 = "DocDate";
                            DateTime dateTime = orderM.DocDate.ToLocalTime(); // 07.04.2023

                            // ISSUE: variable of a boxed type
                            DateTime date1 = dateTime.Date;

                            //__Boxed<DateTime> date1 = (System.ValueType)dateTime.Date;
                            object obj1 = Helper.CreateParam(command2, (DbType)num1, (ParameterDirection)num2, parameterName1, (object)date1);
                            parameters1.Add(obj1);
                            IDataParameterCollection parameters2 = command1.Parameters;
                            IDbCommand command3 = command1;
                            int num3 = 5;
                            int num4 = 1;

                            string parameterName2 = "DueDate";
                            dateTime = orderM.DueDate.ToLocalTime(); // 07.04.2023

                            // ISSUE: variable of a boxed type
                            DateTime date2 = dateTime.Date;
                            
                            //__Boxed<DateTime> date2 = (System.ValueType)dateTime.Date;
                            object obj2 = Helper.CreateParam(command3, (DbType)num3, (ParameterDirection)num4, parameterName2, (object)date2);
                            parameters2.Add(obj2);
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.String, ParameterDirection.Input, "DocNo", (object)orderM.DocNo));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Amt", (object)orderM.Amt));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "AmtDiscTotal", (object)orderM.AmtDisc));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "AmtVat", (object)orderM.AmtVat));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "AmtReceipt", (object)orderM.AmtOrder));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)ConvertToInt32((object)GNL.SourceApplication.SatışSiparişi)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "LoadignCardId", (object)(selectedParameters3 == null ? 0 : selectedParameters3.Id)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)orderM.Latitude));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)orderM.Longitude));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "IsUpload", (object)0));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "Disc0Id", (object)(orderM.MasterDiscRate != Decimal.Zero ? userParameters.DiscId0 : 0)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Disc0Rate", (object)orderM.MasterDiscRate));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CatCode1Id", (object)catCodeId1));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CatCode2Id", (object)catCodeId2));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "IsDeleted", (object)ConvertToInt32(orderM.IsDeleted)));
                            command1.Parameters.Add(Helper.CreateParam(command1, DbType.String, ParameterDirection.Input, "SourceGuid", (object)orderM.SourceGuid));
                            IDataParameter parameter = (IDataParameter)command1.CreateParameter();
                            parameter.DbType = DbType.Int32;
                            parameter.Direction = ParameterDirection.Output;
                            parameter.ParameterName = "myOutputParameter";
                            command1.Parameters.Add((object)parameter);
                            int a = command1.ExecuteNonQuery();
                            int int32 = ConvertToInt32(parameter.Value);
                            int num5 = 10;
                            int index = 0;
                            while (index < orderM.OrderDList.Length)
                            {
                                OrderD detail = orderM.OrderDList[index];
                                SelectedParameters selectedParameters4 = paramsForOrderSales2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.WhouseCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters tmpItem = paramsForOrderSales3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters5 = selectedParamsForUnit.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();
                                ItemTax itemTax = vatIdForOrderSales.Cast<ItemTax>().Where<ItemTax>((Func<ItemTax, bool>)(x =>
                               {
                                   if (x.ItemId == tmpItem.Id && x.StartDate <= orderM.DocDate)
                                       return x.EndDate >= orderM.DocDate;
                                   return false;
                               })).FirstOrDefault<ItemTax>();
                                Decimal num6 = detail.Price * detail.Qty;
                                string cmdText = "INSERT INTO UYUMSOFT.HSMT_ORDER_D  (CO_ID, BRANCH_ID, LINE_NO, WHOUSE_ID, ITEM_ID, QTY , UNIT_ID, UNIT_PRICE, AMT , DISC_ID1, DISC_RATE1, DISC_ID2, DISC_RATE2, SOURCE_APP, HSM_ORDER_M_ID, VAT_ID, TAX_RATE, VAT_STATUS, QTY_PRM, DUE_DAY, CAMPAIGN_ID, COST_CENTER_ID) \r\n                                                                                      values (:CoId, :BranchId, :LineNo, :WhouseId, :ItemId, :Qty, :UnitId, :UnitPrice, :Amt, :DiscId1, :DiscRate1, :DiscId2, :DiscRate2, :SourceApp, :HsmOrderMId  , :VatId, :TaxRate, :VatStatus, :QtyPrm, :DueDay, :CampaignId, :CostCenterId)";
                                command1.Dispose();
                                command1 = conn.CreateCommand();
                                command1.CommandText = cmdText;
                                command1.Transaction = (IDbTransaction)oracleTransaction;
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "LineNo", (object)num5));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "WhouseId", (object)(selectedParameters4 == null ? 0 : selectedParameters4.Id)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "ItemId", (object)(tmpItem == null ? 0 : tmpItem.Id)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Qty", (object)detail.Qty));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "UnitId", (object)(selectedParameters5 == null ? 0 : selectedParameters5.Id)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "UnitPrice", (object)detail.Price));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "Amt", (object)num6));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DiscId1", (object)(detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "DiscRate1", (object)detail.DiscRate1));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DiscId2", (object)(detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "DiscRate2", (object)detail.DiscRate2));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "SourceApp", (object)ConvertToInt32((object)GNL.SourceApplication.SatışSiparişi)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "HsmOrderMId", (object)int32));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "VatId", (object)(itemTax == null ? 0 : itemTax.TaxId)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Decimal, ParameterDirection.Input, "TaxRate", (object)detail.VatRate));
                                if (!detail.VatStatus)
                                    command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "VatStatus", (object)ConvertToInt32((object)GNL.VatStatus.Hariç)));
                                else
                                    command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "VatStatus", (object)ConvertToInt32((object)GNL.VatStatus.Dahil)));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "QtyPrm", (object)detail.QtyPrm));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "DueDay", (object)detail.DueDay));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CampaignId", (object)detail.CampaignId));
                                command1.Parameters.Add(Helper.CreateParam(command1, DbType.Int32, ParameterDirection.Input, "CostCenterId", (object)costCenterId));
                                if (command1.ExecuteNonQuery() != 1)
                                    return "Sipariş detay kaydolmadı";
                                ++index;
                                num5 += 10;
                            }
                            oracleTransaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                EventLog.WriteEntry("Application", "Save order : " + ex.Message, EventLogEntryType.Error, 10106);
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command1 != null)
                    command1.Dispose();
            }
            return str;
        }

        private static bool IsOrderExists(OrderM orderM, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT 1 FROM HSMT_ORDER_M T WHERE T.SOURCE_GUID = '{0}'", (object)orderM.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT 1 FROM HSMT_ORDER_M T WHERE T.SOURCE_GUID = '{0}'", (object)orderM.SourceGuid), conn).ExecuteReader().Read();
        }

        internal static string SaveOnlineOrders(HotSaleServiceTables.Token token, IDbConnection conn, OrderM[] orderMList)
        {
            string str = string.Empty;
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            List<OrderM> orderMList1 = new List<OrderM>();
            try
            {
                if (orderMList.Length == 0)
                    return string.Empty;
                GeneralSenfoniService generalSenfoniService = GetGeneralSenfoniWebService();
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                List<ServiceOrderM> serviceOrderMList = new List<ServiceOrderM>();
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                if (!userParameters.IsSaveRealOrder)
                {
                    str = Helper.SaveOrders(token, conn, orderMList, false);
                }
                else
                {
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "DOS");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "DOSR");
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                    List<SelectedParameters> paramsForOrderSales1 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "E");
                    List<SelectedParameters> paramsForOrderSales2 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "W");
                    List<SelectedParameters> paramsForOrderSales3 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "ID");

                    List<string> source = new List<string>();
                    List<string> source2 = new List<string>();

                    foreach (OrderM orderM in orderMList)
                    {
                        source.AddRange(((IEnumerable<OrderD>)orderM.OrderDList).Select<OrderD, string>((Func<OrderD, string>)(x => x.UnitCode)));
                        source2.AddRange(((IEnumerable<OrderD>)orderM.OrderDList).Select<OrderD, string>((Func<OrderD, string>)(x => x.ReasonCode)));
                    }

                    List<SelectedParameters> selectedParamsForUnit = Helper.GetSelectedParamsForUnit(source.Distinct<string>().ToArray<string>(), conn, null);

                    // 11.04.2022
                    List<SelectedParameters> selectedParamsForReason = Helper.GetSelectedParamsForReason(source2.Distinct<string>().ToArray<string>(), conn, null);

                    List<SelectedParameters> paramsForOrderSales4 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "LC");
                    List<ItemTax> vatIdForOrderSales = Helper.GetVatIdForOrderSales(orderMList, conn, "VAT");
                    foreach (OrderM orderM1 in orderMList)
                    {
                        OrderM orderM = orderM1;
                        if (!Helper.IsRealOrderExists(orderM, conn))
                        {
                            SelectedParameters selectedParameters1 = paramsForOrderSales1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.EntityCode)).FirstOrDefault<SelectedParameters>();
                            paramsForOrderSales1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.EntityCode2)).FirstOrDefault<SelectedParameters>();
                            paramsForOrderSales4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.LoadingCard)).FirstOrDefault<SelectedParameters>();
                            ServiceOrderM serviceOrderM1 = new ServiceOrderM();
                            serviceOrderM1.BranchId = coBranchId1;
                            serviceOrderM1.CoId = coBranchId2;
                            ServiceOrderM serviceOrderM2 = serviceOrderM1;
                            DateTime docDate = orderM.DocDate;
                            int year = docDate.Year;
                            docDate = orderM.DocDate;
                            int month = docDate.Month;
                            docDate = orderM.DocDate;
                            int day = docDate.Day;
                            DateTime dateTime = new DateTime(year, month, day);
                            serviceOrderM2.DocDate = dateTime;
                            serviceOrderM1.EntityId = selectedParameters1 == null ? 0 : selectedParameters1.Id;
                            serviceOrderM1.SalesPersonId = coBranchId3;
                            if (orderM.PurchaseSales == "Satis" || string.IsNullOrEmpty(orderM.PurchaseSales))
                            {
                                serviceOrderM1.DocTraId = coBranchId4;
                                serviceOrderM1.SourceApp = ConvertToInt32((object)GNL.SourceApplication.SatışSiparişi);
                            }
                            else
                            {
                                serviceOrderM1.DocTraId = coBranchId5;
                                serviceOrderM1.SourceApp = ConvertToInt32((object)GNL.SourceApplication.SatınalmaSiparişi);
                            }
                            serviceOrderM1.DueDate = orderM.DueDate.Date;
                            serviceOrderM1.DocNo = orderM.DocNo;
                            serviceOrderM1.Amt = orderM.Amt;
                            serviceOrderM1.AmtDisc = orderM.AmtDisc;
                            serviceOrderM1.AmtVat = orderM.AmtVat;
                            serviceOrderM1.AmtOrder = orderM.AmtOrder;
                            serviceOrderM1.EntityDocNo = orderM.EntityDocNo;
                            serviceOrderM1.SourceApp2 = ConvertToInt32((object)GNL.SourceApplication.SicakSatis);
                            serviceOrderM1.Latitude = orderM.Latitude;
                            serviceOrderM1.Longitude = orderM.Longitude;
                            serviceOrderM1.Disc0Id = orderM.MasterDiscRate != Decimal.Zero ? userParameters.DiscId0 : 0;
                            serviceOrderM1.Disc0Rate = orderM.MasterDiscRate;
                            serviceOrderM1.DiscCalcType0 = HotSaleSenfoniAppServer.Senfoni.DiscTypes.Oran;
                            serviceOrderM1.Id = orderM.ERPOrderMId;
                            serviceOrderM1.CatCode1Id = catCodeId1;
                            serviceOrderM1.CatCode2Id = catCodeId2;
                            serviceOrderM1.SourceGuid = orderM.SourceGuid;
                            int num1 = 10;
                            List<ServiceOrderD> serviceOrderDList = new List<ServiceOrderD>();
                            int index = 0;
                            while (index < orderM.OrderDList.Length)
                            {
                                OrderD detail = orderM.OrderDList[index];
                                ServiceOrderD serviceOrderD = new ServiceOrderD();
                                SelectedParameters selectedParameters2 = paramsForOrderSales2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.WhouseCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters tmpItem = paramsForOrderSales3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                                SelectedParameters selectedParameters3 = selectedParamsForUnit.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();

                                // 11.04.2022
                                SelectedParameters selectedParameters4 = selectedParamsForReason.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ReasonCode)).FirstOrDefault<SelectedParameters>();

                                ItemTax itemTax = vatIdForOrderSales.Cast<ItemTax>().Where<ItemTax>((Func<ItemTax, bool>)(x =>
                               {
                                   if (x.ItemId == tmpItem.Id && x.StartDate <= orderM.DocDate)
                                       return x.EndDate >= orderM.DocDate;
                                   return false;
                               })).FirstOrDefault<ItemTax>();
                                Decimal num2 = detail.Price * detail.Qty;
                                serviceOrderD.Id = detail.ERPOrderDId;
                                serviceOrderD.CoId = coBranchId2;
                                serviceOrderD.BranchId = coBranchId1;
                                serviceOrderD.LineNo = num1;
                                serviceOrderD.WhouseId = selectedParameters2 == null ? 0 : selectedParameters2.Id;
                                serviceOrderD.ItemId = tmpItem == null ? 0 : tmpItem.Id;
                                serviceOrderD.Qty = detail.Qty;
                                serviceOrderD.QtyPrm = detail.QtyPrm;
                                serviceOrderD.UnitId = selectedParameters3 == null ? 0 : selectedParameters3.Id;
                                serviceOrderD.UnitPrice = detail.Price;
                                serviceOrderD.UnitPriceTra = detail.Price;
                                serviceOrderD.DiscCalcType1 = HotSaleSenfoniAppServer.Senfoni.DiscTypes.Oran;
                                serviceOrderD.Disc1Id = detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1;
                                serviceOrderD.DiscCalcType2 = HotSaleSenfoniAppServer.Senfoni.DiscTypes.Oran;
                                serviceOrderD.Disc1Rate = detail.DiscRate1;
                                serviceOrderD.Disc2Id = detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2;
                                serviceOrderD.Disc2Rate = detail.DiscRate2;
                                serviceOrderD.SourceApp = ConvertToInt32((object)GNL.SourceApplication.SatışSiparişi);
                                serviceOrderD.SourceApp2 = ConvertToInt32((object)GNL.SourceApplication.SicakSatis);
                                serviceOrderD.CampaignId = detail.CampaignId;
                                serviceOrderD.VatId = itemTax == null ? 0 : itemTax.TaxId;
                                serviceOrderD.VatRate = detail.VatRate;
                                serviceOrderD.VatStatus = detail.VatStatus ? ConvertToInt32((object)GNL.VatStatus.Dahil) : ConvertToInt32((object)GNL.VatStatus.Hariç);
                                serviceOrderD.Amt = num2;
                                serviceOrderD.AmtTra = num2;
                                serviceOrderD.CurRateTra = Decimal.One;
                                serviceOrderD.CostCenterId = costCenterId;

                                // 11.04.2022
                                serviceOrderD.ReasonId = selectedParameters4 != null ? selectedParameters4.Id : 0;

                                serviceOrderDList.Add(serviceOrderD);
                                ++index;
                                num1 += 10;
                            }
                            serviceOrderM1.ServiceOrderDList = serviceOrderDList.ToArray();
                            serviceOrderMList.Add(serviceOrderM1);
                        }
                    }
                }
                HotSaleSenfoniAppServer.Senfoni.Token senfoniServiceToken = Helper.CreateSenfoniServiceToken(coBranchId2, coBranchId1, token.Username, conn);
                EventLog.WriteEntry("Application", "count : " + serviceOrderMList.Count.ToString(), EventLogEntryType.Information, 2123);
                if ((uint)serviceOrderMList.Count > 0U)
                    str = generalSenfoniService.SaveOrderMulti(senfoniServiceToken, serviceOrderMList.ToArray());
                if ((uint)orderMList1.Count > 0U)
                    str = Helper.SaveOrders(token, conn, orderMList1.ToArray(), true);
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
            return str;
        }

        internal static string SaveOnlineOrdersForEndOfDay(HotSaleServiceTables.Token token, IDbConnection conn, OrderM[] orderMList)
        {
            string str = string.Empty;
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            List<OrderM> orderMList1 = new List<OrderM>();
            try
            {
                if (orderMList.Length == 0)
                    return string.Empty;
                GeneralSenfoniService generalSenfoniService = GetGeneralSenfoniWebService();
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (!userParameters.IsSaveRealOrder)
                {
                    str = Helper.SaveOrders(token, conn, orderMList, false);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "DOS");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "DOSR");
                    int catCodeId1 = Helper.GetCatCodeId(conn, userParameters.CatCode1);
                    int catCodeId2 = Helper.GetCatCodeId(conn, userParameters.CatCode2);
                    int costCenterId = Helper.GetCostCenterId(conn, userParameters.CostCenterCode);
                    List<SelectedParameters> paramsForOrderSales1 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "E");
                    List<SelectedParameters> paramsForOrderSales2 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "W");
                    List<SelectedParameters> paramsForOrderSales3 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "ID");

                    List<SelectedParameters> reasonIdsFromOrder = Helper.GetReasonIdsFromOrder(orderMList, conn); // 14.03.2022

                    List<string> source = new List<string>();
                    foreach (OrderM orderM in orderMList)
                        source.AddRange(((IEnumerable<OrderD>)orderM.OrderDList).Select<OrderD, string>((Func<OrderD, string>)(x => x.UnitCode)));
                    List<SelectedParameters> selectedParamsForUnit = Helper.GetSelectedParamsForUnit(source.Distinct<string>().ToArray<string>(), conn, null);
                    List<SelectedParameters> paramsForOrderSales4 = Helper.GetSelectedParamsForOrderSales(orderMList, conn, "LC");
                    List<ItemTax> vatIdForOrderSales = Helper.GetVatIdForOrderSales(orderMList, conn, "VAT");

                    List<ServiceOrderM> serviceOrderMList = new List<ServiceOrderM>();
                    foreach (OrderM orderM1 in orderMList)
                    {
                        OrderM orderM = orderM1;
                        EventLog.WriteEntry("Application", "serviceOrderM.DocNo : " + orderM.DocNo + " orderM.PurchaseSales : " + orderM.PurchaseSales, EventLogEntryType.Information, 1);
                        if (!Helper.IsRealOrderExists(orderM, conn))
                        {
                            SelectedParameters selectedParameters1 = paramsForOrderSales1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.EntityCode)).FirstOrDefault<SelectedParameters>();
                            paramsForOrderSales1.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.EntityCode2)).FirstOrDefault<SelectedParameters>();
                            paramsForOrderSales4.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == orderM.LoadingCard)).FirstOrDefault<SelectedParameters>();
                            ServiceOrderM serviceOrderM1 = new ServiceOrderM();
                            serviceOrderM1.BranchId = coBranchId1;
                            serviceOrderM1.CoId = coBranchId2;
                            ServiceOrderM serviceOrderM2 = serviceOrderM1;

                            DateTime dateTime1 = orderM.DocDate.ToLocalTime(); // 07.04.2023
                            int year = dateTime1.Year;

                            dateTime1 = orderM.DocDate.ToLocalTime(); // 07.04.2023
                            int month = dateTime1.Month;

                            dateTime1 = orderM.DocDate.ToLocalTime(); // 07.04.2023
                            int day = dateTime1.Day;

                            DateTime dateTime2 = new DateTime(year, month, day);
                            serviceOrderM2.DocDate = dateTime2;
                            serviceOrderM1.EntityId = selectedParameters1 == null ? 0 : selectedParameters1.Id;
                            serviceOrderM1.SalesPersonId = coBranchId3;
                            serviceOrderM1.EntityDocNo = orderM.EntityDocNo;
                            EventLog.WriteEntry("Application", "serviceOrderM.DocNo : " + serviceOrderM1.DocNo + " orderM.PurchaseSales : " + orderM.PurchaseSales, EventLogEntryType.Information, 1);
                            if (orderM.PurchaseSales == "Satis" || string.IsNullOrEmpty(orderM.PurchaseSales))
                            {
                                serviceOrderM1.DocTraId = coBranchId4;
                                serviceOrderM1.SourceApp = ConvertToInt32((object)GNL.SourceApplication.SatışSiparişi);
                            }
                            else
                            {
                                serviceOrderM1.DocTraId = coBranchId5;
                                serviceOrderM1.SourceApp = ConvertToInt32((object)GNL.SourceApplication.SatınalmaSiparişi);
                            }
                            EventLog.WriteEntry("Application", "docTraId : " + (object)coBranchId4 + " returnDocTraId : " + (object)coBranchId5 + " serviceOrderM.DocTraId : " + (object)serviceOrderM1.DocTraId, EventLogEntryType.Information, 2);
                            ServiceOrderM serviceOrderM3 = serviceOrderM1;

                            dateTime1 = orderM.DueDate.ToLocalTime(); // 07.04.2023
                            DateTime date = dateTime1.Date;
                            serviceOrderM3.DueDate = date;

                            serviceOrderM1.DocNo = orderM.DocNo;
                            serviceOrderM1.Amt = orderM.Amt;
                            serviceOrderM1.AmtDisc = orderM.AmtDisc;
                            serviceOrderM1.AmtVat = orderM.AmtVat;
                            serviceOrderM1.AmtOrder = orderM.AmtOrder;
                            serviceOrderM1.SourceApp2 = ConvertToInt32((object)GNL.SourceApplication.SicakSatis);
                            serviceOrderM1.Latitude = orderM.Latitude;
                            serviceOrderM1.Longitude = orderM.Longitude;
                            serviceOrderM1.Disc0Id = orderM.MasterDiscRate != Decimal.Zero ? userParameters.DiscId0 : 0;
                            serviceOrderM1.Disc0Rate = orderM.MasterDiscRate;
                            serviceOrderM1.DiscCalcType0 = HotSaleSenfoniAppServer.Senfoni.DiscTypes.Oran;
                            serviceOrderM1.Id = orderM.ERPOrderMId;
                            serviceOrderM1.CatCode1Id = catCodeId1;
                            serviceOrderM1.CatCode2Id = catCodeId2;
                            serviceOrderM1.SourceGuid = orderM.SourceGuid;
                            int num1 = 10;
                            List<ServiceOrderD> serviceOrderDList = new List<ServiceOrderD>();
                            if (orderM.OrderDList.Length != 0)
                            {
                                int index = 0;
                                while (index < orderM.OrderDList.Length)
                                {
                                    OrderD detail = orderM.OrderDList[index];
                                    ServiceOrderD serviceOrderD = new ServiceOrderD();
                                    SelectedParameters selectedParameters2 = paramsForOrderSales2.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.WhouseCode)).FirstOrDefault<SelectedParameters>();
                                    SelectedParameters tmpItem = paramsForOrderSales3.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ItemCode)).FirstOrDefault<SelectedParameters>();
                                    SelectedParameters selectedParameters3 = selectedParamsForUnit.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.UnitCode)).FirstOrDefault<SelectedParameters>();

                                    ItemTax itemTax = vatIdForOrderSales.Cast<ItemTax>().Where<ItemTax>((Func<ItemTax, bool>)(x =>
                                   {
                                       if (x.ItemId == tmpItem.Id && x.StartDate <= orderM.DocDate)
                                           return x.EndDate >= orderM.DocDate;
                                       return false;
                                   })).FirstOrDefault<ItemTax>();
                                    Decimal num2 = detail.Price * detail.Qty;
                                    serviceOrderD.Id = detail.ERPOrderDId;
                                    serviceOrderD.CoId = coBranchId2;
                                    serviceOrderD.BranchId = coBranchId1;
                                    serviceOrderD.LineNo = num1;
                                    serviceOrderD.WhouseId = selectedParameters2 == null ? 0 : selectedParameters2.Id;
                                    serviceOrderD.ItemId = tmpItem == null ? 0 : tmpItem.Id;
                                    serviceOrderD.Qty = detail.Qty;
                                    serviceOrderD.QtyPrm = detail.QtyPrm;
                                    serviceOrderD.UnitId = selectedParameters3 == null ? 0 : selectedParameters3.Id;
                                    serviceOrderD.UnitPrice = detail.Price;
                                    serviceOrderD.UnitPriceTra = detail.Price;
                                    serviceOrderD.DiscCalcType1 = HotSaleSenfoniAppServer.Senfoni.DiscTypes.Oran;
                                    serviceOrderD.Disc1Id = detail.DiscRate1 == Decimal.Zero ? 0 : userParameters.DiscId1;
                                    serviceOrderD.DiscCalcType2 = HotSaleSenfoniAppServer.Senfoni.DiscTypes.Oran;
                                    serviceOrderD.Disc1Rate = detail.DiscRate1;
                                    serviceOrderD.Disc2Id = detail.DiscRate2 == Decimal.Zero ? 0 : userParameters.DiscId2;
                                    serviceOrderD.Disc2Rate = detail.DiscRate2;
                                    serviceOrderD.SourceApp = ConvertToInt32((object)GNL.SourceApplication.SatışSiparişi);
                                    serviceOrderD.SourceApp2 = ConvertToInt32((object)GNL.SourceApplication.SicakSatis);
                                    serviceOrderD.CampaignId = detail.CampaignId;
                                    serviceOrderD.VatId = itemTax == null ? 0 : itemTax.TaxId;
                                    serviceOrderD.VatRate = detail.VatRate;
                                    serviceOrderD.VatStatus = detail.VatStatus ? ConvertToInt32((object)GNL.VatStatus.Dahil) : ConvertToInt32((object)GNL.VatStatus.Hariç);

                                    // 14.03.2022
                                    try
                                    {
                                        SelectedParameters selectedParameters4 = reasonIdsFromOrder.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == detail.ReasonCode)).FirstOrDefault<SelectedParameters>();
                                        serviceOrderD.ReasonId = selectedParameters4 == null ? 0 : selectedParameters4.Id;
                                    }
                                    catch { }

                                    serviceOrderD.Amt = num2;
                                    serviceOrderD.AmtTra = num2;
                                    serviceOrderD.CurRateTra = Decimal.One;
                                    serviceOrderD.CostCenterId = costCenterId;
                                    serviceOrderDList.Add(serviceOrderD);
                                    ++index;
                                    num1 += 10;
                                }
                                serviceOrderM1.ServiceOrderDList = serviceOrderDList.ToArray();
                                serviceOrderMList.Add(serviceOrderM1);
                            }
                        }
                    }
                    HotSaleSenfoniAppServer.Senfoni.Token senfoniServiceToken = Helper.CreateSenfoniServiceToken(coBranchId2, coBranchId1, token.Username, conn);
                    if ((uint)serviceOrderMList.Count > 0U)
                        str = generalSenfoniService.SaveOrderMulti(senfoniServiceToken, serviceOrderMList.ToArray());
                    if ((uint)orderMList1.Count > 0U)
                        str = Helper.SaveOrders(token, conn, orderMList1.ToArray(), true);
                }
                if ((uint)orderMList1.Count > 0U)
                    str = Helper.SaveOrders(token, conn, orderMList1.ToArray(), true);
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
            return str;
        }

        private static List<SelectedParameters> GetSelectedParamsForReason(
      string[] reasonCodes,
      IDbConnection conn,
      IDbTransaction tr)
        {
            List<SelectedParameters> selectedParamsForReason = new List<SelectedParameters>();
            string inFilter = Helper._GenerateInFilter(reasonCodes, "A.REASON_CODE");
            IDbCommand dbCommand = (IDbCommand)null;
            IDataReader dataReader = (IDataReader)null;
            try
            {
                dbCommand = conn.CreateCommand();
                dbCommand.CommandText = string.Format("select A.REASON_ID,A.REASON_CODE from gnld_reason A WHERE {0}", (object)inFilter);
                dbCommand.Transaction = (IDbTransaction)tr;
                dataReader = dbCommand.ExecuteReader();
                while (dataReader.Read())
                    selectedParamsForReason.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(dataReader[0]),
                        Code = ConvertToString(dataReader[1])
                    });
            }
            finally
            {
                dataReader?.Dispose();
                dbCommand?.Dispose();
            }
            return selectedParamsForReason;
        }


        private static bool IsRealOrderExists(OrderM orderM, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT 1 FROM PSMT_ORDER_M T WHERE T.SOURCE_GUID = '{0}'", (object)orderM.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
        }

        private static string SaveSurveyAnswerM(HotSaleServiceTables.Token token, IDbConnection conn, SurveyAnswerM[] surveyAnswerMList)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (surveyAnswerMList.Length == 0)
                    return string.Empty;
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                Helper.GetCoBranchId(token, conn, userParameters, "S");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                List<SelectedParameters> selectedParamsForEntity = Helper.GetSelectedParamsForEntity(((IEnumerable<SurveyAnswerM>)surveyAnswerMList).ToList<SurveyAnswerM>().Select<SurveyAnswerM, string>((Func<SurveyAnswerM, string>)(x => x.EntityCode)).Distinct<string>().Select<string, Entity>((Func<string, Entity>)(x => new Entity()
                {
                    EntityCode = x
                })).ToList<Entity>().ToArray(), conn);
                for (int i = 0; i < surveyAnswerMList.Length; i++)
                {
                    oracleTransaction = conn.BeginTransaction();
                    if (!Helper.IsSurveyMExists(surveyAnswerMList[i].SourceGuid, conn))
                    {
                        SelectedParameters selectedParameters = selectedParamsForEntity.FirstOrDefault<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == surveyAnswerMList[i].EntityCode));
                        if (selectedParameters == null)
                            throw new Exception(string.Format("Cari tanımı bulunamadı: Cari Kodu : {0}", (object)surveyAnswerMList[i].EntityCode));

                        command = conn.CreateCommand();

                        command.CommandText = "INSERT INTO HSMD_SURVEY_ANSWER_M (CREATE_USER_ID, CREATE_DATE, CO_ID,  BRANCH_ID,  ENTITY_ID,  NAME, SURVEY_DATE, SURVEY_TEMPLATE_M_ID, SOURCE_GUID)\r\n                                                                              VALUES (:CreateUserId , :CreateDate, :CoId, :BranchId , :EntityId , :Name, :SurveyDate, :SurveyTemplateMId  , :SourceGuid) returning SURVEY_ANSWER_M_ID into :myOutputParameter";
                        command.Transaction = (IDbTransaction)oracleTransaction;
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId3));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)DateTime.Now));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)selectedParameters.Id));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Name", (object)surveyAnswerMList[i].Name));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Date, ParameterDirection.Input, "SurveyDate", (object)ConvertToDateTime(surveyAnswerMList[i].SurveyDate)));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyTemplateMId", (object)surveyAnswerMList[i].SurveyTemplateMId));
                        command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "SourceGuid", (object)surveyAnswerMList[i].SourceGuid));
                        IDataParameter parameter1 = (IDataParameter)command.CreateParameter();
                        parameter1.DbType = DbType.Int32;
                        parameter1.Direction = ParameterDirection.Output;
                        parameter1.ParameterName = "myOutputParameter";
                        command.Parameters.Add((object)parameter1);
                        if (command.ExecuteNonQuery() != 1)
                            return "Anket master kaydolamadı.";
                        int int32_1 = ConvertToInt32(parameter1.Value);
                        for (int index1 = 0; index1 < surveyAnswerMList[i].SurveyAnswerDList.Length; ++index1)
                        {
                            SurveyAnswerD surveyAnswerD = surveyAnswerMList[i].SurveyAnswerDList[index1];
                            command = conn.CreateCommand();
                            command.CommandText = "INSERT INTO HSMD_SURVEY_ANSWER_D (CREATE_USER_ID, CREATE_DATE, SURVEY_ANSWER_M_ID, SURVEY_TEMPLATE_D_ID, SURVEY_TEMPLATE_M_ID, CO_ID, BRANCH_ID, DATE_FIELD, STRING_FIELD, INTEGER_FIELD, NUMBER_FIELD)\r\n                                                                                  VALUES (:CreateUserId , :CreateDate, :SurveyAnswerMId  , :SurveyTemplateDId  , :SurveyTemplateMId  , :CoId, :BranchId, :DateField, :StringField, :IntegerField, :NumberField) RETURNING SURVEY_ANSWER_D_ID INTO :myOutputParameter";
                            command.Transaction = (IDbTransaction)oracleTransaction;
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId3));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)DateTime.Now));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyAnswerMId", (object)int32_1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyTemplateDId", (object)surveyAnswerD.SurveyTemplateDId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyTemplateMId", (object)surveyAnswerMList[i].SurveyTemplateMId));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "DateField", (object)surveyAnswerD.DateField));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "StringField", (object)surveyAnswerD.StringField));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IntegerField", (object)surveyAnswerD.IntegerField));
                            command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "NumberField", (object)surveyAnswerD.NumberField));
                            IDataParameter parameter2 = (IDataParameter)command.CreateParameter();
                            parameter2.DbType = DbType.Int32;
                            parameter2.Direction = ParameterDirection.Output;
                            parameter2.ParameterName = "myOutputParameter";
                            command.Parameters.Add((object)parameter2);
                            if (command.ExecuteNonQuery() != 1)
                                return "Anket detay kaydolamadı.";
                            int int32_2 = ConvertToInt32(parameter2.Value);
                            for (int index2 = 0; index2 < surveyAnswerD.SurveyAnswerList.Length; ++index2)
                            {
                                SurveyAnswer surveyAnswer = surveyAnswerD.SurveyAnswerList[index2];
                                command = conn.CreateCommand();
                                command.CommandText = "INSERT INTO HSMD_SURVEY_ANSWER (CREATE_USER_ID, CREATE_DATE, SURVEY_ANSWER_D_ID, CO_ID, BRANCH_ID, IS_SELECTED, ANSWER , ANSWER_NO, SURVEY_ANSWER_M_ID, SURVEY_TEMPLATE_D_ID, SURVEY_TEMPLATE_M_ID)\r\n                                                                                         VALUES (:CreateUserId , :CreateDate, :SurveyAnswerDId  , :CoId, :BranchId, :IsSelected, :Answer, :AnswerNo, :SurveyAnswerMId  , :SurveyTemplateDId  , :SurveyTemplateMId  ) RETURNING SURVEY_ANSWER_ID INTO :myOutputParameter ";
                                command.Transaction = (IDbTransaction)oracleTransaction;
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CreateUserId", (object)coBranchId3));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CreateDate", (object)DateTime.Now));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyAnswerDId", (object)int32_2));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "IsSelected", (object)ConvertToInt32(surveyAnswer.IsSelected)));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Answer", (object)surveyAnswer.SelectAnswer));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "AnswerNo", (object)surveyAnswer.AnswerNo));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyAnswerMId", (object)int32_1));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyTemplateDId", (object)surveyAnswerD.SurveyTemplateDId));
                                command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SurveyTemplateMId", (object)surveyAnswerMList[i].SurveyTemplateMId));
                                IDataParameter parameter3 = (IDataParameter)command.CreateParameter();
                                parameter3.DbType = DbType.Int32;
                                parameter3.Direction = ParameterDirection.Output;
                                parameter3.ParameterName = "myOutputParameter";
                                command.Parameters.Add((object)parameter3);
                                if (command.ExecuteNonQuery() != 1)
                                    return "Anket detay cevap kaydolamadı.";
                                ConvertToInt32(parameter3.Value);
                            }
                        }
                    }
                    oracleTransaction.Commit();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        private static bool IsSurveyMExists(string sourceGuid, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT 1 FROM HSMD_SURVEY_ANSWER_M T WHERE T.SOURCE_GUID = '{0}'", (object)sourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
        }

        private static string SaveEntites(HotSaleServiceTables.Token token, IDbConnection conn, Entity[] entityList)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                if (entityList.Length == 0)
                    return string.Empty;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                Helper.GetCoBranchId(token, conn, userParameters, "B");
                Helper.GetCoBranchId(token, conn, userParameters, "C");
                Helper.GetCoBranchId(token, conn, userParameters, "S");
                Helper.GetCoBranchId(token, conn, userParameters, "U");
                Helper.GetSelectedParamsForEntity(entityList, conn);
                for (int index = 0; index < entityList.Length; ++index)
                {
                    oracleTransaction = conn.BeginTransaction();
                    string str1 = entityList[index].Latitude.ToString();
                    string str2 = entityList[index].Longitude.ToString();
                    if (str1.Length > 15)
                        str1.Substring(0, 15);
                    if (str2.Length > 15)
                        str2.Substring(0, 15);
                    command = conn.CreateCommand();
                    command.CommandText = "UPDATE FIND_ENTITY SET LATITUDE = :Latitude, LONGITUDE = :Longitude , RADIUS = :Radius WHERE ENTITY_CODE = :EntityCode";
                    command.Transaction = (IDbTransaction)oracleTransaction;

                    //command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)entityList[index].Latitude));
                    //command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)entityList[index].Longitude));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Latitude", (object)entityList[index].Latitude.ToString()));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "Longitude", (object)entityList[index].Longitude.ToString()));

                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "Radius", (object)entityList[index].Radius));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.String, ParameterDirection.Input, "EntityCode", (object)entityList[index].EntityCode));
                    if (command.ExecuteNonQuery() != 1)
                        throw new Exception("Could not save items");
                    oracleTransaction.Commit();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        private static void SaveEntityCheckins(HotSaleServiceTables.Token token, IDbConnection conn, EntityCheckInInfo[] entityCheckInInfo)
        {
            IDbCommand command = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            string empty = string.Empty;
            try
            {
                if (entityCheckInInfo.Length == 0)
                    return;
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                Helper.GetCoBranchId(token, conn, userParameters, "U");
                Helper.GetCoBranchId(token, conn, userParameters, "CB");
                List<SelectedParameters> selectedParamsForEntity = Helper.GetSelectedParamsForEntity(((IEnumerable<EntityCheckInInfo>)entityCheckInInfo).ToList<EntityCheckInInfo>().Select<EntityCheckInInfo, string>((Func<EntityCheckInInfo, string>)(x => x.EntityCode)).ToArray<string>(), conn);
                foreach (EntityCheckInInfo entityCheckInInfo1 in entityCheckInInfo)
                {
                    EntityCheckInInfo entityCheckinInfo = entityCheckInInfo1;
                    SelectedParameters selectedParameters = selectedParamsForEntity.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == entityCheckinInfo.EntityCode)).FirstOrDefault<SelectedParameters>();
                    oracleTransaction = conn.BeginTransaction();
                    string str = "INSERT INTO HSMT_ENTITY_CHECK_IN_INFO (CO_ID,BRANCH_ID,ENTITY_ID,SALES_PERSON_ID,CHECKIN_DATE,LATITUDE ,LONGITUDE )\r\n                                                                               values (:CoId,:BranchId,:EntityId,:SalesPersonId ,:CheckinDate,:Latitude,:Longitude) ";
                    command = (IDbCommand)conn.CreateCommand();
                    command.CommandText = str;
                    command.Transaction = (IDbTransaction)oracleTransaction;
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "CoId", (object)coBranchId2));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "BranchId", (object)coBranchId1));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "EntityId", (object)(selectedParameters == null ? 0 : selectedParameters.Id)));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Int32, ParameterDirection.Input, "SalesPersonId", (object)coBranchId3));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.DateTime, ParameterDirection.Input, "CheckinDate", (object)entityCheckinInfo.CheckinDate));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Latitude", (object)entityCheckinInfo.Latitude));
                    command.Parameters.Add(Helper.CreateParam(command, DbType.Decimal, ParameterDirection.Input, "Longitude", (object)entityCheckinInfo.Longitude));
                    if (command.ExecuteNonQuery() != 1)
                        throw new Exception("Cari Checkin kayıt olmadı");
                    oracleTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                throw ex;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (command != null)
                    command.Dispose();
            }
        }

        internal static HotSaleSenfoniAppServer.Senfoni.Token CreateSenfoniServiceToken(int coId, int branchId, string userName, IDbConnection conn)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            string empty = string.Empty;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT U.US_PASSWORD\r\n                                                 FROM USERS U WHERE U.US_USERNAME = '{0}'\r\n                                                 ", (object)userName);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    empty = ConvertToString(oracleDataReader[0]);
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            if (string.IsNullOrEmpty(empty))
                throw new Exception(string.Format("Erp kullanıcı tanımı bulunamadı. Kullanıcı Kodu : {0}", (object)userName));
            return new HotSaleSenfoniAppServer.Senfoni.Token()
            {
                CoId = coId,
                BranchId = branchId,
                UserName = userName,
                Password = empty
            };
        }

        internal static string SaveRealCreditPayment(HotSaleServiceTables.Token token, IDbConnection conn, Payment[] paymentList)
        {
            string str = string.Empty;
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                GeneralSenfoniService generalSenfoniService = GetGeneralSenfoniWebService();
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (!userParameters.IsSaveRealPayment)
                {
                    str = Helper.SaveCashPayment(token, conn, paymentList);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    
                    List<SelectedParameters> paramsForCashPayment = Helper.GetSelectedParamsForCashPayment(paymentList, conn, "E");
                    List<ServiceFinM> serviceFinMList = new List<ServiceFinM>();
                    foreach (Payment payment1 in paymentList)
                    {
                        int coBranchId5 = Helper.GetCCPaymentFindCardId(token, conn, payment1);
                        Payment payment = payment1;
                        if (!Helper.IsRealCashPaymentExists(payment, conn)) // kontrol yeri, mükerrerlik!...
                        {
                            SelectedParameters selectedParameters = paramsForCashPayment.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == payment.EntityCode)).FirstOrDefault<SelectedParameters>();
                            Payment payment2 = payment;
                            DateTime docDate = payment.DocDate.ToLocalTime(); // 07.04.2023
                            int year = docDate.Year;
                            docDate = payment.DocDate.ToLocalTime(); // 07.04.2023
                            int month = docDate.Month;
                            docDate = payment.DocDate.ToLocalTime(); // 07.04.2023
                            int day = docDate.Day;
                            DateTime dateTime = new DateTime(year, month, day);
                            payment2.DocDate = dateTime;
                            ServiceFinM finM = Helper.CreateFinM(
                                    payment, 
                                    userParameters, 
                                    coBranchId5, 
                                    coBranchId4, 
                                    coBranchId3, 
                                    coBranchId2, 
                                    coBranchId1, 
                                    selectedParameters == null ? 0 : selectedParameters.Id, token, conn, true); // 29.12.2022 de kredi kartı şey edildi
                            serviceFinMList.Add(finM);
                        }
                    }
                    HotSaleSenfoniAppServer.Senfoni.Token senfoniServiceToken = Helper.CreateSenfoniServiceToken(
                        coBranchId2, 
                        coBranchId1, 
                        token.Username, 
                        conn);
                    ServiceResultOfBoolean serviceResultOfBoolean;
                    if ((uint)serviceFinMList.Count > 0U)
                    {
                        serviceResultOfBoolean = generalSenfoniService.SaveFinanceMulti(senfoniServiceToken, serviceFinMList.ToArray());
                    }
                    else
                    {
                        serviceResultOfBoolean = new ServiceResultOfBoolean();
                        serviceResultOfBoolean.Result = true;
                    }
                    if (!serviceResultOfBoolean.Result)
                        throw new Exception(serviceResultOfBoolean.Message);
                }
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                return ex.Message;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
            return str;
        }

        internal static string SaveRealCashPayment(HotSaleServiceTables.Token token, IDbConnection conn, Payment[] paymentList)
        {
            string str = string.Empty;
            IDbCommand dbCommand = (IDbCommand)null;
            IDbTransaction oracleTransaction = null;
            try
            {
                GeneralSenfoniService generalSenfoniService = GetGeneralSenfoniWebService();
                CultureInfo cultureInfo = new CultureInfo("TR-tr");
                PdaUserParams userParameters = Helper.GetUserParameters(token, conn);
                if (userParameters == null)
                    throw new Exception("Invalid token");
                if (!userParameters.IsSaveRealPayment)
                {
                    str = Helper.SaveCashPayment(token, conn, paymentList);
                }
                else
                {
                    int coBranchId1 = Helper.GetCoBranchId(token, conn, userParameters, "B");
                    int coBranchId2 = Helper.GetCoBranchId(token, conn, userParameters, "C");
                    int coBranchId3 = Helper.GetCoBranchId(token, conn, userParameters, "S");
                    int coBranchId4 = Helper.GetCoBranchId(token, conn, userParameters, "U");
                    int coBranchId5 = Helper.GetCoBranchId(token, conn, userParameters, "CB");
                    List<SelectedParameters> paramsForCashPayment = Helper.GetSelectedParamsForCashPayment(paymentList, conn, "E");
                    List<ServiceFinM> serviceFinMList = new List<ServiceFinM>();
                    foreach (Payment payment1 in paymentList)
                    {
                        Payment payment = payment1;
                        if (!Helper.IsRealCashPaymentExists(payment, conn)) // kontrol yeri, mükerrerlik!...
                        {
                            SelectedParameters selectedParameters = paramsForCashPayment.Cast<SelectedParameters>().Where<SelectedParameters>((Func<SelectedParameters, bool>)(x => x.Code == payment.EntityCode)).FirstOrDefault<SelectedParameters>();
                            Payment payment2 = payment;

                            DateTime docDate = payment.DocDate.ToLocalTime(); // 07.04.2023
                            int year = docDate.Year;
                            docDate = payment.DocDate.ToLocalTime(); // 07.04.2023
                            int month = docDate.Month;
                            docDate = payment.DocDate.ToLocalTime(); // 07.04.2023
                            int day = docDate.Day;
                            DateTime dateTime = new DateTime(year, month, day);
                            payment2.DocDate = dateTime;
                            ServiceFinM finM = Helper.CreateFinM(payment, userParameters, coBranchId5, coBranchId4, coBranchId3, coBranchId2, coBranchId1, selectedParameters == null ? 0 : selectedParameters.Id, token, conn);
                            serviceFinMList.Add(finM);
                        }
                    }
                    HotSaleSenfoniAppServer.Senfoni.Token senfoniServiceToken = Helper.CreateSenfoniServiceToken(coBranchId2, coBranchId1, token.Username, conn);
                    ServiceResultOfBoolean serviceResultOfBoolean;
                    if ((uint)serviceFinMList.Count > 0U)
                    {
                        serviceResultOfBoolean = generalSenfoniService.SaveFinanceMulti(senfoniServiceToken, serviceFinMList.ToArray());
                    }
                    else
                    {
                        serviceResultOfBoolean = new ServiceResultOfBoolean();
                        serviceResultOfBoolean.Result = true;
                    }
                    if (!serviceResultOfBoolean.Result)
                        throw new Exception(serviceResultOfBoolean.Message);
                }
            }
            catch (Exception ex)
            {
                if (oracleTransaction != null)
                    oracleTransaction.Rollback();
                return ex.Message;
            }
            finally
            {
                if (oracleTransaction != null)
                    oracleTransaction.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
            return str;
        }

        private static bool IsRealCashPaymentExists(Payment payment, IDbConnection conn)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = string.Format("SELECT 1 FROM FINT_FIN_M T WHERE T.SOURCE_GUID = '{0}'", (object)payment.SourceGuid);
            using (var dr = command.ExecuteReader())
            {
                return dr.Read();
            }
            //return new OracleCommand(string.Format("SELECT 1 FROM FINT_FIN_M T WHERE T.SOURCE_GUID = '{0}'", (object)payment.SourceGuid), conn).ExecuteReader().Read();
        }

        internal static ServiceFinM CreateFinM(
            Payment payment,
            PdaUserParams param,
            int cashboxId,
            int userId,
            int salesPersonId,
            int coId,
            int branchId,
            int entityId,
            HotSaleServiceTables.Token token,
            IDbConnection conn, 
            bool IsCreditCard = false)
        {
            int catCodeId1 = Helper.GetCatCodeId(conn, param.CatCode1);
            int catCodeId2 = Helper.GetCatCodeId(conn, param.CatCode2);
            var tmp = new ServiceFinM()
            {
                DocDate = payment.DocDate.Date,
                ReceiptTypeId = IsCreditCard == false ? param.CashReceiptTypeId : param.BankReceiptTypeId, // 29.12.2022 tarihinden yapıldı.
                CardCode = IsCreditCard == false ? null : payment.BankCode,
                //CardType = HotSaleSenfoniAppServer.Senfoni.CardType.Kasa,
                CardId = cashboxId,
                //CardCode = "",
                //CardName = "",
                CoId = coId,
                BranchId = branchId,
                //CreateDate = DateTime.Now,
                CreateUserId = userId,
                DocNo = payment.DocNo,
                AmtDebit = payment.Amt,
                AmtCredit = payment.Amt,
                SourceApp = SourceApplication.Finans,
                SourceApp2 = SourceApplication.Finans,
                //Latitude = payment.Latitude,
                //Longitude = payment.Longitude,
                CatCode1Id = catCodeId1,
                CatCode2Id = catCodeId2,
                SourceGuid = payment.SourceGuid,
                ServiceFinDList = Helper.CreateFinDList(param, payment, coId, branchId, entityId, salesPersonId, token, conn)
            };
            return tmp;
        }

        internal static ServiceFinD[] CreateFinDList(PdaUserParams param, Payment payment, int coId, int branchId, int entityId, int salesPersonId, HotSaleServiceTables.Token token, IDbConnection conn)
        {
            return new List<ServiceFinD>()
      {
        Helper.GenerateFinD(param, payment, coId, branchId, entityId, salesPersonId, token, conn)
      }.ToArray();
        }

        internal static ServiceFinD GenerateFinD(PdaUserParams param, Payment payment, int coId, int branchId, int entityId, int salesPersonId, HotSaleServiceTables.Token token, IDbConnection conn)
        {
            return new ServiceFinD()
            {
                CurTraId = Helper.GetCoCurId(token, conn, coId),
                CurRateTypeId = 0,
                CurRateTra = 1,
                CoId = coId,
                BranchId = branchId,
                LineNo = 10,
                TraTypeId = param.TraTypeId,
                PlusMinus = HotSaleSenfoniAppServer.Senfoni.PlusMinus.Alacak,
                AmtTra = payment.Amt,
                Amt = payment.Amt,
                CostCenterId = 0,
                CardType = HotSaleSenfoniAppServer.Senfoni.CardType.Cari,
                FinDCardId = entityId,
                CardCode = "",
                CardName = "",
                EntityId = entityId,
                SalesPersonId = salesPersonId,
                SalesPersonCode = ""
            };
        }

        private static int GetCoCurId(HotSaleServiceTables.Token token, IDbConnection conn, int coId)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            int num = 0;
            try
            {
                oracleCommand = conn.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT C.CUR_ID FROM GNLD_COMPANY C WHERE C.CO_ID = {0}", (object)coId);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    num = ConvertToInt32(oracleDataReader[0]);
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return num;
        }

        private static List<SelectedParameters> GetSelectedParamsForUnit(string[] unitCodes, IDbConnection conn, IDbTransaction tr)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            string inFilter = Helper._GenerateInFilter(unitCodes, "A.UNIT_CODE");
            IDbCommand dbCommand = (IDbCommand)null;
            IDataReader dataReader = (IDataReader)null;
            try
            {
                dbCommand = conn.CreateCommand();
                dbCommand.CommandText = string.Format("SELECT\r\n                                                A.UNIT_ID,                                                \r\n                                                A.UNIT_CODE\r\n                                                FROM UYUMSOFT.INVD_UNIT A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                dbCommand.Transaction = (IDbTransaction)tr;
                dataReader = dbCommand.ExecuteReader();
                while (dataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(dataReader[0]),
                        Code = ConvertToString(dataReader[1])
                    });
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static int GetCCPaymentFindCardId(HotSaleServiceTables.Token token, IDbConnection connection, Payment p)
        {
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            int num = 0;
            try
            {
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT BANK_ACC_ID FROM UYUMSOFT.FIND_BANK_ACC A WHERE A.BANK_ACC_NO = '{0}'", (object)p.BankCode);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    num = ConvertToInt32(oracleDataReader[0]);
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return num;
        }

        private static int GetCoBranchId(HotSaleServiceTables.Token token, IDbConnection connection, PdaUserParams param, string type)
        {
            if (type == "C" || type == "B")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num1 = 0;
                int num2 = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.BRANCH_ID,                                                \r\n                                                A.CO_ID\r\n                                                FROM {0} A\r\n\r\n                                                 WHERE   A.BRANCH_CODE = '{1}'\r\n                                                 ", (object)"UYUMSOFT.GNLD_BRANCH", (object)token.BranchCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                    {
                        num1 = ConvertToInt32(oracleDataReader[0]);
                        num2 = ConvertToInt32(oracleDataReader[1]);
                    }
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                if (type == "B")
                    return num1;
                return num2;
            }
            if (type == "S")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.SALES_PERSON_ID                                                \r\n                                                FROM UYUMSOFT.FIND_SALES_PERSON A\r\n\r\n                                                 WHERE   A.SALES_PERSON_CODE = '{0}'\r\n                                                 ", (object)param.SalesPersonCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "U")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.US_ID FROM UYUMSOFT.USERS A WHERE   A.US_USERNAME  = '{0}'", (object)token.Username);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DIT")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}' AND A.SOURCE_APP = 210", (object)param.OneToOneDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DITC")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}' AND A.SOURCE_APP = 210", (object)param.ConsigneDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DITC2")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}' AND (A.SOURCE_APP = 1000 OR A.SOURCE_APP = 210)", (object)param.ConsigneDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DSI")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}' AND A.SOURCE_APP = 2", (object)param.SalesInvoiceDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DSIR")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}' AND A.SOURCE_APP = 2", (object)param.SalesReturnInvoiceDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DSW")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}' AND A.SOURCE_APP = 1000", (object)param.SalesWaybillDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DSWR")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE A.DOC_TRA_CODE  = '{0}' AND A.SOURCE_APP = 1000", (object)param.SalesReturnWaybillDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DOS")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID  FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}' AND A.SOURCE_APP = 122", (object)param.OrderSaleDocTraCode, 122);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "DOSR")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.DOC_TRA_ID FROM UYUMSOFT.GNLD_DOC_TRA A WHERE   A.DOC_TRA_CODE  = '{0}'  AND A.SOURCE_APP = 102", // 
                        (object)param.SalesOrderReturnDocTraCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            if (type == "CB")
            {
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                int num = 0;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.CASH_BOX_ID FROM UYUMSOFT.FIND_CASH_BOX A WHERE   A.CASH_BOX_CODE  = '{0}'", (object)param.CashBoxCode);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        num = ConvertToInt32(oracleDataReader[0]);
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return num;
            }
            IDbCommand oracleCommand1 = null;
            IDataReader oracleDataReader1 = null;
            int num3 = 0;
            try
            {
                oracleCommand1 = connection.CreateCommand();
                oracleCommand1.CommandText = string.Format("SELECT\r\n                                                A.WHOUSE_ID                                                \r\n                                                FROM UYUMSOFT.INVD_WHOUSE A\r\n                                                 WHERE   A.WHOUSE_CODE  = '{0}'\r\n                                                 ", (object)param.VehicleWhouseCode);
                oracleDataReader1 = oracleCommand1.ExecuteReader();
                while (oracleDataReader1.Read())
                    num3 = ConvertToInt32(oracleDataReader1[0]);
            }
            finally
            {
                if (oracleDataReader1 != null)
                    oracleDataReader1.Dispose();
                if (oracleCommand1 != null)
                    oracleCommand1.Dispose();
            }
            return num3;
        }

        private static List<SelectedParameters> GetSelectedParamsForOneToOne(OneToOneM[] oneToOneList, IDbConnection connection, string type)
        {
            if (type == "E")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (OneToOneM oneToOne in oneToOneList)
                {
                    if (!stringList.Contains(oneToOne.EntityCode))
                        stringList.Add(oneToOne.EntityCode);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ENTITY_CODE");
                IDbCommand dbCommand = (IDbCommand)null;
                IDataReader dataReader = (IDataReader)null;
                try
                {
                    dbCommand = connection.CreateCommand();
                    dbCommand.CommandText = string.Format("SELECT\r\n                                                A.ENTITY_ID,                                                \r\n                                                A.ENTITY_CODE\r\n                                                FROM UYUMSOFT.FIND_ENTITY A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                    dataReader = dbCommand.ExecuteReader();
                    while (dataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(dataReader[0]),
                            Code = ConvertToString(dataReader[1])
                        });
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Dispose();
                    if (dbCommand != null)
                        dbCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "W")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (OneToOneM oneToOne in oneToOneList)
                {
                    if (!stringList.Contains(oneToOne.WhouseCodeOut))
                        stringList.Add(oneToOne.WhouseCodeOut);
                    if (!stringList.Contains(oneToOne.WhouseCodeIn))
                        stringList.Add(oneToOne.WhouseCodeIn);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.WHOUSE_CODE");
                IDbCommand dbCommand = (IDbCommand)null;
                IDataReader dataReader = (IDataReader)null;
                try
                {

                    dbCommand = connection.CreateCommand();
                    dbCommand.CommandText = string.Format("SELECT\r\n                                                A.WHOUSE_ID,                                                \r\n                                                A.WHOUSE_CODE\r\n                                                FROM UYUMSOFT.INVD_WHOUSE A\r\n                                                 WHERE  {0}\r\n                                                 ", (object)inFilter);
                    dataReader = dbCommand.ExecuteReader();
                    while (dataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(dataReader[0]),
                            Code = ConvertToString(dataReader[1])
                        });
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Dispose();
                    if (dbCommand != null)
                        dbCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "ID")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (OneToOneM oneToOne in oneToOneList)
                {
                    for (int index = 0; index < oneToOne.OneToOneDetailList.Length; ++index)
                    {
                        if (!stringList.Contains(oneToOne.OneToOneDetailList[index].ItemCode))
                            stringList.Add(oneToOne.OneToOneDetailList[index].ItemCode);
                    }
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ITEM_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.ITEM_ID,                                                \r\n                                                A.ITEM_CODE\r\n                                                FROM UYUMSOFT.INVD_ITEM A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            List<SelectedParameters> selectedParametersList1 = new List<SelectedParameters>();
            List<string> stringList1 = new List<string>();
            foreach (OneToOneM oneToOne in oneToOneList)
            {
                for (int index = 0; index < oneToOne.OneToOneDetailList.Length; ++index)
                {
                    if (!stringList1.Contains(oneToOne.OneToOneDetailList[index].UnitCode))
                        stringList1.Add(oneToOne.OneToOneDetailList[index].UnitCode);
                }
            }
            string inFilter1 = Helper._GenerateInFilter(stringList1.ToArray(), "A.UNIT_CODE");
            IDbCommand oracleCommand1 = null;
            IDataReader oracleDataReader1 = null;
            try
            {
                oracleCommand1 = connection.CreateCommand();
                oracleCommand1.CommandText = string.Format("SELECT\r\n                                                A.UNIT_ID,                                                \r\n                                                A.UNIT_CODE\r\n                                                FROM UYUMSOFT.INVD_UNIT A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter1);
                oracleDataReader1 = oracleCommand1.ExecuteReader();
                while (oracleDataReader1.Read())
                    selectedParametersList1.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader1[0]),
                        Code = ConvertToString(oracleDataReader1[1])
                    });
            }
            finally
            {
                if (oracleDataReader1 != null)
                    oracleDataReader1.Dispose();
                if (oracleCommand1 != null)
                    oracleCommand1.Dispose();
            }
            return selectedParametersList1;
        }

        private static List<SelectedParameters> GetSelectedParamsForInvoiceM(InvoiceM[] invoiceMList, IDbConnection connection, string type)
        {
            if (type == "E")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (InvoiceM invoiceM in invoiceMList)
                {
                    if (!stringList.Contains(invoiceM.EntityCode))
                        stringList.Add(invoiceM.EntityCode);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ENTITY_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.ENTITY_ID,                                                \r\n                                                A.ENTITY_CODE\r\n                                                FROM UYUMSOFT.FIND_ENTITY A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "W")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (InvoiceM invoiceM in invoiceMList)
                {
                    if (!stringList.Contains(invoiceM.WhouseCode))
                        stringList.Add(invoiceM.WhouseCode);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.WHOUSE_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.WHOUSE_ID,                                                \r\n                                                A.WHOUSE_CODE\r\n                                                FROM UYUMSOFT.INVD_WHOUSE A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "ID")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (InvoiceM invoiceM in invoiceMList)
                {
                    for (int index = 0; index < invoiceM.InvoiceDList.Length; ++index)
                    {
                        if (!stringList.Contains(invoiceM.InvoiceDList[index].ItemCode))
                            stringList.Add(invoiceM.InvoiceDList[index].ItemCode);
                    }
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ITEM_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.ITEM_ID,                                                \r\n                                                A.ITEM_CODE\r\n                                                FROM UYUMSOFT.INVD_ITEM A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                catch(Exception ex)
                {
                    string err = ex.Message;
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            List<SelectedParameters> selectedParametersList1 = new List<SelectedParameters>();
            List<string> stringList1 = new List<string>();
            foreach (InvoiceM invoiceM in invoiceMList)
            {
                for (int index = 0; index < invoiceM.InvoiceDList.Length; ++index)
                {
                    if (!stringList1.Contains(invoiceM.InvoiceDList[index].UnitCode))
                        stringList1.Add(invoiceM.InvoiceDList[index].UnitCode);
                }
            }
            string inFilter1 = Helper._GenerateInFilter(stringList1.ToArray(), "A.UNIT_CODE");
            IDbCommand oracleCommand1 = null;
            IDataReader oracleDataReader1 = null;
            try
            {
                oracleCommand1 = connection.CreateCommand();
                oracleCommand1.CommandText = string.Format("SELECT\r\n                                                A.UNIT_ID,                                                \r\n                                                A.UNIT_CODE\r\n                                                FROM UYUMSOFT.INVD_UNIT A\r\n                                                 WHERE  {0}\r\n                                                 ", (object)inFilter1);
                oracleDataReader1 = oracleCommand1.ExecuteReader();
                while (oracleDataReader1.Read())
                    selectedParametersList1.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader1[0]),
                        Code = ConvertToString(oracleDataReader1[1])
                    });
            }
            finally
            {
                if (oracleDataReader1 != null)
                    oracleDataReader1.Dispose();
                if (oracleCommand1 != null)
                    oracleCommand1.Dispose();
            }
            return selectedParametersList1;
        }

        private static List<ItemTax> GetVatIdForInvoiceM(InvoiceM[] invoiceMList, IDbConnection connection, string type)
        {
            List<ItemTax> itemTaxList = new List<ItemTax>();
            int length = 0;
            foreach (InvoiceM invoiceM in invoiceMList)
                length += invoiceM.InvoiceDList.Length;
            int[] numArray = new int[length];
            int index1 = 0;
            List<int> intList = new List<int>();
            foreach (InvoiceM invoiceM in invoiceMList)
            {
                for (int index2 = 0; index2 < invoiceM.InvoiceDList.Length; ++index2)
                {
                    numArray[index1] = invoiceM.InvoiceDList[index2].VatRate;
                    if (!intList.Contains(invoiceM.InvoiceDList[index2].VatRate))
                        intList.Add(invoiceM.InvoiceDList[index2].VatRate);
                    ++index1;
                }
            }
            string inFilterVatCodes = Helper._GenerateInFilterVatCodes(intList.ToArray());
            if (string.IsNullOrEmpty(inFilterVatCodes))
                return itemTaxList;
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = connection.CreateCommand();
                string str = string.Format("SELECT\r\n                                            A.TAX_ID,                                                \r\n                                            T.TAX_RATE,\r\n                                            A.START_DATE,\r\n                                            A.END_DATE,\r\n                                            A.ITEM_ID\r\n                                            FROM UYUMSOFT.INVD_ITEM_TAX A\r\n                                                    INNER JOIN INVD_TAX T ON T.TAX_ID = A.TAX_ID\r\n                                                WHERE   T.TAX_RATE IN ({0})\r\n                                                ", (object)inFilterVatCodes);
                EventLog.WriteEntry("Application", str, EventLogEntryType.Information, 18123);
                oracleCommand.CommandText = str;
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    itemTaxList.Add(new ItemTax()
                    {
                        TaxId = ConvertToInt32(oracleDataReader[0]),
                        VatRate = ConvertToDecimal(oracleDataReader[1]),
                        StartDate = ConvertToDateTime(oracleDataReader[2]),
                        EndDate = ConvertToDateTime(oracleDataReader[3]),
                        ItemId = ConvertToInt32(oracleDataReader[4])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return itemTaxList;
        }

        private static List<SelectedParameters> GetSelectedParamsForWaybillM(WaybillM[] waybillMList, IDbConnection connection, string type)
        {
            if (type == "E")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (WaybillM waybillM in waybillMList)
                {
                    if (!stringList.Contains(waybillM.EntityCode))
                        stringList.Add(waybillM.EntityCode);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ENTITY_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.ENTITY_ID,                                                \r\n                                                A.ENTITY_CODE\r\n                                                FROM UYUMSOFT.FIND_ENTITY A\r\n                                                 WHERE  {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "W")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (WaybillM waybillM in waybillMList)
                {
                    if (!stringList.Contains(waybillM.WhouseCode))
                        stringList.Add(waybillM.WhouseCode);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.WHOUSE_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.WHOUSE_ID,                                                \r\n                                                A.WHOUSE_CODE\r\n                                                FROM UYUMSOFT.INVD_WHOUSE A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "ID")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (WaybillM waybillM in waybillMList)
                {
                    for (int index = 0; index < waybillM.WaybillDList.Length; ++index)
                    {
                        if (!stringList.Contains(waybillM.WaybillDList[index].ItemCode))
                            stringList.Add(waybillM.WaybillDList[index].ItemCode);
                    }
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ITEM_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.ITEM_ID,                                                \r\n                                                A.ITEM_CODE\r\n                                                FROM UYUMSOFT.INVD_ITEM A\r\n                                                 WHERE  {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            List<SelectedParameters> selectedParametersList1 = new List<SelectedParameters>();
            List<string> stringList1 = new List<string>();
            foreach (WaybillM waybillM in waybillMList)
            {
                for (int index = 0; index < waybillM.WaybillDList.Length; ++index)
                {
                    if (!stringList1.Contains(waybillM.WaybillDList[index].UnitCode))
                        stringList1.Add(waybillM.WaybillDList[index].UnitCode);
                }
            }
            string inFilter1 = Helper._GenerateInFilter(stringList1.ToArray(), "A.UNIT_CODE");
            IDbCommand oracleCommand1 = null;
            IDataReader oracleDataReader1 = null;
            try
            {
                oracleCommand1 = connection.CreateCommand();
                oracleCommand1.CommandText = string.Format("SELECT\r\n                                                A.UNIT_ID,                                                \r\n                                                A.UNIT_CODE\r\n                                                FROM UYUMSOFT.INVD_UNIT A\r\n                                                 WHERE  {0}\r\n                                                 ", (object)inFilter1);
                oracleDataReader1 = oracleCommand1.ExecuteReader();
                while (oracleDataReader1.Read())
                    selectedParametersList1.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader1[0]),
                        Code = ConvertToString(oracleDataReader1[1])
                    });
            }
            finally
            {
                if (oracleDataReader1 != null)
                    oracleDataReader1.Dispose();
                if (oracleCommand1 != null)
                    oracleCommand1.Dispose();
            }
            return selectedParametersList1;
        }

        private static List<ItemTax> GetVatIdForWaybillM(WaybillM[] waybillMList, IDbConnection connection, string type)
        {
            List<ItemTax> itemTaxList = new List<ItemTax>();
            List<int> intList = new List<int>();
            foreach (WaybillM waybillM in waybillMList)
            {
                for (int index = 0; index < waybillM.WaybillDList.Length; ++index)
                {
                    if (!intList.Contains((int)waybillM.WaybillDList[index].SVatRate))
                        intList.Add((int)waybillM.WaybillDList[index].SVatRate);
                }
            }
            string inFilterVatCodes = Helper._GenerateInFilterVatCodes(intList.ToArray());
            if (string.IsNullOrEmpty(inFilterVatCodes))
                return itemTaxList;
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT A.TAX_ID, T.TAX_RATE, A.START_DATE, A.END_DATE, A.ITEM_ID FROM UYUMSOFT.INVD_ITEM_TAX A INNER JOIN INVD_TAX T ON T.TAX_ID = A.TAX_ID WHERE T.TAX_RATE IN ({0}) ", (object)inFilterVatCodes);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    itemTaxList.Add(new ItemTax()
                    {
                        TaxId = ConvertToInt32(oracleDataReader[0]),
                        VatRate = ConvertToDecimal(oracleDataReader[1]),
                        StartDate = ConvertToDateTime(oracleDataReader[2]),
                        EndDate = ConvertToDateTime(oracleDataReader[3]),
                        ItemId = ConvertToInt32(oracleDataReader[4])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return itemTaxList;
        }

        private static List<SelectedParameters> GetSelectedParamsForCashPayment(Payment[] paymentList, IDbConnection connection, string type)
        {
            if (type == "E")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (Payment payment in paymentList)
                {
                    if (!stringList.Contains(payment.EntityCode))
                        stringList.Add(payment.EntityCode);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ENTITY_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.ENTITY_ID, A.ENTITY_CODE FROM UYUMSOFT.FIND_ENTITY A WHERE  {0} ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            List<SelectedParameters> selectedParametersList1 = new List<SelectedParameters>();
            List<string> stringList1 = new List<string>();
            foreach (Payment payment in paymentList)
            {
                if (!stringList1.Contains(payment.BankCode))
                    stringList1.Add(payment.BankCode);
            }
            string inFilter1 = Helper._GenerateInFilter(stringList1.ToArray(), "A.BANK_ACC_NO");
            IDbCommand oracleCommand1 = null;
            IDataReader oracleDataReader1 = null;
            try
            {
                oracleCommand1 = connection.CreateCommand();
                oracleCommand1.CommandText = string.Format("SELECT\r\n                                                A.BANK_ACC_ID,                                                \r\n                                                A.BANK_ACC_NO\r\n                                                FROM UYUMSOFT.FIND_BANK_ACC A\r\n                                                 WHERE  {0}\r\n                                                 ", (object)inFilter1);
                oracleDataReader1 = oracleCommand1.ExecuteReader();
                while (oracleDataReader1.Read())
                    selectedParametersList1.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader1[0]),
                        Code = ConvertToString(oracleDataReader1[1])
                    });
            }
            finally
            {
                if (oracleDataReader1 != null)
                    oracleDataReader1.Dispose();
                if (oracleCommand1 != null)
                    oracleCommand1.Dispose();
            }
            return selectedParametersList1;
        }

        private static List<SelectedParameters> GetSelectedParamsForEndOfDayM(EndOfDayItems[] endOfDayItems, IDbConnection connection, IDbTransaction tr, string type)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            List<string> stringList = new List<string>();
            foreach (EndOfDayItems endOfDayItem in endOfDayItems)
            {
                if (!stringList.Contains(endOfDayItem.ItemCode))
                    stringList.Add(endOfDayItem.ItemCode);
            }
            string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ITEM_CODE");
            IDbCommand dbCommand = (IDbCommand)null;
            IDataReader dataReader = (IDataReader)null;
            try
            {
                dbCommand = connection.CreateCommand();
                dbCommand.CommandText = string.Format("SELECT A.ITEM_ID, A.ITEM_CODE FROM UYUMSOFT.INVD_ITEM A WHERE   {0} ", (object)inFilter);
                dbCommand.Transaction = (IDbTransaction)tr;
                dataReader = dbCommand.ExecuteReader();
                while (dataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(dataReader[0]),
                        Code = ConvertToString(dataReader[1])
                    });
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Dispose();
                if (dbCommand != null)
                    dbCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static List<SelectedParameters> GetSelectedParamsForActivity(Activity[] activityList, IDbConnection connection, string type)
        {
            if (type == "E")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (Activity activity in activityList)
                {
                    if (!stringList.Contains(activity.EntityCode))
                        stringList.Add(activity.EntityCode);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ENTITY_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.ENTITY_ID, A.ENTITY_CODE FROM UYUMSOFT.FIND_ENTITY A WHERE  {0} ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (!(type == "C"))
                return (List<SelectedParameters>)null;
            List<SelectedParameters> selectedParametersList1 = new List<SelectedParameters>();
            List<string> stringList1 = new List<string>();
            foreach (Activity activity in activityList)
            {
                if (!stringList1.Contains(activity.Category1Code))
                    stringList1.Add(activity.Category1Code);
                if (!stringList1.Contains(activity.Category2Code))
                    stringList1.Add(activity.Category2Code);
                if (!stringList1.Contains(activity.Category3Code))
                    stringList1.Add(activity.Category3Code);
            }
            string inFilter1 = Helper._GenerateInFilter(stringList1.ToArray(), "A.CATEGORIES_CODE");
            IDbCommand oracleCommand1 = null;
            IDataReader oracleDataReader1 = null;
            try
            {
                oracleCommand1 = connection.CreateCommand();
                oracleCommand1.CommandText = string.Format("SELECT\r\n                                                A.CATEGORIES_ID,                                                \r\n                                                A.CATEGORIES_CODE\r\n                                                FROM UYUMSOFT.GNLD_CATEGORIES A\r\n                                                 WHERE {0}\r\n                                                 ", (object)inFilter1);
                oracleDataReader1 = oracleCommand1.ExecuteReader();
                while (oracleDataReader1.Read())
                    selectedParametersList1.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader1[0]),
                        Code = ConvertToString(oracleDataReader1[1])
                    });
            }
            finally
            {
                if (oracleDataReader1 != null)
                    oracleDataReader1.Dispose();
                if (oracleCommand1 != null)
                    oracleCommand1.Dispose();
            }
            return selectedParametersList1;
        }

        private static List<SelectedParameters> GetSelectedParamsForOrderSales(OrderM[] orderMList, IDbConnection connection, string type)
        {
            if (type == "E")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (OrderM orderM in orderMList)
                {
                    if (!stringList.Contains(orderM.EntityCode))
                        stringList.Add(orderM.EntityCode);
                    if (!stringList.Contains(orderM.EntityCode2))
                        stringList.Add(orderM.EntityCode2);
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ENTITY_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT A.ENTITY_ID, A.ENTITY_CODE FROM UYUMSOFT.FIND_ENTITY A WHERE  {0} ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "W")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (OrderM orderM in orderMList)
                {
                    for (int index = 0; index < orderM.OrderDList.Length; ++index)
                    {
                        if (!stringList.Contains(orderM.OrderDList[index].WhouseCode))
                            stringList.Add(orderM.OrderDList[index].WhouseCode);
                    }
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.WHOUSE_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.WHOUSE_ID,                                                \r\n                                                A.WHOUSE_CODE\r\n                                                FROM UYUMSOFT.INVD_WHOUSE A\r\n                                                 WHERE  {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "ID")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (OrderM orderM in orderMList)
                {
                    for (int index = 0; index < orderM.OrderDList.Length; ++index)
                    {
                        if (!stringList.Contains(orderM.OrderDList[index].ItemCode))
                            stringList.Add(orderM.OrderDList[index].ItemCode);
                    }
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ITEM_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT\r\n                                                A.ITEM_ID,                                                \r\n                                                A.ITEM_CODE\r\n                                                FROM UYUMSOFT.INVD_ITEM A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            if (type == "U")
            {
                List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
                List<string> stringList = new List<string>();
                foreach (OrderM orderM in orderMList)
                {
                    for (int index = 0; index < orderM.OrderDList.Length; ++index)
                    {
                        if (!stringList.Contains(orderM.OrderDList[index].UnitCode))
                            stringList.Add(orderM.OrderDList[index].UnitCode);
                    }
                }
                string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "T.UNIT_CODE");
                IDbCommand oracleCommand = null;
                IDataReader oracleDataReader = null;
                try
                {
                    oracleCommand = connection.CreateCommand();
                    oracleCommand.CommandText = string.Format("SELECT U.UNIT_ID, U.UNIT_CODE\r\n                                                  FROM INVD_ITEM T\r\n                                                 INNER JOIN INVD_UNIT U\r\n                                                    ON U.UNIT_ID = T.UNIT_ID\r\n                                                    WHERE  {0}", (object)inFilter);
                    oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                        selectedParametersList.Add(new SelectedParameters()
                        {
                            Id = ConvertToInt32(oracleDataReader[0]),
                            Code = ConvertToString(oracleDataReader[1])
                        });
                }
                finally
                {
                    if (oracleDataReader != null)
                        oracleDataReader.Dispose();
                    if (oracleCommand != null)
                        oracleCommand.Dispose();
                }
                return selectedParametersList;
            }
            List<SelectedParameters> selectedParametersList1 = new List<SelectedParameters>();
            List<string> stringList1 = new List<string>();
            foreach (OrderM orderM in orderMList)
            {
                if (!stringList1.Contains(orderM.LoadingCard))
                    stringList1.Add(orderM.LoadingCard);
            }
            string inFilter1 = Helper._GenerateInFilter(stringList1.ToArray(), "A.LOADING_CARD_CODE");
            IDbCommand oracleCommand1 = null;
            IDataReader oracleDataReader1 = null;
            try
            {
                oracleCommand1 = connection.CreateCommand();
                oracleCommand1.CommandText = string.Format("SELECT\r\n                                                A.LOADING_CARD_ID,                                                \r\n                                                A.LOADING_CARD_CODE\r\n                                                FROM UYUMSOFT.HSMD_LOADING_CARD A\r\n                                                 WHERE   {0}\r\n                                                 ", (object)inFilter1);
                oracleDataReader1 = oracleCommand1.ExecuteReader();
                while (oracleDataReader1.Read())
                    selectedParametersList1.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader1[0]),
                        Code = ConvertToString(oracleDataReader1[1])
                    });
            }
            finally
            {
                if (oracleDataReader1 != null)
                    oracleDataReader1.Dispose();
                if (oracleCommand1 != null)
                    oracleCommand1.Dispose();
            }
            return selectedParametersList1;
        }

        private static List<SelectedParameters> GetSelectedParamsForEntity(Entity[] entityList, IDbConnection connection)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            List<string> stringList = new List<string>();
            foreach (Entity entity in entityList)
            {
                if (!stringList.Contains(entity.EntityCode))
                    stringList.Add(entity.EntityCode);
            }
            string inFilter = Helper._GenerateInFilter(stringList.ToArray(), "A.ENTITY_CODE");
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT A.ENTITY_ID, A.ENTITY_CODE FROM UYUMSOFT.FIND_ENTITY A WHERE  {0} ", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static List<SelectedParameters> GetSelectedParamsForEntity(string[] entityCode, IDbConnection connection)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            string inFilter = Helper._GenerateInFilter(entityCode, "A.ENTITY_CODE");
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT A.ENTITY_ID, A.ENTITY_CODE FROM UYUMSOFT.FIND_ENTITY A WHERE  {0} ", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static List<SelectedParameters> GetSelectedParamsForLoadingCard(string[] loadingCardCode, IDbConnection connection)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            string inFilter = Helper._GenerateInFilter(loadingCardCode, "T.LOADING_CARD_CODE");
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("select T.LOADING_CARD_ID,T.LOADING_CARD_CODE from UYUMSOFT.HSMD_LOADING_CARD T WHERE  {0} ", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static List<SelectedParameters> GetSelectedParamsForDepositCard(string[] depositCode, IDbConnection connection)
        {
            List<SelectedParameters> selectedParametersList = new List<SelectedParameters>();
            string inFilter = Helper._GenerateInFilter(depositCode, "T.DEPOSIT_CARD_CODE");
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format(@"select T.DEPOSIT_CARD_ID,T.DEPOSIT_CARD_CODE from UYUMSOFT.HSMD_DEPOSIT_CARD T WHERE  {0} ", (object)inFilter);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    selectedParametersList.Add(new SelectedParameters()
                    {
                        Id = ConvertToInt32(oracleDataReader[0]),
                        Code = ConvertToString(oracleDataReader[1])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return selectedParametersList;
        }

        private static List<ItemTax> GetVatIdForOrderSales(OrderM[] orderMList, IDbConnection connection, string type)
        {
            List<ItemTax> itemTaxList = new List<ItemTax>();
            List<int> intList = new List<int>();
            foreach (OrderM orderM in orderMList)
            {
                for (int index = 0; index < orderM.OrderDList.Length; ++index)
                {
                    if (!intList.Contains((int)orderM.OrderDList[index].VatRate))
                        intList.Add((int)orderM.OrderDList[index].VatRate);
                }
            }
            string inFilterVatCodes = Helper._GenerateInFilterVatCodes(intList.ToArray());
            if (string.IsNullOrEmpty(inFilterVatCodes))
                return itemTaxList;
            IDbCommand oracleCommand = null;
            IDataReader oracleDataReader = null;
            try
            {
                oracleCommand = connection.CreateCommand();
                oracleCommand.CommandText = string.Format("SELECT\r\n                                            A.TAX_ID,                                                \r\n                                            T.TAX_RATE,\r\n                                            A.START_DATE,\r\n                                            A.END_DATE,\r\n                                            A.ITEM_ID\r\n                                            FROM UYUMSOFT.INVD_ITEM_TAX A\r\n                                                    INNER JOIN INVD_TAX T ON T.TAX_ID = A.TAX_ID\r\n                                                WHERE   T.TAX_RATE IN ({0})\r\n                                                ", (object)inFilterVatCodes);
                oracleDataReader = oracleCommand.ExecuteReader();
                while (oracleDataReader.Read())
                    itemTaxList.Add(new ItemTax()
                    {
                        TaxId = ConvertToInt32(oracleDataReader[0]),
                        VatRate = ConvertToDecimal(oracleDataReader[1]),
                        StartDate = ConvertToDateTime(oracleDataReader[2]),
                        EndDate = ConvertToDateTime(oracleDataReader[3]),
                        ItemId = ConvertToInt32(oracleDataReader[4])
                    });
            }
            finally
            {
                if (oracleDataReader != null)
                    oracleDataReader.Dispose();
                if (oracleCommand != null)
                    oracleCommand.Dispose();
            }
            return itemTaxList;
        }

        private static string _GenerateInFilter(string[] entityCodes, string preText)
        {
            string str = string.Empty;
            if (entityCodes.Length == 0)
                return preText + " IN ('')";
            int int32 = ConvertToInt32(Math.Ceiling((double)entityCodes.Length / 1000.0));
            int index1 = 0;
            for (int index2 = 1; index2 <= int32; ++index2)
            {
                for (; index1 < entityCodes.Length && index1 <= 1000 * index2; ++index1)
                {
                    if (index1 == 0)
                    {
                        str = preText + " IN ('" + entityCodes[index1] + "'";
                        if (entityCodes.Length == 1)
                            str += ")";
                    }
                    else if (index1 == 1000 * index2)
                    {
                        str = str + " OR " + preText + " IN ('" + entityCodes[index1] + "'";
                        if (index1 == entityCodes.Length - 1)
                            str += ")";
                    }
                    else
                        str = index1 != entityCodes.Length - 1 && index1 != 1000 * index2 - 1 ? str + ",'" + entityCodes[index1] + "'" : str + ",'" + entityCodes[index1] + "')";
                }
            }
            return "(" + str + ")";
        }

        private static string _GenerateInFilter(int[] valueArray, string preText)
        {
            string str = string.Empty;
            if (valueArray.Length == 0)
                return preText + " IN (0)";
            int int32 = ConvertToInt32(Math.Ceiling((double)valueArray.Length / 1000.0));
            int index1 = 0;
            for (int index2 = 1; index2 <= int32; ++index2)
            {
                for (; index1 < valueArray.Length && index1 <= 1000 * index2; ++index1)
                {
                    if (index1 == 0)
                    {
                        str = preText + " IN ( " + (object)valueArray[index1];
                        if (valueArray.Length == 1)
                            str += " )";
                    }
                    else if (index1 == 1000 * index2)
                    {
                        str = str + " OR " + preText + " IN ( " + (object)valueArray[index1];
                        if (index1 == valueArray.Length - 1)
                            str += " )";
                    }
                    else if (index1 == valueArray.Length - 1 || index1 == 1000 * index2 - 1)
                        str = str + ", " + (object)valueArray[index1] + " )";
                    else
                        str = str + ", " + (object)valueArray[index1];
                }
            }
            return "(" + str + ")";
        }

        private static string _GenerateNotInFilter(string[] entityCodes, string preText)
        {
            string str = string.Empty;
            if (entityCodes.Length == 0)
                return preText + " NOT IN ('')";
            int int32 = ConvertToInt32(Math.Ceiling((double)entityCodes.Length / 1000.0));
            int index1 = 0;
            for (int index2 = 1; index2 <= int32; ++index2)
            {
                for (; index1 < entityCodes.Length && index1 <= 1000 * index2; ++index1)
                {
                    if (index1 == 0)
                    {
                        str = preText + " NOT IN ('" + entityCodes[index1] + "'";
                        if (entityCodes.Length == 1)
                            str += ")";
                    }
                    else if (index1 == 1000 * index2)
                    {
                        str = str + " OR " + preText + " NOT IN ('" + entityCodes[index1] + "'";
                        if (index1 == entityCodes.Length - 1)
                            str += ")";
                    }
                    else
                        str = index1 != entityCodes.Length - 1 && index1 != 1000 * index2 - 1 ? str + ",'" + entityCodes[index1] + "'" : str + ",'" + entityCodes[index1] + "')";
                }
            }
            return "(" + str + ")";
        }

        private static string _GenerateInFilterVatCodes(int[] vatCodes)
        {
            if (vatCodes.Length == 0)
                return "";
            string str = string.Empty;
            for (int index = 0; index < vatCodes.Length; ++index)
                str = index != 0 ? str + "," + (object)vatCodes[index] : vatCodes[index].ToString();
            return str;
        }

        private static void _Validate(PdaUserParams param)
        {
            if (string.IsNullOrEmpty(param.VehicleWhouseCode))
                throw new Exception("Arac depo belirtilmemis");
            if (string.IsNullOrEmpty(param.VehicleReturnWhouseCode))
                throw new Exception("Arac iade depo belirtilmemis");
            if (string.IsNullOrEmpty(param.OneToOneWhouseCode))
                throw new Exception("Birebir depo belirtilmemis");
        }

        private static string _GenerateBankCodeFilter(PdaUserParams param)
        {
            string str = string.Empty;
            bool flag = false;
            if (!string.IsNullOrEmpty(param.BankAccCode1))
            {
                str = "'" + param.BankAccCode1 + "'";
                flag = true;
            }
            if (!string.IsNullOrEmpty(param.BankAccCode2))
            {
                if (flag)
                    str += ",";
                str = str + "'" + param.BankAccCode2 + "'";
                flag = true;
            }
            if (!string.IsNullOrEmpty(param.BankAccCode3))
            {
                if (flag)
                    str += ",";
                str = str + "'" + param.BankAccCode3 + "'";
            }
            return str;
        }

        private static string isNullCommand = null;
        public static string GetIsNullCommand(IDbConnection connection)
        {
            if (object.ReferenceEquals(isNullCommand, null))
            {
                if (connection is OracleConnection)
                {
                    isNullCommand = "NVL";
                }
                else
                {
                    isNullCommand = "COALESCE";
                }
            }
            return isNullCommand;
        }

        public static string GetIsNullCommandReplace(IDbConnection connection, string sql)
        {
            if (connection is OracleConnection)
            {
                return sql.Replace("NVL(", "NVL(");
            }
            else
            {
                string tmp = sql.Replace("NVL(", "COALESCE(");
                return tmp;
            }
        }

        public static int ConvertToInt32(object value)
        {
            try
            {
                if (object.ReferenceEquals(value, null)) return 0;
                if (object.ReferenceEquals(value, DBNull.Value)) return 0;
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        public static decimal ConvertToDecimal(object value)
        {
            try
            {
                if (object.ReferenceEquals(value, null)) return 0M;
                if (object.ReferenceEquals(value, DBNull.Value)) return 0M;
                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0M;
            }
        }

        public static bool ConvertToBoolean(object value)
        {
            try
            {
                if (object.ReferenceEquals(value, null)) return false;
                if (object.ReferenceEquals(value, DBNull.Value)) return false;
                if (value.ToString() == "t" || value.ToString() == "1") return true;
                if (value.ToString() == "f" || value.ToString() == "0") return false;
                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        public static string ConvertToString(object value)
        {
            try
            {
                if (object.ReferenceEquals(value, null)) return "";
                if (object.ReferenceEquals(value, DBNull.Value)) return "";
                return Convert.ToString(value);
            }
            catch
            {
                return "";
            }
        }

        public static DateTime ConvertToDateTime(object value)
        {
            try
            {
                if (object.ReferenceEquals(value, null)) return DateTime.Now;
                if (object.ReferenceEquals(value, DBNull.Value)) return DateTime.Now;
                return Convert.ToDateTime(value);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static string SendMessage(string id, string msg)
        {
            string res = "";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://onesignal.com/api/v1/notifications");
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = @"{""app_id"": ""d2adb2cd-eb26-4afe-95f2-94728052d810""," + "\n" +
                                    @"""name"": {""en"": ""My notification Name""},     " + "\n" +
                                    @"""contents"": {""en"": """ + msg + @"""}," + "\n" +
                                    @"""headings"": {""en"": ""Update""}," + "\n" +
                                    @"""include_player_ids"": [""" + id + @"""]}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    res = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }

            return res;
        }

        public static string SendBulkMessage(List<string> ids, string msg)
        {
            string res = "";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://onesignal.com/api/v1/notifications");
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = @"{""app_id"": ""d2adb2cd-eb26-4afe-95f2-94728052d810""," + "\n" +
                                    @"""name"": {""en"": ""My notification Name""},     " + "\n" +
                                    @"""contents"": {""en"": """ + msg + @"""}," + "\n" +
                                    @"""headings"": {""en"": ""Update""}," + "\n" +
                                    @"""include_player_ids"": [""" + String.Join("\",\"", ids.ToArray()) + @"""]}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    res = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }

            return res;
        }
    }

    

   

    public class PushMessage
    {
        public string app_id { get; set; }
        public Name name { get; set; }
        public Contents contents { get; set; }
        public Headings headings { get; set; }
        public List<string> include_player_ids { get; set; }
    }

    public class Contents
    {
        public string en { get; set; }
    }

    public class Headings
    {
        public string en { get; set; }
    }

    public class Name
    {
        public string en { get; set; }
    }
}
