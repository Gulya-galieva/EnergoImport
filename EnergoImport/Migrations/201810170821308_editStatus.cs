namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EditESubStatus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EditESubStatus");
        }
    }
}
