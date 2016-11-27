using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeStore;
using RecipeStore.Controllers;

namespace RecipeStore.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
        
    }

    [TestClass]
    public class RecipeTest
    {
        [TestMethod]
        public void Create()
        {

        }

        [TestMethod]
        public void Delete()
        {

        }

        [TestMethod]
        public void Details()
        {

        }

        [TestMethod]
        public void Edit()
        {

        }

        [TestMethod]
        public void IndexShowContainData()
        {
            RecipeModelsController controller = new RecipeModelsController();
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }
    }
}
