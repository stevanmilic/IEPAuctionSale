namespace IEPAuctionSale.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wuhu3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auction", "RowVersion", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auction", "RowVersion");
        }
    }
}
