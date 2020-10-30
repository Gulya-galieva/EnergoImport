namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editMaxLength1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RegPoints", "Address", c => c.String(maxLength: 1000));
            AlterColumn("dbo.RegPoints", "FIO", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RegPoints", "FIO", c => c.String(maxLength: 100));
            AlterColumn("dbo.RegPoints", "Address", c => c.String(maxLength: 100));
        }
    }
}
