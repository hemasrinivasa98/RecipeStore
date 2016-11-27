namespace RecipeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRecipeTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipeName = c.String(),
                        Description = c.String(),
                        Ingredients = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RecipeModels");
        }
    }
}
