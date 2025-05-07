namespace TimeTrackingDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalIdleRecordFileFilename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdditionalIdleRecordFiles", "Filename", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdditionalIdleRecordFiles", "Filename");
        }
    }
}
