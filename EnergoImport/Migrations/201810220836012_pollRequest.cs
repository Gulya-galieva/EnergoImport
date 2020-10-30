namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pollRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ESubstations", "PollRequest", c => c.Boolean(nullable: false));
            DropColumn("dbo.RegPoints", "InSmeta");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegPoints", "InSmeta", c => c.Boolean(nullable: false));
            DropColumn("dbo.ESubstations", "PollRequest");
        }
    }
}
