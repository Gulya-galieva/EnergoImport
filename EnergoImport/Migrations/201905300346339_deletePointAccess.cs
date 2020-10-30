namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletePointAccess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DeleteRegPoint", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DeleteRegPoint");
        }
    }
}
