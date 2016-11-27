namespace RecipeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecipeModels", "CreatedBy_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.RecipeModels", "CreatedBy_Id");
            AddForeignKey("dbo.RecipeModels", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecipeModels", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.RecipeModels", new[] { "CreatedBy_Id" });
            DropColumn("dbo.RecipeModels", "CreatedBy_Id");
        }
    }
}
