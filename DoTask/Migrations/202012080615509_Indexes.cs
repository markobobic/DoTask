namespace DoTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Indexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Assignments", new[] { "StatusId" });
            DropIndex("dbo.Assignments", new[] { "ProjectId" });
            CreateIndex("dbo.Assignments", "StatusId");
            CreateIndex("dbo.Assignments", "ProjectId");
            CreateIndex("dbo.AspNetUsers", "FirstName");
            CreateIndex("dbo.AspNetUsers", "LastName");
            CreateIndex("dbo.Projects", "Name");
            CreateIndex("dbo.Status", "Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Status", new[] { "Name" });
            DropIndex("dbo.Projects", new[] { "Name" });
            DropIndex("dbo.AspNetUsers", new[] { "LastName" });
            DropIndex("dbo.AspNetUsers", new[] { "FirstName" });
            DropIndex("dbo.Assignments", new[] { "ProjectId" });
            DropIndex("dbo.Assignments", new[] { "StatusId" });
            CreateIndex("dbo.Assignments", "ProjectId");
            CreateIndex("dbo.Assignments", "StatusId");
        }
    }
}
