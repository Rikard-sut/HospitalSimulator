namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class revert : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Queues", "PatientId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Queues", "PatientId", c => c.Int());
        }
    }
}
