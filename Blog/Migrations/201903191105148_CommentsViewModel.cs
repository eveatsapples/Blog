namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentsViewModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShowCommentViewModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        PostID = c.Int(nullable: false),
                        Body = c.String(),
                        Date_created = c.String(),
                        Date_updated = c.String(),
                        Updated_reason = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShowCommentViewModels");
        }
    }
}
