namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V04 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "BirthYear");
            DropColumn("dbo.Users", "BirthMonth");
            DropColumn("dbo.Users", "BirthDay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "BirthDay", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "BirthMonth", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "BirthYear", c => c.Int(nullable: false));
        }
    }
}
