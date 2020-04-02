namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Queues", "PatientID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Queues", "PatientID");
        }
    }
}
