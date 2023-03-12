using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    [Authorize]
    public class ForumsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;             // aceasta clasa are numeroase metode prin care putem sa prelucram useri
        private readonly RoleManager<IdentityRole> _roleManager;
        public ForumsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show(int id, int? showOrder)
        {
            Forum forum = db.Forums.Include("Section").Include("Subforums").Include("User")
                            .Where(foru => foru.Id == id)
                            .First();
            ViewBag.userName = forum.User.UserName;
            if (showOrder == null)
            {
                showOrder = 0;
            }
            ViewBag.showOrder = showOrder;

            switch (showOrder)
            {
                case 1:
                    forum.Subforums = (ICollection<Subforum>?)forum.Subforums.OrderBy(sf => sf.CreationDate).ToList();
                    break;
                case 2:
                    forum.Subforums = (ICollection<Subforum>?)forum.Subforums.OrderBy(sf => sf.SubforumName).ToList();
                    break;
                case 3:
                    forum.Subforums = (ICollection<Subforum>?)forum.Subforums.OrderByDescending(sf => sf.SubforumName).ToList();
                    break;
                case 4:
                    forum.Subforums = (ICollection<Subforum>?)forum.Subforums.OrderByDescending(sf => sf.MsgCount).ToList();
                    break;
                default:
                    forum.Subforums = (ICollection<Subforum>?)forum.Subforums.OrderByDescending(sf => sf.CreationDate).ToList();
                    break;
            }

            SetAccessRights();
            return View(forum);
        }

        private void SetAccessRights()
        {
            ViewBag.IsAdminn = User.IsInRole("Admin");
            ViewBag.IsEditor = User.IsInRole("Editor");
            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        [Authorize(Roles = "User,Editor,Admin")]
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
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(Forum forum, int? id)
        {
            forum.UserId = _userManager.GetUserId(User);        // preluam idul si il stocam in baza de date
            forum.Id = 0;
            Section s = db.Sections.Find(forum.SectionId);
            if (ModelState.IsValid)
            {
                forum.Id = 0;                   // setam explicit valoarea Id-ului la 0, deoarece nsh dc din moment ce pasam Idul sectiunii se schimba ceva aici
                                                // baza de date vrea sa primeasca Id 0, deoarece face auto increment la cheie primara
                db.Forums.Add(forum);
                s.CountOfForums++;
                db.SaveChanges();
                return Redirect("/Sections/Index");
            }
            else
            {
                forum.ForumAccess = GetAllCategories();
                forum.Sect = GetAllSections();
                // mutam cele doua actiuni din afara in interiorul else-ului pentru eficienta programului
                // cele doua metode se apeleaza doar daca crearea forumul nu a avut succes
                // pentru ca noi sa avem din nou informatiile din dropdown-uri

                return View(forum);
            }
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Forum forum = db.Forums.Include("Section")
                            .Where(f => f.Id == id)
                            .First();
            forum.ForumAccess = GetAllCategories();
            forum.Sect = GetAllSections();
            if(forum.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Editor"))
            {
                return View(forum);
            }
            else
            {
                TempData["message"] = "Nu aveti drepturi de editare asupra acestui forum!";
                return Redirect("/Forums/Show/" + id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Forum requestForum)
        {
            Forum forum = db.Forums.Find(id);
            requestForum.ForumAccess = GetAllCategories();
            requestForum.Sect = GetAllSections();

            if(ModelState.IsValid)
            {
                if (forum.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Editor"))
                {
                    forum.ForumName = requestForum.ForumName;
                    forum.ForumDescription = requestForum.ForumDescription;
                    forum.SectionId = requestForum.SectionId;
                    forum.ForumType = requestForum.ForumType;
                    db.SaveChanges();
                    return Redirect("/Forums/Show/" + id);
                }
                else
                {
                    TempData["message"] = "Nu aveti drepturi de editare asupra acestui forum!";
                    return Redirect("/Forums/Show/" + id);
                }
            }
            else
            {
                requestForum.ForumAccess= GetAllCategories();
                return View(requestForum);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
           // Forum forum = db.Forums.Find(id); /*: The DELETE statement conflicted with the REFERENCE constraint "FK_Subforums_Forums_ForumId". The conflict occurred in database "aspnet-ForumApp-B81DF690-405B-48DC-BA7C-CC7910F869E8", table "dbo.Subforums", column 'ForumId'.The statement has been terminated.*/
            var forum = db.Forums.Include("Subforums")
                                   .Where(f => f.Id == id)
                                   .First();
            /*var forum = db.Forums.Include(f => f.Subforums)
                                 .FirstOrDefault(f => f.Id == id);*/
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
