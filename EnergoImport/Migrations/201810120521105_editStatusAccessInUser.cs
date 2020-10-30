namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editStatusAccessInUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AccessEditAccepted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AccessEditLinkIsOk", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AccessEditAdded", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "AccessPointStatus");
            DropColumn("dbo.Users", "AccessUserPointStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "AccessUserPointStatus", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AccessPointStatus", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "AccessEditAdded");
            DropColumn("dbo.Users", "AccessEditLinkIsOk");
            DropColumn("dbo.Users", "AccessEditAccepted");
        }
    }
}
