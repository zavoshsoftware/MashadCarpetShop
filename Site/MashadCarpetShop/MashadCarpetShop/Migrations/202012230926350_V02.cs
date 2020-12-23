namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "Title", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "Title", c => c.String(nullable: false, maxLength: 256));
        }
    }
}
