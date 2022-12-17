using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ForumApp.Controllers
{
    [Authorize]
    public class SectionsController : Controller
    {
        // vrem sa avem acces la baza de date, userManager, cel cu roluri si rolurile
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SectionsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // afisam sectiunile, impreuna cu forumurile
        // avem:
        // sectiune 1 -> forum1, forum2...
        // sectiune n -> forum1, forum2...
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Index()
        {
            var sections = db.Sections.Include("Forums");
            ViewBag.Sections = sections;

            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            
            SetAccessRights();                              // o data ce se apeleaza metoda asta, voi avea acces la toate variabilele de mai jos in ViewBag

            return View();
        }

        private void SetAccessRights()
        {
            ViewBag.AdaugareSectiuni = false;
            ViewBag.EditareSectiuni = false;
            if (User.IsInRole("Admin"))
            {
                ViewBag.EditareSectiuni = true;
                ViewBag.AdaugareSectiuni = true;
            }
            
            if(User.IsInRole("Editor"))
            {
                ViewBag.EditareSectiuni = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        // se afiseaza detaliat o singura sectiune
        // impreuna cu forumurile pe care le include
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show(int id)
        {
            Section section = db.Sections.Include("Forums")
                                .Where(sec => sec.Id == id).First();

            SetAccessRights();                      // setam accesul pt userul curent care acceseaza vier

            return View(section);
        }

        // se afiseaza un formular in care se vor completa datele unui forum
        // doar utilizatorii editor/ admin pot adauga sectiuni pe platforma

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            Section section = new Section();
            section.Sect = GetAllCategories();
            return View(section);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Section section = db.Sections.Where(sec => sec.Id == id).First();
            section.Sect = GetAllCategories();

            return View(section);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            /*Section section = db.Sections.Find(id);*/

            Section section = db.Sections.Include("Forums")
                                         .Where(sec => sec.Id == id)
                                         .First();

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
