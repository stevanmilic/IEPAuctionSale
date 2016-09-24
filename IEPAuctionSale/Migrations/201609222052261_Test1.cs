namespace IEPAuctionSale.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Auction", "RowVersion", c => c.Binary());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Auction", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
    }
}
