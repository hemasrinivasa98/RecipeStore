namespace RecipeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRecipeCategoryModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        CategoryDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.RecipeModels", "Category_Id", c => c.Int());
            CreateIndex("dbo.RecipeModels", "Category_Id");
            AddForeignKey("dbo.RecipeModels", "Category_Id", "dbo.RecipeCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecipeModels", "Category_Id", "dbo.RecipeCategories");
            DropIndex("dbo.RecipeModels", new[] { "Category_Id" });
            DropColumn("dbo.RecipeModels", "Category_Id");
            DropTable("dbo.RecipeCategories");
        }
    }
}
