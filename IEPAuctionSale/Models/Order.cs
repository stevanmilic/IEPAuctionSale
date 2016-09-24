namespace IEPAuctionSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string AccountId { get; set; }

        public int NoTokens { get; set; }

        public double PriceOfPackage { get; set; }

        public DateTime TimeOrdered { get; set; }

        [Required]
        [StringLength(20)]
        public string StatusIndicatior { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
