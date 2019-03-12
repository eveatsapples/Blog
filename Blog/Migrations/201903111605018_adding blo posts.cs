namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addingbloposts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BlogPosts", "Date_created", c => c.DateTime(nullable: false));
            AlterColumn("dbo.BlogPosts", "Date_updated", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.BlogPosts", "Date_updated", c => c.String());
            AlterColumn("dbo.BlogPosts", "Date_created", c => c.String());
        }
    }
}
