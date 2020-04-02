namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        patientID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SSN = c.String(),
                        SymptomLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.patientID);
            
            AddColumn("dbo.QueuePatients", "PatientID", c => c.Int(nullable: false));
            CreateIndex("dbo.QueuePatients", "PatientID");
            AddForeignKey("dbo.QueuePatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            DropColumn("dbo.QueuePatients", "Name");
            DropColumn("dbo.QueuePatients", "SSN");
            DropColumn("dbo.QueuePatients", "SymptomLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QueuePatients", "SymptomLevel", c => c.Int(nullable: false));
            AddColumn("dbo.QueuePatients", "SSN", c => c.String());
            AddColumn("dbo.QueuePatients", "Name", c => c.String());
            DropForeignKey("dbo.QueuePatients", "PatientID", "dbo.Patients");
            DropIndex("dbo.QueuePatients", new[] { "PatientID" });
            DropColumn("dbo.QueuePatients", "PatientID");
            DropTable("dbo.Patients");
        }
    }
}
