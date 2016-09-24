namespace IEPAuctionSale.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wuhu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auction", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auction", "RowVersion");
        }
    }
}
