using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RecipeStore.Models
{
    public class RecipeModel
    {
        //Administration elements
        public int Id { get; set; }
        [Display(Name = "Recipe Name")]
        public string RecipeName { get; set; }

        [Display(Name= "Recipe Category")]
        public RecipeCategory Category { get; set; }
        public string Description { get; set; }
        
        [Display(Name = "Date Added")]
        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }

        [Display(Name = "Added By")]
        public ApplicationUser CreatedBy { get; set; }

        //Recipe specifics
        public string Ingredients { get; set; }

        [Display(Name = "Number of servings")]
        public string Servings { get; set; }

        [Display(Name = "Cooking Time")]
        public string Time { get; set; }

        [Display(Name = "Instructions")]
        public string PreparationInstructions { get; set; }
    }

    public class RecipeCategory
    {
        public int Id { get; set; }
        
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Description")]
        public string CategoryDescription { get; set; }
    }

    public class RecipeModelCategory
    {
        [Display(Name = "Categories")]
        public List<RecipeCategory> CategoryItems { get; set; }
        public RecipeModel RecipeItem { get; set; }
    }
}
