using ForumApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    public class SectionsController : Controller
    {
        private readonly ApplicationDbContext db;
        public SectionsController(ApplicationDbContext context)
        {
            db = context;
        }

        // afisam sectiunile, impreuna cu forumurile
        // avem:
        // sectiune 1 -> forum1, forum2...
        // sectiune n -> forum1, forum2...
        public IActionResult Index()
        {
            var sections = db.Forums.Include("Forums");
            ViewBag.Sections = sections;

            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }
    }
}
