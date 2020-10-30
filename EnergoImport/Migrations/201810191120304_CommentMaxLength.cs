namespace EnergoImport.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Actions", "Comment", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Actions", "Comment", c => c.String(maxLength: 100));
        }
    }
}
