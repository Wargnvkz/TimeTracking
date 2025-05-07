namespace TimeTrackingDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecordDateTimeCreation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdditionalIdleRecords", "RecordDateTimeCreation", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdditionalIdleRecords", "RecordDateTimeCreation");
        }
    }
}
