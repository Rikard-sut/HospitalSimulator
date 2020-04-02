namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Queues", "PatientID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Queues", "PatientID", c => c.Int(nullable: false));
        }
    }
}
