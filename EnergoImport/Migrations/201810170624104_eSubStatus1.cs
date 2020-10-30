namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eSubStatus1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ESubstations", "ESubStatusId", "dbo.ESubstations");
            DropForeignKey("dbo.ESubstations", "StatusESub_Id", "dbo.StatusESubs");
            DropIndex("dbo.ESubstations", new[] { "ESubStatusId" });
            DropIndex("dbo.ESubstations", new[] { "StatusESub_Id" });
            RenameColumn(table: "dbo.ESubstations", name: "StatusESub_Id", newName: "StatusESubId");
            AlterColumn("dbo.ESubstations", "StatusESubId", c => c.Int(nullable: true));
            CreateIndex("dbo.ESubstations", "StatusESubId");
            AddForeignKey("dbo.ESubstations", "StatusESubId", "dbo.StatusESubs", "Id", cascadeDelete: false);
            DropColumn("dbo.ESubstations", "ESubStatusId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ESubstations", "ESubStatusId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ESubstations", "StatusESubId", "dbo.StatusESubs");
            DropIndex("dbo.ESubstations", new[] { "StatusESubId" });
            AlterColumn("dbo.ESubstations", "StatusESubId", c => c.Int());
            RenameColumn(table: "dbo.ESubstations", name: "StatusESubId", newName: "StatusESub_Id");
            CreateIndex("dbo.ESubstations", "StatusESub_Id");
            CreateIndex("dbo.ESubstations", "ESubStatusId");
            AddForeignKey("dbo.ESubstations", "StatusESub_Id", "dbo.StatusESubs", "Id");
            AddForeignKey("dbo.ESubstations", "ESubStatusId", "dbo.ESubstations", "Id");
        }
    }
}
