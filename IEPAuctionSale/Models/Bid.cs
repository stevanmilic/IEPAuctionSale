namespace IEPAuctionSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        [Column(Order = 0)]
        public string AccountId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AuctionId { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime TimeBidded { get; set; }

        public virtual Auction Auction { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
