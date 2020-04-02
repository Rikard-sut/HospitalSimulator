namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixKEY1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Queues", "PatientId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Queues", "PatientId");
        }
    }
}
