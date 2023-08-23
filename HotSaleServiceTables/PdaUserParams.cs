namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class PdaUserParams
    {
        public string BankAccCode1 { get; set; }

        public string BankAccCode2 { get; set; }

        public string BankAccCode3 { get; set; }

        public int BankReceiptTypeId { get; set; }

        public string BranchCode { get; set; }

        public string CashBoxCode { get; set; }

        public int CashReceiptTypeId { get; set; }

        public string CatCode1 { get; set; }

        public string CatCode2 { get; set; }

        public string ChequePositionCode { get; set; }

        public string CoCode { get; set; }

        public string CoCurCode { get; set; }

        public string ConsigneDocTraCode { get; set; }

        public string ConsigneReturnDocTraCode { get; set; }

        public string ConsigneReturnWhouseCode { get; set; }

        public string ConsigneReturnWhouseDesc { get; set; }

        public string CostCenterCode { get; set; }

        public int DiscId0 { get; set; }

        public int DiscId1 { get; set; }

        public int DiscId2 { get; set; }

        public int Discount1Source { get; set; }

        public int Discount2Source { get; set; }

        public string DraftDebitAccCode { get; set; }

        public string DraftPositionCode { get; set; }

        public string EntityDebitAccCode { get; set; }

        public DateTime InvoiceStartDate { get; set; }

        public bool IsActivityEnabled { get; set; }

        public bool IsControlLocForActivity { get; set; }

        public bool IsControlLocForInvoice { get; set; }

        public bool IsControlLocForOneToOne { get; set; }

        public bool IsControlLocForOrder { get; set; }

        public bool IsControlLocForPayment { get; set; }

        public bool IsControlLocForWaybill { get; set; }

        public bool IsFastOrderEnabled { get; set; }

        public bool IsGetQtyFromLoadingIns { get; set; }

        public bool IsOneToOne { get; set; }

        public bool IsPayment { get; set; }

        public bool IsSalesInvoice { get; set; }

        public bool IsSalesOrder { get; set; }

        public bool IsSalesReturnWaybill { get; set; }

        public bool IsSalesWaybill { get; set; }

        public bool IsSaveLocationForEntity { get; set; }

        public bool IsSaveRealInvoice { get; set; }

        public bool IsSaveRealOneToOne { get; set; }

        public bool IsSaveRealOrder { get; set; }

        public bool IsSaveRealPayment { get; set; }

        public bool IsSaveRealWaybill { get; set; }

        public bool IsUseVatStatus { get; set; }

        public bool IsMobileDnoteGibNo { get; set; }

        public string ItemTransactionDocTraCode { get; set; }

        public string LoadingCardNo1 { get; set; }

        public string LoadingCardNo1Name { get; set; }

        public int LoadingCardNo1Order { get; set; }

        public string LoadingCardNo2 { get; set; }

        public string LoadingCardNo2Name { get; set; }

        public int LoadingCardNo2Order { get; set; }

        public string LoadingCardNo3 { get; set; }

        public string LoadingCardNo3Name { get; set; }

        public int LoadingCardNo3Order { get; set; }

        public string LoadingCardNo4 { get; set; }

        public string LoadingCardNo4Name { get; set; }

        public int LoadingCardNo4Order { get; set; }

        public string LoadingCardNo5 { get; set; }

        public string LoadingCardNo5Name { get; set; }

        public int LoadingCardNo5Order { get; set; }

        public string LoadingCardNo6 { get; set; }

        public string LoadingCardNo6Name { get; set; }

        public int LoadingCardNo6Order { get; set; }

        public string OneToOneDocTraCode { get; set; }

        public string OneToOneWhouseCode { get; set; }

        public string OneToOneWhouseDesc { get; set; }

        public int OrderDeliveryDay { get; set; }

        public int OrderInvoicePriceSource { get; set; }

        public string OrderSaleDocTraCode { get; set; }

        public int OrderWaybillPriceSource { get; set; }

        public string PdaAdminPassword { get; set; }

        public string PdaVehicleTransferPassword { get; set; }

        public string PdaWhouseAdminPassword { get; set; }

        public string ProductWhouseCode { get; set; }

        public string ProductWhouseDesc { get; set; }

        public string RegionCode { get; set; }

        public string SalesInvoiceDocTraCode { get; set; }

        public string SalesOrderDocTraCode { get; set; }

        public string SalesOrderReturnDocTraCode { get; set; }

        public string SalesPersonCode { get; set; }

        public string SalesPersonName { get; set; }

        public string SalesReturnInvoiceDocTraCode { get; set; }

        public string SalesReturnWaybillDocTraCode { get; set; }

        public string SalesWaybillDocTraCode { get; set; }

        public int TraTypeId { get; set; }

        public string UnloadingDocTraCode { get; set; }

        public string VehicleLoadDocTraCode { get; set; }

        public string VehicleReturnWhouseCode { get; set; }

        public string VehicleUnloadingWhouseCode { get; set; }

        public string VehicleWhouseCode { get; set; }

        public string VehicleWhouseDesc { get; set; }

        public int WaybillInvoicePriceSource { get; set; }
        public bool IsSalesOrderReturn { get; set; }
        public bool IsSalesReturnInvoice { get; set; }

        // 11.01.2023
        public bool IsPaymentDel { get;set;}
        public bool IsSalesInvoiceDel { get;set;}
        public bool IsSalesWatbillDel { get;set;}
        public bool  IsSalesOrderDel  { get;set;}

        public bool IsVehicleItem { get; set; } // sadece sunucuda kullanılıyor.

        // 29.01.2023
        public string UserName { get;set;}
        public string UserSurname { get;set;}
        public string UserNote2 { get;set;}

        // 20.03.2023
        public bool IsPaymentUpd { get; set; }
        public bool IsSalesInvoiceUpd { get; set; }
    	public bool IsSalesWaybillUpd { get; set; }
        public bool IsSalesOrderUpd { get; set; }

        // 18.05.2023
        public bool IsFirstPriceList { get; set; }

    }
}

