namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Actions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActionType = c.Int(nullable: false),
                        Comment = c.String(),
                        Time = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        PointId = c.Int(nullable: false),
                        ESubstationId = c.Int(nullable: false),
                        RegPoint_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ESubstations", t => t.ESubstationId, cascadeDelete: true)
                .ForeignKey("dbo.RegPoints", t => t.RegPoint_Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ESubstationId)
                .Index(t => t.RegPoint_Id);
            
            CreateTable(
                "dbo.ESubstations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldId = c.Int(nullable: false),
                        Name = c.String(),
                        LastUpdatePointsList = c.DateTime(nullable: false),
                        NetRegionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NetRegions", t => t.NetRegionId, cascadeDelete: true)
                .Index(t => t.NetRegionId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Time = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        ESubstationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ESubstations", t => t.ESubstationId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ESubstationId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Login = c.String(nullable: false),
                        Pass = c.String(nullable: false),
                        AccessImport = c.Boolean(nullable: false),
                        AccessPointStatus = c.Boolean(nullable: false),
                        AccessUserPointStatus = c.Boolean(nullable: false),
                        AccessComments = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NetRegions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LongName = c.String(),
                        LastUpdateESubList = c.DateTime(nullable: false),
                        ContractId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId, cascadeDelete: true)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RegPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OldId = c.Int(nullable: false),
                        Local = c.String(),
                        Address = c.String(),
                        FIO = c.String(),
                        TypePU = c.String(),
                        TypeLink = c.String(),
                        Serial = c.String(),
                        InstallPlace = c.String(),
                        PhoneNumber = c.String(),
                        TTKoefficient = c.String(),
                        AcceptedInEnergo = c.Boolean(nullable: false),
                        AddedInEnergo = c.Boolean(nullable: false),
                        InUSPD = c.Boolean(nullable: false),
                        ESubstationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ESubstations", t => t.ESubstationId, cascadeDelete: true)
                .Index(t => t.ESubstationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Actions", "UserId", "dbo.Users");
            DropForeignKey("dbo.RegPoints", "ESubstationId", "dbo.ESubstations");
            DropForeignKey("dbo.Actions", "RegPoint_Id", "dbo.RegPoints");
            DropForeignKey("dbo.ESubstations", "NetRegionId", "dbo.NetRegions");
            DropForeignKey("dbo.NetRegions", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.Comments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Comments", "ESubstationId", "dbo.ESubstations");
            DropForeignKey("dbo.Actions", "ESubstationId", "dbo.ESubstations");
            DropIndex("dbo.RegPoints", new[] { "ESubstationId" });
            DropIndex("dbo.NetRegions", new[] { "ContractId" });
            DropIndex("dbo.Comments", new[] { "ESubstationId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.ESubstations", new[] { "NetRegionId" });
            DropIndex("dbo.Actions", new[] { "RegPoint_Id" });
            DropIndex("dbo.Actions", new[] { "ESubstationId" });
            DropIndex("dbo.Actions", new[] { "UserId" });
            DropTable("dbo.RegPoints");
            DropTable("dbo.Contracts");
            DropTable("dbo.NetRegions");
            DropTable("dbo.Users");
            DropTable("dbo.Comments");
            DropTable("dbo.ESubstations");
            DropTable("dbo.Actions");
        }
    }
}
