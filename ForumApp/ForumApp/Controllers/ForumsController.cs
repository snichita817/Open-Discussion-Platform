using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace ForumApp.Controllers
{
    public class ForumsController : Controller
    {
        private readonly ApplicationDbContext db;
        public ForumsController(ApplicationDbContext context)
        {
            db = context;
        }
        
        public IActionResult Show(int id)
        {
            Forum forum = db.Forums.Include("Section").Include("Subforums")
                            .Where(foru => foru.Id == id)
                            .First();
            return View(forum);
        }

        public IActionResult New()
        {
            Forum forum = new Forum();
            forum.ForumAccess = GetAllCategories();
            forum.Sect = GetAllSections();
            return View(forum);
        }

        [HttpPost]
        public IActionResult New(Forum forum)
        {
            forum.ForumAccess = GetAllCategories();
            forum.Sect = GetAllSections();
            if (ModelState.IsValid)
            {
                db.Forums.Add(forum);
                db.SaveChanges();
                return Redirect("/Sections/Index");
            }
            else
            {
                return View(forum);
            }
        }

        public IActionResult Edit(int id)
        {
            Forum forum = db.Forums.Include("Section")
                            .Where(f => f.Id == id)
                            .First();
            forum.ForumAccess = GetAllCategories();
            forum.Sect = GetAllSections();
            return View(forum);
        }

        [HttpPost]
        public IActionResult Edit(int id, Forum requestForum)
        {
            Forum forum = db.Forums.Find(id);
            requestForum.ForumAccess = GetAllCategories();
            requestForum.Sect = GetAllSections();

            if(ModelState.IsValid)
            {
                forum.ForumName = requestForum.ForumName;
                forum.ForumDescription = requestForum.ForumDescription;
                forum.SectionId = requestForum.SectionId;
                forum.ForumAccess = requestForum.ForumAccess;
                db.SaveChanges();
                return Redirect("/Forums/Show/" + id);
            }
            else
            {
                return View(requestForum);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Forum forum = db.Forums.Find(id);
            db.Forums.Remove(forum);
            db.SaveChanges();
            return Redirect("/Sections/Index");
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            selectList.Add(new SelectListItem() { Text = "Public", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "Private", Value = "1" });
            return selectList;
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllSections()
        {
            var selectList = new List<SelectListItem>();
            var sections = from sect in db.Sections
                           select sect;

            foreach(var section in sections)
            {
                selectList.Add(new SelectListItem
                {
                    Value = section.Id.ToString(),
                    Text = section.SectionName.ToString()
                });
            }
            return selectList;
        }
    }
}
