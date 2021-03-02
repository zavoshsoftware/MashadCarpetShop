namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V07 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Phone = c.String(),
                        City = c.String(),
                        Address = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletionDate = c.DateTime(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TempStores",
                c => new
                    {
                        StoreID = c.Guid(nullable: false),
                        StoreName = c.String(),
                        StorePhone = c.String(),
                        StoreAddress = c.String(),
                        StoreCity = c.String(),
                        IsStore = c.Boolean(nullable: false),
                        Prov = c.String(),
                        StoreDesc = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.StoreID);
        }
        
        public override void Down()
        {
            DropTable("dbo.TempStores");
            DropTable("dbo.Branches");
        }
    }
}
