namespace DoTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableProjectIdInAssignments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assignments", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Assignments", new[] { "ProjectId" });
            AlterColumn("dbo.Assignments", "ProjectId", c => c.Int());
            CreateIndex("dbo.Assignments", "ProjectId");
            AddForeignKey("dbo.Assignments", "ProjectId", "dbo.Projects", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Assignments", new[] { "ProjectId" });
            AlterColumn("dbo.Assignments", "ProjectId", c => c.Int(nullable: false));
            CreateIndex("dbo.Assignments", "ProjectId");
            AddForeignKey("dbo.Assignments", "ProjectId", "dbo.Projects", "Id", cascadeDelete: true);
        }
    }
}
