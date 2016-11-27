namespace RecipeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_extra_recipe_details : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RecipeModels", "Servings", c => c.String());
            AddColumn("dbo.RecipeModels", "Time", c => c.String());
            AddColumn("dbo.RecipeModels", "PreparationInstructions", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RecipeModels", "PreparationInstructions");
            DropColumn("dbo.RecipeModels", "Time");
            DropColumn("dbo.RecipeModels", "Servings");
        }
    }
}
