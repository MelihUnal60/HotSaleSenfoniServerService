namespace HotSaleServiceTables
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class BeginOfDay
    {
        public List<BankAcc> BankAccList { get; set; }

        public List<BatchProcess> BatchProcessList { get; set; }

        public List<CampaignExEntity> CampaignExEntityList { get; set; }

        public List<CampaignExItem> CampaignExItemList { get; set; }

        public List<CampaignInEntity> CampaignInEntityList { get; set; }

        public List<CampaignInItem> CampaignInItemList { get; set; }

        public List<Campaign> CampaignList { get; set; }

        public List<CampaignResultItem> CampaignResultItemList { get; set; }

        public List<Category> CategoryList { get; set; }

        public List<Deposit> DepositList { get; set; }

        public List<DepositTransaction> DepositTransactionList { get; set; }

        public List<DueDisc> DueDiscList { get; set; }

        public List<EntityDiscGroup> EntityDiscGroupList { get; set; }

        public List<EntityItem> EntityItemList { get; set; }

        public List<Entity> EntityList { get; set; }

        public List<EntityRouteItem> EntityRouteItemList { get; set; }

        public List<EntityRoute> EntityRouteList { get; set; }

        public List<IadeNedeni> IadeNedeniList { get; set; }

        public List<InvoiceD> InvoiceDList { get; set; }

        public List<InvoiceM> InvoiceMList { get; set; }

        public List<ItemBarcode> ItemBarcodeList { get; set; }

        public List<ItemDiscGroup> ItemDiscGroupList { get; set; }

        public List<Item> ItemList { get; set; }

        public List<ItemTransaction> ItemTransactionList { get; set; }

        public List<ItemUnit> ItemUnitList { get; set; }

        public HotSaleServiceTables.LoadingCard LoadingCard { get; set; }

        public List<OrderD> OrderDList { get; set; }

        public List<OrderInvoiceD> OrderInvoiceDList { get; set; }

        public List<OrderInvoiceM> OrderInvoiceMList { get; set; }

        public List<OrderM> OrderMList { get; set; }

        public HotSaleServiceTables.PdaUserParams PdaUserParams { get; set; }

        public List<PriceList> PriceListList { get; set; }

        public List<Route> RouteList { get; set; }

        public List<SurveyTemplateM> SurveyTemplateMList { get; set; }

        public HotSaleServiceTables.SystemSettings SystemSettings { get; set; }

        public List<WaybillD> WaybillDList { get; set; }

        public List<WaybillM> WaybillMList { get; set; }

        public List<Whouse> WhouseList { get; set; }
    }
}

