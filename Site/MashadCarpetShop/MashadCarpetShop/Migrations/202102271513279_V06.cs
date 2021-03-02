namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V06 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempColors",
                c => new
                    {
                        ColorID = c.Int(nullable: false, identity: true),
                        ColorName = c.String(),
                        ColorTitle = c.String(),
                        ColorEN_Title = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                        DeleteDate = c.DateTime(),
                        ColorNo = c.String(),
                        Rus_ColorTitle = c.String(),
                        China_ColorTitle = c.String(),
                    })
                .PrimaryKey(t => t.ColorID);
            
            CreateTable(
                "dbo.TempSizes",
                c => new
                    {
                        SizeID = c.Int(nullable: false, identity: true),
                        SizeTitle = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SizeID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempSizes");
            DropTable("dbo.TempColors");
        }
    }
}
