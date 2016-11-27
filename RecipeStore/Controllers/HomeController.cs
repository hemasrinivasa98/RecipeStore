using RecipeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecipeStore.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //Simply returns the latest 10 recipes by the date added
            //var data = db.RecipeModels.OrderByDescending(x => x.DateAdded);
            List<RecipeModel> recipes = RecipeModelsController.GetRecipes();
            return View(recipes.OrderByDescending(x=>x.DateAdded).Take(10));
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Recipe Store";

            return View();
        }
    }
}