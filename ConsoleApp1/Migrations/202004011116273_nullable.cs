namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Queues", "PatientId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Queues", "PatientId", c => c.Int(nullable: false));
        }
    }
}
