namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class str1Delete : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Actions", "Str1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Actions", "Str1", c => c.String());
        }
    }
}
