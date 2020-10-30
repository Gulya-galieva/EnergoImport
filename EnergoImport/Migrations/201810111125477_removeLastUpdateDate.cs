namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeLastUpdateDate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ESubstations", "LastUpdatePointsList");
            DropColumn("dbo.NetRegions", "LastUpdateESubList");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NetRegions", "LastUpdateESubList", c => c.DateTime(nullable: false));
            AddColumn("dbo.ESubstations", "LastUpdatePointsList", c => c.DateTime(nullable: false));
        }
    }
}
