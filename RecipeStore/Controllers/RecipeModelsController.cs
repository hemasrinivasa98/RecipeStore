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

namespace RecipeStore.Controllers
{
    public class RecipeModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Retrieve all records when navigating to the index page, perhaps not ideal but temp solution
        [Authorize]
        public ActionResult Index()
        {
            var recipes = db.RecipeModels.Include(x => x.CreatedBy).Include(y=>y.Category); //Include the creator and category details as well
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

        // GET: RecipeModels/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeModel recipeModel = db.RecipeModels.Find(id);
            if (recipeModel == null)
            {
                return HttpNotFound();
            }
            return View(recipeModel);
        }

        // TODO
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,RecipeName,Description,Ingredients,DateAdded")] RecipeModel recipeModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipeModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recipeModel);
        }

        // TODO
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeModel recipeModel = db.RecipeModels.Find(id);
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
    }
}
