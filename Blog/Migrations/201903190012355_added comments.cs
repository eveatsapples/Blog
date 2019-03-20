namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Addedcomments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    UserID = c.Int(nullable: false),
                    PostID = c.Int(nullable: false),
                    Body = c.String(),
                    Date_created = c.DateTime(nullable: false),
                    Date_updated = c.DateTime(),
                    Updated_reason = c.String(),
                    User_Id = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Comments", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Comments", new[] { "User_Id" });
            DropTable("dbo.Comments");
        }
    }
}
