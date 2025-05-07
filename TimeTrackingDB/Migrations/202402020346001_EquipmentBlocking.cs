namespace TimeTrackingDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EquipmentBlocking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EquipmentBlockings",
                c => new
                    {
                        EquipmentBlockID = c.Int(nullable: false, identity: true),
                        EquipmentNumber = c.Int(nullable: false),
                        MalfunctionReasonTypeID = c.Int(),
                        MalfunctionReasonProfileID = c.Int(),
                        MalfunctionReasonNodeID = c.Int(),
                        MalfunctionReasonElementID = c.Int(),
                        MalfunctionReasonMalfunctionTextID = c.Int(),
                        MalfunctionReasonMalfunctionTextComment = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentBlockID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EquipmentBlockings");
        }
    }
}
