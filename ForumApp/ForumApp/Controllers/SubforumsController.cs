using Microsoft.AspNetCore.Mvc;
using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace ForumApp.Controllers
{
    public class SubforumsController : Controller
    {
        private readonly ApplicationDbContext db;
        public SubforumsController(ApplicationDbContext context)
        {
            db = context;
        }
        public IActionResult Show(int id)
        {
            Subforum subforum = db.Subforums.Include("Posts").Include("Forum").Include("Section")
                .Where(pos => pos.Id == id)
                .First();
            return View(subforum);
        }


        public IActionResult New(int id)
        {
            Subforum subforum = new Subforum();
            Forum f = db.Forums.Find(id);
            Models.Section s = db.Sections.Find(f.SectionId);
            if (f == null)
            {
                return HttpNotFound();
            }
            if (s == null)
            {
                return HttpNotFound();
            }
            subforum.ForumId = id;
            ViewBag.sectionId = f.SectionId;
            ViewBag.sectionName = s.SectionName;
            ViewBag.forumId = f.Id;
            ViewBag.forumName = f.ForumName;
            subforum.SectionId = f.SectionId;


            subforum.AccessLevel = GetAllCategories();
            return View(subforum);
        }
        [HttpPost]
        public IActionResult New(int id, Subforum subforum)
        {
            Forum f = db.Forums.Find(id);
            Models.Section s = db.Sections.Find(f.SectionId);
            if (f == null)
            {
                return HttpNotFound();
            }
            if (s == null)
            {
                return HttpNotFound();
            }
            subforum.ForumId = id;
            subforum.CreationDate = DateTime.Now;
            subforum.MsgCount = 0;
            subforum.ViewCount = 0;
            subforum.Creator = "NULL"; //TODO: De adaugat userul curent
            subforum.LastPostUsr = "NULL";
            subforum.AccessLevel = GetAllCategories();
            subforum.Id = 0; // Fara asta da SqlException: Cannot insert explicit value for identity column in table 'Subforums' when IDENTITY_INSERT is set to OFF. Nu intelegem de ce
            subforum.SectionId = f.SectionId;
            if (ModelState.IsValid)
            {
                db.Subforums.Add(subforum);
                db.SaveChanges();
                return Redirect("/Forums/Show/" + id);
            }
            else
            {
                ViewBag.sectionId = f.SectionId;
                ViewBag.sectionName = s.SectionName;
                ViewBag.forumId = f.Id;
                ViewBag.forumName = f.ForumName;
                return View(subforum);
            }
        
        }

        public IActionResult Edit(int id)
        {
            Subforum subforum = db.Subforums.Find(id);
            if (subforum == null)
            {
                return HttpNotFound();
            }
            subforum.AccessLevel = GetAllCategories();
            return View(subforum);
        }

        [HttpPost]
        public IActionResult Edit(int id, Subforum requestSubforum)
        {
            Subforum subforum = db.Subforums.Find(id);
            if (subforum == null)
            {
                return HttpNotFound();
            }
            requestSubforum.AccessLevel = GetAllCategories();
            if(ModelState.IsValid)
            {
                subforum.SubforumName = requestSubforum.SubforumName;
                subforum.SubforumDesc = requestSubforum.SubforumDesc;
                subforum.AccessLevel = requestSubforum.AccessLevel;
                db.SaveChanges();
                return Redirect("/Subforums/Show/" + id);
            }
            else
            {
                return View(requestSubforum);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Subforum subforum = db.Subforums.Find(id);
            if (subforum == null)
            {
                return HttpNotFound();
            }
            db.Subforums.Remove(subforum);
            db.SaveChanges();
            return Redirect("/Forums/Show/" + subforum.ForumId);
        }

            private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            selectList.Add(new SelectListItem() { Text = "Public", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "Private", Value = "1" });
            return selectList;
        }
    }
}