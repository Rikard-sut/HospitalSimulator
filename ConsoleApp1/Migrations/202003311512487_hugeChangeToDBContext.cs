namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hugeChangeToDBContext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AfterlifePatients", "PatientID", c => c.Int(nullable: false));
            AddColumn("dbo.HealthyPatients", "PatientID", c => c.Int(nullable: false));
            AddColumn("dbo.IVAPatients", "PatientID", c => c.Int(nullable: false));
            AddColumn("dbo.SanatoriumPatients", "PatientID", c => c.Int(nullable: false));
            CreateIndex("dbo.AfterlifePatients", "PatientID");
            CreateIndex("dbo.HealthyPatients", "PatientID");
            CreateIndex("dbo.IVAPatients", "PatientID");
            CreateIndex("dbo.SanatoriumPatients", "PatientID");
            AddForeignKey("dbo.AfterlifePatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            AddForeignKey("dbo.HealthyPatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            AddForeignKey("dbo.IVAPatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            AddForeignKey("dbo.SanatoriumPatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            DropColumn("dbo.AfterlifePatients", "Name");
            DropColumn("dbo.AfterlifePatients", "SSN");
            DropColumn("dbo.AfterlifePatients", "SymptomLevel");
            DropColumn("dbo.HealthyPatients", "Name");
            DropColumn("dbo.HealthyPatients", "SSN");
            DropColumn("dbo.HealthyPatients", "SymptomLevel");
            DropColumn("dbo.IVAPatients", "Name");
            DropColumn("dbo.IVAPatients", "SSN");
            DropColumn("dbo.IVAPatients", "SymptomLevel");
            DropColumn("dbo.SanatoriumPatients", "Name");
            DropColumn("dbo.SanatoriumPatients", "SSN");
            DropColumn("dbo.SanatoriumPatients", "SymptomLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SanatoriumPatients", "SymptomLevel", c => c.Int(nullable: false));
            AddColumn("dbo.SanatoriumPatients", "SSN", c => c.String());
            AddColumn("dbo.SanatoriumPatients", "Name", c => c.String());
            AddColumn("dbo.IVAPatients", "SymptomLevel", c => c.Int(nullable: false));
            AddColumn("dbo.IVAPatients", "SSN", c => c.String());
            AddColumn("dbo.IVAPatients", "Name", c => c.String());
            AddColumn("dbo.HealthyPatients", "SymptomLevel", c => c.Int(nullable: false));
            AddColumn("dbo.HealthyPatients", "SSN", c => c.String());
            AddColumn("dbo.HealthyPatients", "Name", c => c.String());
            AddColumn("dbo.AfterlifePatients", "SymptomLevel", c => c.Int(nullable: false));
            AddColumn("dbo.AfterlifePatients", "SSN", c => c.String());
            AddColumn("dbo.AfterlifePatients", "Name", c => c.String());
            DropForeignKey("dbo.SanatoriumPatients", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.IVAPatients", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.HealthyPatients", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.AfterlifePatients", "PatientID", "dbo.Patients");
            DropIndex("dbo.SanatoriumPatients", new[] { "PatientID" });
            DropIndex("dbo.IVAPatients", new[] { "PatientID" });
            DropIndex("dbo.HealthyPatients", new[] { "PatientID" });
            DropIndex("dbo.AfterlifePatients", new[] { "PatientID" });
            DropColumn("dbo.SanatoriumPatients", "PatientID");
            DropColumn("dbo.IVAPatients", "PatientID");
            DropColumn("dbo.HealthyPatients", "PatientID");
            DropColumn("dbo.AfterlifePatients", "PatientID");
        }
    }
}
