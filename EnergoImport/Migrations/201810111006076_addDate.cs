namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ESubstations", "AddDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RegPoints", "AddDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegPoints", "AddDate");
            DropColumn("dbo.ESubstations", "AddDate");
        }
    }
}
