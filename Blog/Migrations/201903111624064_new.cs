namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class _new : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BlogPosts", "Date_updated", c => c.DateTime());
        }

        public override void Down()
        {
            AlterColumn("dbo.BlogPosts", "Date_updated", c => c.DateTime(nullable: false));
        }
    }
}
