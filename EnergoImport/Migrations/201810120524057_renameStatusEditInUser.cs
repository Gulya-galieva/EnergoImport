namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameStatusEditInUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EditStatusInEnergo", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "EditStatusLinkIsOk", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "EditStatusAdded", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "AccessEditAccepted");
            DropColumn("dbo.Users", "AccessEditLinkIsOk");
            DropColumn("dbo.Users", "AccessEditAdded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "AccessEditAdded", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AccessEditLinkIsOk", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AccessEditAccepted", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "EditStatusAdded");
            DropColumn("dbo.Users", "EditStatusLinkIsOk");
            DropColumn("dbo.Users", "EditStatusInEnergo");
        }
    }
}
