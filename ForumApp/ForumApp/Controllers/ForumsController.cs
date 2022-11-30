using ForumApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace ForumApp.Controllers
{
    public class ForumsController : Controller
    {
        private readonly ApplicationDbContext db;
        public ForumsController(ApplicationDbContext context)
        {
            db = context;
        }
        
        // afisam toate forumurile de pe site
        // sub sectiunea din care fac parte

    }
}
