namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aGAINHugeupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AfterlifePatients", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.HealthyPatients", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.IVAPatients", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.QueuePatients", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.SanatoriumPatients", "PatientID", "dbo.Patients");
            DropIndex("dbo.AfterlifePatients", new[] { "PatientID" });
            DropIndex("dbo.HealthyPatients", new[] { "PatientID" });
            DropIndex("dbo.IVAPatients", new[] { "PatientID" });
            DropIndex("dbo.QueuePatients", new[] { "PatientID" });
            DropIndex("dbo.SanatoriumPatients", new[] { "PatientID" });
            CreateTable(
                "dbo.Afterlives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Healthies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IVAs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Queues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sanatoriums",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Patients", "Afterlife_Id", c => c.Int());
            AddColumn("dbo.Patients", "Healthy_Id", c => c.Int());
            AddColumn("dbo.Patients", "IVA_Id", c => c.Int());
            AddColumn("dbo.Patients", "Queue_Id", c => c.Int());
            AddColumn("dbo.Patients", "Sanatorium_Id", c => c.Int());
            CreateIndex("dbo.Patients", "Afterlife_Id");
            CreateIndex("dbo.Patients", "Healthy_Id");
            CreateIndex("dbo.Patients", "IVA_Id");
            CreateIndex("dbo.Patients", "Queue_Id");
            CreateIndex("dbo.Patients", "Sanatorium_Id");
            AddForeignKey("dbo.Patients", "Afterlife_Id", "dbo.Afterlives", "Id");
            AddForeignKey("dbo.Patients", "Healthy_Id", "dbo.Healthies", "Id");
            AddForeignKey("dbo.Patients", "IVA_Id", "dbo.IVAs", "Id");
            AddForeignKey("dbo.Patients", "Queue_Id", "dbo.Queues", "Id");
            AddForeignKey("dbo.Patients", "Sanatorium_Id", "dbo.Sanatoriums", "Id");
            DropTable("dbo.AfterlifePatients");
            DropTable("dbo.HealthyPatients");
            DropTable("dbo.IVAPatients");
            DropTable("dbo.QueuePatients");
            DropTable("dbo.SanatoriumPatients");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SanatoriumPatients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QueuePatients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IVAPatients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HealthyPatients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AfterlifePatients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Patients", "Sanatorium_Id", "dbo.Sanatoriums");
            DropForeignKey("dbo.Patients", "Queue_Id", "dbo.Queues");
            DropForeignKey("dbo.Patients", "IVA_Id", "dbo.IVAs");
            DropForeignKey("dbo.Patients", "Healthy_Id", "dbo.Healthies");
            DropForeignKey("dbo.Patients", "Afterlife_Id", "dbo.Afterlives");
            DropIndex("dbo.Patients", new[] { "Sanatorium_Id" });
            DropIndex("dbo.Patients", new[] { "Queue_Id" });
            DropIndex("dbo.Patients", new[] { "IVA_Id" });
            DropIndex("dbo.Patients", new[] { "Healthy_Id" });
            DropIndex("dbo.Patients", new[] { "Afterlife_Id" });
            DropColumn("dbo.Patients", "Sanatorium_Id");
            DropColumn("dbo.Patients", "Queue_Id");
            DropColumn("dbo.Patients", "IVA_Id");
            DropColumn("dbo.Patients", "Healthy_Id");
            DropColumn("dbo.Patients", "Afterlife_Id");
            DropTable("dbo.Sanatoriums");
            DropTable("dbo.Queues");
            DropTable("dbo.IVAs");
            DropTable("dbo.Healthies");
            DropTable("dbo.Afterlives");
            CreateIndex("dbo.SanatoriumPatients", "PatientID");
            CreateIndex("dbo.QueuePatients", "PatientID");
            CreateIndex("dbo.IVAPatients", "PatientID");
            CreateIndex("dbo.HealthyPatients", "PatientID");
            CreateIndex("dbo.AfterlifePatients", "PatientID");
            AddForeignKey("dbo.SanatoriumPatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            AddForeignKey("dbo.QueuePatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            AddForeignKey("dbo.IVAPatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            AddForeignKey("dbo.HealthyPatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
            AddForeignKey("dbo.AfterlifePatients", "PatientID", "dbo.Patients", "patientID", cascadeDelete: true);
        }
    }
}
