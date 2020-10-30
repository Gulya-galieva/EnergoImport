namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Actions", "Comment", c => c.String(maxLength: 100));
            AlterColumn("dbo.ESubstations", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.Comments", "Text", c => c.String(maxLength: 1000));
            AlterColumn("dbo.Users", "Name", c => c.String(maxLength: 100));
            AlterColumn("dbo.Users", "Login", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "Pass", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.NetRegions", "Name", c => c.String(maxLength: 20));
            AlterColumn("dbo.NetRegions", "LongName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Contracts", "Name", c => c.String(maxLength: 20));
            AlterColumn("dbo.RegPoints", "Local", c => c.String(maxLength: 100));
            AlterColumn("dbo.RegPoints", "Address", c => c.String(maxLength: 1000));
            AlterColumn("dbo.RegPoints", "FIO", c => c.String(maxLength: 1000));
            AlterColumn("dbo.RegPoints", "TypePU", c => c.String(maxLength: 50));
            AlterColumn("dbo.RegPoints", "TypeLink", c => c.String(maxLength: 50));
            AlterColumn("dbo.RegPoints", "Serial", c => c.String(maxLength: 30));
            AlterColumn("dbo.RegPoints", "InstallPlace", c => c.String(maxLength: 100));
            AlterColumn("dbo.RegPoints", "PhoneNumber", c => c.String(maxLength: 20));
            AlterColumn("dbo.RegPoints", "TTKoefficient", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RegPoints", "TTKoefficient", c => c.String());
            AlterColumn("dbo.RegPoints", "PhoneNumber", c => c.String());
            AlterColumn("dbo.RegPoints", "InstallPlace", c => c.String());
            AlterColumn("dbo.RegPoints", "Serial", c => c.String());
            AlterColumn("dbo.RegPoints", "TypeLink", c => c.String());
            AlterColumn("dbo.RegPoints", "TypePU", c => c.String());
            AlterColumn("dbo.RegPoints", "FIO", c => c.String());
            AlterColumn("dbo.RegPoints", "Address", c => c.String());
            AlterColumn("dbo.RegPoints", "Local", c => c.String());
            AlterColumn("dbo.Contracts", "Name", c => c.String());
            AlterColumn("dbo.NetRegions", "LongName", c => c.String());
            AlterColumn("dbo.NetRegions", "Name", c => c.String());
            AlterColumn("dbo.Users", "Pass", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Login", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Name", c => c.String());
            AlterColumn("dbo.Comments", "Text", c => c.String());
            AlterColumn("dbo.ESubstations", "Name", c => c.String());
            AlterColumn("dbo.Actions", "Comment", c => c.String());
        }
    }
}
