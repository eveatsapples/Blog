namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedslug : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogPosts", "Slug", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BlogPosts", "Slug");
        }
    }
}
