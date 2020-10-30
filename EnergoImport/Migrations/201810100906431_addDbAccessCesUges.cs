namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDbAccessCesUges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AccessDbUGES", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AccessDbCES", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "AccessDbCES");
            DropColumn("dbo.Users", "AccessDbUGES");
        }
    }
}
