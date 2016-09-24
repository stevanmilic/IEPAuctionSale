namespace IEPAuctionSale.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RowVersionTrue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auction", "RowVersion", c => c.Binary(nullable: true, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auction", "RowVersion");
        }
    }
}
