namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V09 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ArCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ArCode");
        }
    }
}
