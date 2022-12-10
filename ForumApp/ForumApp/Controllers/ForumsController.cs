using ForumApp.Data;
using ForumApp.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;
using Section = ForumApp.Models.Section;

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
            /*
             * Titlu forum
             * - Subforum 1
             * - Subforum 2
             */
            Forum forum = db.Forums.Include("Section").Include("Subforums")
                            .Where(subforu => subforu.Id == id)
                            .First();
            return View(forum);
        }

        public IActionResult New(int? id)
        {
            Forum forum = new Forum();
            forum.ForumAccess = GetAllCategories();
            forum.Sect = GetAllSections();

            // daca id-ul nu este null, adica daca clickul pe butonul de add
            // a fost facut din showul unei sectiuni
            // se va transmite un parametru Id, corespunzator unei sectiuni
            if (id != null)
            {
                forum.SectionId = (int)id;
            }

            return View(forum);
        }

        [HttpPost]
        public IActionResult New(Forum forum, int? id)
        {
   
            forum.ForumAccess = GetAllCategories();
            forum.Sect = GetAllSections();
            
            if (ModelState.IsValid)
            {
                forum.Id = 0;                   // setam explicit valoarea Id-ului la 0, deoarece nsh dc din moment ce pasam Idul sectiunii se schimba ceva aici
                                                // baza de date vrea sa primeasca Id 0, deoarece face auto increment la cheie primara
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
