namespace DoTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Projects", "ProjectManagerId", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "ProjectManagerId" });
            AlterColumn("dbo.Projects", "ProjectManagerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Projects", "ProjectManagerId");
            AddForeignKey("dbo.Projects", "ProjectManagerId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "ProjectManagerId", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "ProjectManagerId" });
            AlterColumn("dbo.Projects", "ProjectManagerId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Projects", "ProjectManagerId");
            AddForeignKey("dbo.Projects", "ProjectManagerId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
