namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accessFlagEditUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EditUsers", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EditUsers");
        }
    }
}
