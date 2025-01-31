using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    public class ArticleController : Controller
    {
        // GET: BlogController
        public ActionResult Articles()
        {
            return View();
        }

        // Pass in the id of the article that was clicked
        public ActionResult ArticleDetails(int id)
        {
            return View();
        }

        public ActionResult CreateArticle()
        {
            return View();
        }

    }
}
