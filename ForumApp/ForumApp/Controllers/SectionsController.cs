using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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
            var sections = db.Sections.Include("Forums");
            ViewBag.Sections = sections;

            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // se afiseaza detaliat o singura sectiune
        // impreuna cu forumurile pe care le include
        public IActionResult Show(int id)
        {
            Section section = db.Sections.Include("Forums")
                                .Where(sec => sec.Id == id).First();
            return View(section);
        }

        // se afiseaza un formular in care se vor completa datele unui forum
        public IActionResult New()
        {
            Section section = new Section();
            section.Sect = GetAllCategories();
            return View(section);
        }

        [HttpPost]
        public IActionResult New(Section section)
        {
            int nr = 1;
            section.CountOfForums = nr;
            section.Sect = GetAllCategories();
            if (ModelState.IsValid)
            {
                db.Sections.Add(section);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            else
            {
                return View(section);
            }
        }

        // se editeaza o sectiune existenta in baza de date
        // se afiseaza formularul impreuna cu datele aferente sectiunii din bd
        public IActionResult Edit(int id)
        {
            Section section = db.Sections.Where(sec => sec.Id == id).First();
            section.Sect = GetAllCategories();

            return View(section);
        }

        [HttpPost]
        public IActionResult Edit(int id, Section requestSection)
        {
            Section section = db.Sections.Find(id);
            requestSection.Sect = GetAllCategories();

            if(ModelState.IsValid)
            {
                section.SectionName = requestSection.SectionName;
                section.CountOfForums = requestSection.CountOfForums;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View(requestSection);
            }

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Section section = db.Sections.Find(id);
            db.Sections.Remove(section);
            db.SaveChanges();
            return RedirectToAction("Index");
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
