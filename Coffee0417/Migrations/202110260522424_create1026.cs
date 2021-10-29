namespace Coffee0417.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create1026 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductName = c.String(nullable: false),
                        ProductImg = c.String(),
                        ProductBrew = c.String(),
                        UnitPrice = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Name = c.String(),
                        Address = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Status = c.Int(nullable: false),
                        Payment = c.Int(nullable: false),
                        Notice = c.Int(nullable: false),
                        ProTotal = c.Int(nullable: false),
                        Shipping = c.Int(nullable: false),
                        SubTotal = c.Int(nullable: false),
                        Remark = c.String(),
                        AddTime = c.DateTime(nullable: false),
                        EidtTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LINE = c.Boolean(nullable: false),
                        Account = c.String(),
                        Password = c.String(),
                        PasswordSalt = c.String(),
                        ImgName = c.String(),
                        Authority = c.Int(nullable: false),
                        CheckAccount = c.Int(nullable: false),
                        Phone = c.String(),
                        Email = c.String(),
                        AddTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.MinProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false),
                        ProductImg = c.String(),
                        ProducPrice = c.Int(nullable: false),
                        ProductDescription = c.String(),
                        ProductStock = c.Int(nullable: false),
                        ProductContent = c.String(),
                        ProductUri = c.String(),
                        ProductItemNo = c.String(),
                        ProductClass = c.String(),
                        Productlabel = c.String(),
                        AddTime = c.DateTime(),
                        EidtTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropTable("dbo.MinProducts");
            DropTable("dbo.Users");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
        }
    }
}
