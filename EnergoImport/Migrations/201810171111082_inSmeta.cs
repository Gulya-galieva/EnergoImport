namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inSmeta : DbMigration
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
