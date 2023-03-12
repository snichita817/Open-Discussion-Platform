using Microsoft.AspNetCore.Mvc;
using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Section = ForumApp.Models.Section;
using Ganss.Xss;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForumApp.Controllers
{
    [Authorize]
    public class SubforumsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;             // aceasta clasa are numeroase metode prin care putem sa prelucram useri
        private readonly RoleManager<IdentityRole> _roleManager;
        public SubforumsController(
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
        public IActionResult Show(int id)
        {
            Subforum subforum = db.Subforums.Include("Posts").Include("Forum")
                .Where(pos => pos.Id == id)
                .First();


            ViewBag.userForumCreator = subforum.Forum.UserId;
            SetAccessRights();
            return View(subforum);
        }

        [HttpPost]
        public IActionResult Show([FromForm] Post post)
        {
            var sanitizer = new HtmlSanitizer();

            Subforum s = db.Subforums.Find(post.SubforumId);
            Forum f = db.Forums.Find(s.ForumId);
            Section sec = db.Sections.Find(f.SectionId);
            if (s == null || f == null || sec == null)
            {
                return HttpNotFound();
            }

            post.UserId = _userManager.GetUserId(User);
            post.UserName = _userManager.GetUserName(User);
            post.PostDate = DateTime.Now;
            post.Id = 0;
            if (ModelState.IsValid)
            {
                post.PostContent = sanitizer.Sanitize(post.PostContent);
                post.PostTitle = sanitizer.Sanitize(post.PostTitle);

                db.Posts.Add(post);
                s.MsgCount++;
                f.MsgCount++;
                db.SaveChanges();
                return RedirectToAction("Show", "Subforums", new { id = post.SubforumId });
            }
            else
            {
                return Redirect("/Subforums/Show/" + post.SubforumId);
            }
        }

        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteEditor = User.IsInRole("Editor");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(int id)
        {
            Subforum subforum = new Subforum();
            Forum f = db.Forums.Find(id);
            //Models.Section s = db.Sections.Find(f.SectionId);
            if (f == null)
            {
                return HttpNotFound();
            }
/*            if (s == null)
            {
                return HttpNotFound();
            }*/
            subforum.ForumId = id;
            //ViewBag.sectionId = f.SectionId;
            //ViewBag.sectionName = s.SectionName;
            ViewBag.forumId = f.Id;
            ViewBag.forumName = f.ForumName;
            //subforum.SectionId = f.SectionId;


            subforum.AccessLevel = GetAllCategories();
            return View(subforum);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(int id, Subforum subforum)
        {
            Forum f = db.Forums.Find(id);
            Section s = db.Sections.Find(f.SectionId);
            //Models.Section s = db.Sections.Find(f.SectionId);
            if (f == null || s == null)
            {
                return HttpNotFound();
            }
/*            if (s == null)
            {
                return HttpNotFound();
            }*/
            
            //subforum.SectionId = f.SectionId;
            subforum.ForumId = id;
            subforum.CreationDate = DateTime.Now;
            subforum.MsgCount = 0;
            subforum.ViewCount = 0;
            subforum.UserId = _userManager.GetUserId(User);
            subforum.Id = 0; // Fara asta da SqlException: Cannot insert explicit value for identity column in table 'Subforums' when IDENTITY_INSERT is set to OFF. Nu intelegem de ce
            if (ModelState.IsValid)
            {
                db.Subforums.Add(subforum);
                f.CountOfSubforums++;
                db.SaveChanges();
                return Redirect("/Forums/Show/" + id);
            }
            else
            {
                subforum.AccessLevel = GetAllCategories();
                //ViewBag.sectionId = f.SectionId;
                //ViewBag.sectionName = s.SectionName;
                ViewBag.forumId = f.Id;
                ViewBag.forumName = f.ForumName;
                return View(subforum);
            }
        
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Subforum subforum = db.Subforums.Find(id);
            if (subforum == null)
            {
                return HttpNotFound();
            }
            subforum.AccessLevel = GetAllCategories();

            // verificam daca userul care modifica este creatorul subforumului
            // sau este admin/editor
            if(subforum.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Editor"))
            {
                return View(subforum);
            }
            else
            {
                TempData["message"] = "Nu aveti drepturi de editare asupra acestei teme!";
                return Redirect("/Subforums/Show/" + id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Subforum requestSubforum)
        {
            Subforum subforum = db.Subforums.Find(id);
            if (subforum == null)
            {
                return HttpNotFound();
            }
            if(ModelState.IsValid)
            {
                // verificam ca cineva sa nu faca smecherii sa dovedeasca sa editeze subforumul
                if(subforum.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Editor"))
                {
                    subforum.SubforumName = requestSubforum.SubforumName;
                    subforum.SubforumDesc = requestSubforum.SubforumDesc;
                    subforum.AccessLevel = requestSubforum.AccessLevel;
                    db.SaveChanges();
                    return Redirect("/Subforums/Show/" + id);
                }
                else
                {
                    TempData["message"] = "Nu aveti drepturi de editare asupra acestei teme!";
                    return Redirect("/Subforums/Show/" + id);
                }
            }
            else
            {
                requestSubforum.AccessLevel = GetAllCategories();
                return View(requestSubforum);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
           // Subforum subforum = db.Subforums.Find(id);   // nu merge cu find cand are postari

            Subforum subforum = db.Subforums.Include("Posts")
                                            .Where(sf => sf.Id == id)
                                            .First();

            if (subforum == null)
            {
                return HttpNotFound();
            }

            // daca este idul userului care a creat subforumul
            // daca este admin sau editor
            if (subforum.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Editor"))
            {
                db.Subforums.Remove(subforum);
                db.SaveChanges();
                return Redirect("/Forums/Show/" + subforum.ForumId);
            }
            else
            {
                TempData["message"] = "Nu aveti drepturi sa stergeti o tema care nu va apartine!";
                return Redirect("/Subforums/Show/" + id);
            }
            
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