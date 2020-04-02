namespace ConsoleApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nottrue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "BirthDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "BirthDate");
        }
    }
}
