namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActionRegPointId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Actions", "RegPoint_Id", "dbo.RegPoints");
            DropIndex("dbo.Actions", new[] { "RegPoint_Id" });
            RenameColumn(table: "dbo.Actions", name: "RegPoint_Id", newName: "RegPointId");
            AlterColumn("dbo.Actions", "RegPointId", c => c.Int(nullable: true));
            CreateIndex("dbo.Actions", "RegPointId");
            AddForeignKey("dbo.Actions", "RegPointId", "dbo.RegPoints", "Id", cascadeDelete: false);
            DropColumn("dbo.Actions", "PointId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Actions", "PointId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Actions", "RegPointId", "dbo.RegPoints");
            DropIndex("dbo.Actions", new[] { "RegPointId" });
            AlterColumn("dbo.Actions", "RegPointId", c => c.Int());
            RenameColumn(table: "dbo.Actions", name: "RegPointId", newName: "RegPoint_Id");
            CreateIndex("dbo.Actions", "RegPoint_Id");
            AddForeignKey("dbo.Actions", "RegPoint_Id", "dbo.RegPoints", "Id");
        }
    }
}
