namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Actions", "RegPointId", "dbo.RegPoints");
            DropIndex("dbo.Actions", new[] { "RegPointId" });
            AlterColumn("dbo.Actions", "RegPointId", c => c.Int());
            CreateIndex("dbo.Actions", "RegPointId");
            AddForeignKey("dbo.Actions", "RegPointId", "dbo.RegPoints", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Actions", "RegPointId", "dbo.RegPoints");
            DropIndex("dbo.Actions", new[] { "RegPointId" });
            AlterColumn("dbo.Actions", "RegPointId", c => c.Int(nullable: false));
            CreateIndex("dbo.Actions", "RegPointId");
            AddForeignKey("dbo.Actions", "RegPointId", "dbo.RegPoints", "Id", cascadeDelete: true);
        }
    }
}
