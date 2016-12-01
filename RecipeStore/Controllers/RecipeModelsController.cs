using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecipeStore.Models;
using Microsoft.AspNet.Identity;
using RecipeStore;
using System.IO;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RecipeStore.Controllers
{
    public class RecipeModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Retrieve all records when navigating to the index page, perhaps not ideal but temp solution
        [Authorize]
        public async Task<ActionResult> Index()
        {
            ViewBag.SyncOrAsync = "Asynchronous";
            await LoadTestPage(); //Test page that loads recipe from website, currently only contains Anabel Langbein
            var recipes = db.RecipeModels.Include(x => x.CreatedBy).Include(y => y.Category); //Include the creator and category details as well
            return View(recipes);
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Return details of recipe including author and category based on id
            RecipeModel recipes = db.RecipeModels.Include(x => x.CreatedBy).Include(y=>y.Category).First(c => c.Id == id);
            
            if (recipes == null)
            {
                return HttpNotFound();
            }

            return View(recipes);
        }
        
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(RecipeModelCategory recipeModel)
        {
                //Set the date of added to current date. Had to instanciate obj as error popped up otherwise
                recipeModel.RecipeItem.DateAdded = new DateTime();
                recipeModel.RecipeItem.DateAdded = DateTime.Now;

                //Assigning the current logged in user as the creator of the recipe
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                recipeModel.RecipeItem.CreatedBy = user;

                //Assigning a category based on dropdown value
                //Uses form dropdown value to find category then reassigns category data to the model
                int catId = recipeModel.RecipeItem.Category.Id;
                RecipeCategory recCat = new RecipeCategory();
                recCat = db.RecipeCategories.FirstOrDefault(x => x.Id == catId);
                recipeModel.RecipeItem.Category = recCat;

                //Making changes to db and redirecting
                db.RecipeModels.Add(recipeModel.RecipeItem);
                db.SaveChanges();
                return RedirectToAction("Index");
        }
        
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeModelCategory rec = new RecipeModelCategory();
            rec.RecipeItem = db.RecipeModels.Find(id);

            if (rec.RecipeItem == null)
            {
                return HttpNotFound();
            }
            return View(rec);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(RecipeModelCategory recipeModel)
        {
            if (ModelState.IsValid)
            {
                //Retrieve category information
                var category = db.RecipeCategories.FirstOrDefault(x => x.Id == recipeModel.RecipeItem.Category.Id);
                recipeModel.RecipeItem.Category = category;

                //Change recipe details
                db.RecipeModels.Attach(recipeModel.RecipeItem);
                var entry = db.Entry(recipeModel.RecipeItem);
                entry.Property(e => e.Ingredients).IsModified = true;
                entry.Property(e => e.PreparationInstructions).IsModified = true;
                entry.Property(e => e.RecipeName).IsModified = true;
                entry.Property(e => e.Servings).IsModified = true;
                entry.Property(e => e.Time).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;

                //Change category details
                var catEntry = db.Entry(recipeModel.RecipeItem.Category);
                catEntry.Property(e => e.CategoryName).IsModified = true;
                catEntry.Property(e => e.CategoryDescription).IsModified = true;

                //Make changes to db
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recipeModel.RecipeItem);
        }

        // TODO
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeModel recipeModel = db.RecipeModels.Include(x=>x.Category).FirstOrDefault(y=>y.Id == id);
            if (recipeModel == null)
            {
                return HttpNotFound();
            }
            return View(recipeModel);
        }

        // TODO
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            RecipeModel recipeModel = db.RecipeModels.Find(id);
            db.RecipeModels.Remove(recipeModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Static method for use in other methods to allow access to Recipe data, mainly used for Main Index
        public static List<RecipeModel> GetRecipes()
        {
            ApplicationDbContext db = new ApplicationDbContext(); //No access to previous db context?
            List<RecipeModel> recipes = db.RecipeModels.Include(x => x.CreatedBy).Include(y => y.Category).ToList(); //Include the creator and category details as well
            return recipes;
        }

        //Get recipes by user 
        public static List<RecipeModel> GetRecipesByUser(int i)
        {
            ApplicationDbContext db = new ApplicationDbContext(); //No access to previous db context?
            ApplicationUser usr = new ApplicationUser();
            List<RecipeModel> recipes = db.RecipeModels.Include(x => x.CreatedBy).Include(y => y.Category).Where(x=>x.Id == Int32.Parse(usr.Id)).ToList(); //Include the creator and category details as well
            return recipes;
        }

 // ----------------------------------------------------------------- IN PROGRESS ----------------------------------------------
        //Scrapes data from provided URL and gets the Ingredients, Servings and time taken
        //Need to filter based on which site as the sites are not consistent
        
        public static async Task<RecipeModel> FindData(string data)
        {

            //Retrieve HTML doc from Anabel recipe site
            HttpClient http = new HttpClient();
            var response = await http.GetByteArrayAsync(data);
            String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            source = WebUtility.HtmlDecode(source);
            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(source);

            //Using xpath and get by id to get specific data. Will need to change them if the website ever changes structure. 
            //Need to implement try/catch
            var recipeName = resultat.DocumentNode.SelectSingleNode("//*[@id='middle_col']/div[1]/h1").InnerText;
            var ingredients = resultat.GetElementbyId("ingred").InnerText;
            var method = resultat.GetElementbyId("method").InnerText;
            var recipeData = resultat.DocumentNode.SelectSingleNode("//*[@id='middle_col']/div[1]/dl");
            var servings = resultat.DocumentNode.SelectSingleNode("//*[@id='middle_col']/div[1]/dl/dd[3]").InnerText;
            var time = resultat.DocumentNode.SelectSingleNode("//*[@id='middle_col']/div[1]/dl/dd[2]").InnerText;

            //Debug to check if its working
            System.Diagnostics.Debug.WriteLine("Recipe Name: " + recipeName);
            System.Diagnostics.Debug.WriteLine("Servings: " + servings);
            System.Diagnostics.Debug.WriteLine("Time: " + time);

            //Modelling data to recipe model to pass back
            RecipeModel recipe = new RecipeModel();
            recipe.RecipeName = recipeName;
            recipe.Ingredients = ingredients;
            recipe.PreparationInstructions = method;
            recipe.Servings = servings;
            recipe.Time = time;

            return recipe;

        }

        [HttpPost]
        public async Task<ActionResult> CreateFromURL(RecipeFromURL rec)
        {

            //recipe data
            RecipeModel newRecipe = new RecipeModel();
            newRecipe = await FindData(rec.URL);
            newRecipe.Description = rec.Description;

            //date
            newRecipe.DateAdded = DateTime.Now;

            //User conversion
            string id = User.Identity.GetUserId();
            var user = db.Users.Find(id);
            newRecipe.CreatedBy = user;

            //Category
            newRecipe.Category = db.RecipeCategories.Find(rec.Category);

            db.RecipeModels.Add(newRecipe);
            db.SaveChanges();

            return RedirectToAction("Index", "RecipeModels");
        }

        //Perform async data test
        //Currently contains static URL for testing purposes

        //Only anabel langbein recipes will work at the moment
        public static async Task LoadTestPage()
        {
            await FindData("http://www.annabel-langbein.com/recipes/huntsmans-chicken-pie/3382/");
        }
    }
}
