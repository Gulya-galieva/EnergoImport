namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linkIsOkRenamed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegPoints", "LinkIsOk", c => c.Boolean(nullable: false));
            DropColumn("dbo.RegPoints", "InUSPD");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegPoints", "InUSPD", c => c.Boolean(nullable: false));
            DropColumn("dbo.RegPoints", "LinkIsOk");
        }
    }
}
