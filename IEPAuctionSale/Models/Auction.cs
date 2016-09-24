namespace IEPAuctionSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;
    [Table("Auction")]
    public partial class Auction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auction()
        {
            Bids = new HashSet<Bid>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public int Duration { get; set; }

        public int StartingPrice { get; set; }

        public int? CurrentPrice { get; set; }

        [ForeignKey("CurrentBider")]
        public virtual ApplicationUser User { get; set; }

        [StringLength(128)]
        public string CurrentBider { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime? TimeOpened { get; set; }

        public DateTime? TimeClosed { get; set; }

        [StringLength(10)]
        public string State { get; set; }

        [Column(TypeName = "image")]
        public byte[] ProductPicture { get; set; }

        [NotMapped]
        public HttpPostedFileBase ProductPictureUpload { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bids { get; set; }

    }
}
