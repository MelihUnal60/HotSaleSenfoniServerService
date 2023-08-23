namespace HotSaleServiceTables
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class EndOfDay
    {
        public List<Activity> ActivityList { get; set; }

        public List<Payment> CashPaymentList { get; set; }

        public List<Payment> ChequePaymentList { get; set; }

        public List<Payment> CreditCardPaymentList { get; set; }

        public List<DepositTransaction> DepositTransactionList { get; set; }

        public List<Payment> DraftPaymentList { get; set; }

        public List<EndOfDayItems> EndOfDayItemsList { get; set; }

        public List<EntityCard> EntityCardList { get; set; }

        public List<EntityCheckInInfo> EntityCheckinInfoList { get; set; }

        public List<Entity> EntityList { get; set; }

        public List<InvoiceM> InvoiceMList { get; set; }

        public List<OneToOneM> OneToOneMList { get; set; }

        public List<OrderM> OrderMList { get; set; }

        public List<RemainingItemInfo> RemainingItemInfoList { get; set; }

        public List<SurveyAnswerM> SurveyAnswerMList { get; set; }

        public List<VehicleTransferM> VehicleTransferMList { get; set; }

        public List<VehicleUnloadM> VehicleUnloadMList { get; set; }

        public List<WaybillM> WaybillMList { get; set; }

        public List<SystemLog> LogList { get; set; }
    }
}

