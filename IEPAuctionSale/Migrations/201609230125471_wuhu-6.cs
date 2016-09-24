namespace IEPAuctionSale.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wuhu6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Auction", "CurrentBider");
            RenameColumn(table: "dbo.Auction", name: "User_Id", newName: "CurrentBider");
            RenameIndex(table: "dbo.Auction", name: "IX_User_Id", newName: "IX_CurrentBider");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Auction", name: "IX_CurrentBider", newName: "IX_User_Id");
            RenameColumn(table: "dbo.Auction", name: "CurrentBider", newName: "User_Id");
            AddColumn("dbo.Auction", "CurrentBider", c => c.String(maxLength: 128));
        }
    }
}
