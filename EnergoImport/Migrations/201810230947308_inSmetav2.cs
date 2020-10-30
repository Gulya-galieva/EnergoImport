namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inSmetav2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegPoints", "InSmeta", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegPoints", "InSmeta");
        }
    }
}
