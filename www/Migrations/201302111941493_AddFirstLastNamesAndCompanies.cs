namespace www.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFirstLastNamesAndCompanies : DbMigration
    {
        public override void Up()
        {
            AlterColumn("UserProfiles", "UserEmail", c => c.String(maxLength: 150, nullable: false));
            AddColumn("UserProfiles", "UserFirstName", c => c.String(maxLength: 50, nullable: false));
            AddColumn("UserProfiles", "UserLastName", c => c.String(maxLength: 50, nullable: false));
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false),
                        CompanyLogo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Companies");
            DropTable("dbo.UserProfiles");
        }
    }
}
