namespace TimeTrackingDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeePositions",
                c => new
                    {
                        EmployeePositionID = c.Int(nullable: false, identity: true),
                        IsAuxiliary = c.Boolean(nullable: false),
                        EmployeePositionName = c.String(),
                    })
                .PrimaryKey(t => t.EmployeePositionID);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        Shift = c.Int(nullable: false),
                        EmployeePositionID = c.Int(nullable: false),
                        FIO = c.String(),
                        WorkingHours = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeID);
            
            CreateTable(
                "dbo.EquipmentIdles",
                c => new
                    {
                        EquipmentIdleID = c.Int(nullable: false, identity: true),
                        ShiftStart = c.DateTime(),
                        IsNightShift = c.Boolean(nullable: false),
                        EquipmentNumber = c.Int(nullable: false),
                        IdleStart = c.DateTime(),
                        IdleEnd = c.DateTime(),
                        MalfunctionReasonTypeID = c.Int(),
                        MalfunctionReasonProfileID = c.Int(),
                        MalfunctionReasonNodeID = c.Int(),
                        MalfunctionReasonElementID = c.Int(),
                        MalfunctionReasonMalfunctionTextID = c.Int(),
                        MalfunctionReasonMalfunctionTextComment = c.String(),
                        SAPOrderID = c.String(),
                        DivisionParentEquipmentIdleID = c.Int(),
                        DivisionChildEquipmentIdleID = c.Int(),
                        IsOpenIdle = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EquipmentIdleID);
            
            CreateTable(
                "dbo.MaintainShiftEmployees",
                c => new
                    {
                        MaintainShiftEmployeeID = c.Int(nullable: false, identity: true),
                        ShiftDate = c.DateTime(nullable: false),
                        EmployeePositionID = c.Int(nullable: false),
                        FIO = c.String(),
                        WorkingHours = c.Int(nullable: false),
                        AdditionalHours = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaintainShiftEmployeeID);
            
            CreateTable(
                "dbo.MaintainShiftSupervisors",
                c => new
                    {
                        MaintainShiftSupervisorID = c.Int(nullable: false, identity: true),
                        ShiftDate = c.DateTime(),
                        IsNightShift = c.Boolean(nullable: false),
                        FIO = c.String(),
                    })
                .PrimaryKey(t => t.MaintainShiftSupervisorID);
            
            CreateTable(
                "dbo.MalfunctionReasonElements",
                c => new
                    {
                        MalfunctionReasonElementID = c.Int(nullable: false, identity: true),
                        MalfunctionReasonNodeID = c.Int(nullable: false),
                        MalfunctionReasonElementName = c.String(),
                    })
                .PrimaryKey(t => t.MalfunctionReasonElementID);
            
            CreateTable(
                "dbo.MalfunctionReasonMalfunctionTexts",
                c => new
                    {
                        MalfunctionReasonMalfunctionTextID = c.Int(nullable: false, identity: true),
                        MalfunctionReasonTypeID = c.Int(nullable: false),
                        MalfunctionReasonProfileID = c.Int(),
                        MalfunctionReasonNodeID = c.Int(),
                        MalfunctionReasonElementID = c.Int(),
                        MalfunctionReasonMalfunctionTextName = c.String(),
                    })
                .PrimaryKey(t => t.MalfunctionReasonMalfunctionTextID);
            
            CreateTable(
                "dbo.MalfunctionReasonNodes",
                c => new
                    {
                        MalfunctionReasonNodeID = c.Int(nullable: false, identity: true),
                        MalfunctionReasonProfileID = c.Int(nullable: false),
                        MalfunctionReasonNodeName = c.String(),
                    })
                .PrimaryKey(t => t.MalfunctionReasonNodeID);
            
            CreateTable(
                "dbo.MalfunctionReasonProfiles",
                c => new
                    {
                        MalfunctionReasonProfileID = c.Int(nullable: false, identity: true),
                        MalfunctionReasonTypeID = c.Int(nullable: false),
                        MalfunctionReasonProfileName = c.String(),
                    })
                .PrimaryKey(t => t.MalfunctionReasonProfileID);
            
            CreateTable(
                "dbo.MalfunctionReasonTypes",
                c => new
                    {
                        MalfunctionReasonTypeID = c.Int(nullable: false, identity: true),
                        MalfunctionReasonTypeName = c.String(),
                    })
                .PrimaryKey(t => t.MalfunctionReasonTypeID);
            
            CreateTable(
                "dbo.Operators",
                c => new
                    {
                        OperatorID = c.Int(nullable: false, identity: true),
                        ShiftNumber = c.String(),
                        OperatorName = c.String(),
                    })
                .PrimaryKey(t => t.OperatorID);
            
            CreateTable(
                "dbo.Supervisors",
                c => new
                    {
                        SupervisorID = c.Int(nullable: false, identity: true),
                        FIO = c.String(),
                        MaintenanceShift = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SupervisorID);
            
            CreateTable(
                "dbo.TypeOfWorks",
                c => new
                    {
                        TypeOfWorkId = c.Int(nullable: false, identity: true),
                        TypeOfWorkName = c.String(),
                    })
                .PrimaryKey(t => t.TypeOfWorkId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        AutoLogonComputerNames = c.String(),
                        PasswordHash = c.String(),
                        Rights = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.TypeOfWorks");
            DropTable("dbo.Supervisors");
            DropTable("dbo.Operators");
            DropTable("dbo.MalfunctionReasonTypes");
            DropTable("dbo.MalfunctionReasonProfiles");
            DropTable("dbo.MalfunctionReasonNodes");
            DropTable("dbo.MalfunctionReasonMalfunctionTexts");
            DropTable("dbo.MalfunctionReasonElements");
            DropTable("dbo.MaintainShiftSupervisors");
            DropTable("dbo.MaintainShiftEmployees");
            DropTable("dbo.EquipmentIdles");
            DropTable("dbo.Employees");
            DropTable("dbo.EmployeePositions");
        }
    }
}
