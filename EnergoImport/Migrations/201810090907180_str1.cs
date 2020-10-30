namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class str1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actions", "Str1", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Actions", "Str1");
        }
    }
}
