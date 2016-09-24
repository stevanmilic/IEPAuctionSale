namespace IEPAuctionSale.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AuctionModel : DbContext
    {
        public AuctionModel()
            : base("name=AuctionModel")
        {
        }

        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .Property(e => e.ProductName)
                .IsUnicode(false);

            //modelBuilder.Entity<Auction>()
             //  .Property(p => p.RowVersion).IsConcurrencyToken();

            modelBuilder.Entity<Auction>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<Auction>()
                .HasMany(e => e.Bids)
                .WithRequired(e => e.Auction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.StatusIndicatior)
                .IsUnicode(false);
        }
    }
}
