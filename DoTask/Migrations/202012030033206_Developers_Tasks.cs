namespace DoTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Developers_Tasks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assignments", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Assignments", new[] { "UserId" });
            CreateTable(
                "dbo.ApplicationUserAssignments",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Assignment_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Assignment_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Assignments", t => t.Assignment_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Assignment_Id);
            
            DropColumn("dbo.Assignments", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assignments", "UserId", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationUserAssignments", "Assignment_Id", "dbo.Assignments");
            DropForeignKey("dbo.ApplicationUserAssignments", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserAssignments", new[] { "Assignment_Id" });
            DropIndex("dbo.ApplicationUserAssignments", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserAssignments");
            CreateIndex("dbo.Assignments", "UserId");
            AddForeignKey("dbo.Assignments", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
