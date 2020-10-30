namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eSubStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StatusESubs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ESubstations", "ESubStatusId", c => c.Int(nullable: true));
            AddColumn("dbo.ESubstations", "StatusESub_Id", c => c.Int());
            CreateIndex("dbo.ESubstations", "ESubStatusId");
            CreateIndex("dbo.ESubstations", "StatusESub_Id");
            AddForeignKey("dbo.ESubstations", "ESubStatusId", "dbo.ESubstations", "Id");
            AddForeignKey("dbo.ESubstations", "StatusESub_Id", "dbo.StatusESubs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ESubstations", "StatusESub_Id", "dbo.StatusESubs");
            DropForeignKey("dbo.ESubstations", "ESubStatusId", "dbo.ESubstations");
            DropIndex("dbo.ESubstations", new[] { "StatusESub_Id" });
            DropIndex("dbo.ESubstations", new[] { "ESubStatusId" });
            DropColumn("dbo.ESubstations", "StatusESub_Id");
            DropColumn("dbo.ESubstations", "ESubStatusId");
            DropTable("dbo.StatusESubs");
        }
    }
}
