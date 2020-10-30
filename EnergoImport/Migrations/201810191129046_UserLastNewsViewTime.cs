namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserLastNewsViewTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastNewsViewTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LastNewsViewTime");
        }
    }
}
