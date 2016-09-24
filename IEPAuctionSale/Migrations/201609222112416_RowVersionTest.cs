namespace IEPAuctionSale.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RowVersionTest : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Auction", "RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Auction", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
    }
}
