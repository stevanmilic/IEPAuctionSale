namespace IEPAuctionSale.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wuhu4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Auction", "RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Auction", "RowVersion", c => c.Binary());
        }
    }
}
