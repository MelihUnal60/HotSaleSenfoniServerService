namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class Category
    {
        public string CategoryCode { get; set; }

        public string CategoryDesc { get; set; }

        public int CategoryId { get; set; }

        public int CategoryLevel { get; set; }

        public string ParentCategoryCode { get; set; }

        public int ParentCategoryId { get; set; }
    }
}

