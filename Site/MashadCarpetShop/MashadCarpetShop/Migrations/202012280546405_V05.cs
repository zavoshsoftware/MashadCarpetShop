namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V05 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blogs", "CommentCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blogs", "CommentCount");
        }
    }
}
