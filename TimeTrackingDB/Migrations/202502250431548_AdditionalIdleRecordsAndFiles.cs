namespace TimeTrackingDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalIdleRecordsAndFiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdditionalIdleRecordFiles",
                c => new
                    {
                        AdditionalIdleRecordFileID = c.Int(nullable: false, identity: true),
                        AdditionalIdleRecordID = c.Int(nullable: false),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.AdditionalIdleRecordFileID);
            
            CreateTable(
                "dbo.AdditionalIdleRecords",
                c => new
                    {
                        AdditionalIdleRecordID = c.Int(nullable: false, identity: true),
                        EquipmentIdleID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.AdditionalIdleRecordID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AdditionalIdleRecords");
            DropTable("dbo.AdditionalIdleRecordFiles");
        }
    }
}
