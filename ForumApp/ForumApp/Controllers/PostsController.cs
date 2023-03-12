using ForumApp.Data;
using ForumApp.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ForumApp.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PostsController(
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
        public IActionResult New(int id)
        {
            Post post = new Post();
            Subforum s = db.Subforums.Find(id);
            Forum f = db.Forums.Find(s.ForumId);
            Section sec = db.Sections.Find(f.SectionId);


            if (s == null || f == null || sec == null)
            {
                return HttpNotFound();
            }
        
            post.SubforumId = id;
            ViewBag.subforumId = s.Id;
            ViewBag.subforumName = s.SubforumName;
            ViewBag.forumId = f.Id;
            ViewBag.forumName = f.ForumName;
            ViewBag.sectionId = sec.Id;
            ViewBag.sectionName = sec.SectionName;

            

            post.PostDate = DateTime.Now;
            return View(post);
        }

        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteEditor = User.IsInRole("Editor");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(int id, Post post)
        {

            Subforum s = db.Subforums.Find(id);
            Forum f = db.Forums.Find(s.ForumId);
            Section sec = db.Sections.Find(f.SectionId);
            if (s == null || f == null || sec == null)
            {
                return HttpNotFound();
            }
            
            post.UserId = _userManager.GetUserId(User);
            post.UserName = _userManager.GetUserName(User);
            post.SubforumId = id;
            post.PostDate = DateTime.Now;
            post.Id = 0;
            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                s.MsgCount++;
                f.MsgCount++;
                db.SaveChanges();
                return RedirectToAction("Show", "Subforums", new { id = post.SubforumId });
            }
            else
            {
                ViewBag.subforumId = s.Id;
                ViewBag.subforumName = s.SubforumName;
                ViewBag.forumId = f.Id;
                ViewBag.forumName = f.ForumName;
                ViewBag.sectionId = sec.Id;
                ViewBag.sectionName = sec.SectionName;
                return View(post);
            }
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show(int id)
        {
            Post post = db.Posts
                .Where(pos => pos.Id == id)
                .First();
            Subforum s = db.Subforums.Find(post.SubforumId);
            Forum f = db.Forums.Find(s.ForumId);
            Section sec = db.Sections.Find(f.SectionId);
            if (s == null || f == null || sec == null)
            {
                return HttpNotFound();
            }

            ViewBag.userForumCreator = f.UserId;
            SetAccessRights();

            ViewBag.subforumId = s.Id;
            ViewBag.subforumName = s.SubforumName;
            ViewBag.forumId = f.Id;
            ViewBag.forumName = f.ForumName;
            ViewBag.sectionId = sec.Id;
            ViewBag.sectionName = sec.SectionName;
            return View(post);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            Subforum s = db.Subforums.Find(post.SubforumId);
            Forum f = db.Forums.Find(s.ForumId);
            Section sec = db.Sections.Find(f.SectionId);
            if (s == null || f == null || sec == null)
            {
                return HttpNotFound();
            }

            if(post.UserId == _userManager.GetUserId(User) || User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.subforumId = s.Id;
                ViewBag.subforumName = s.SubforumName;
                ViewBag.forumId = f.Id;
                ViewBag.forumName = f.ForumName;
                ViewBag.sectionId = sec.Id;
                ViewBag.sectionName = sec.SectionName;
                return View(post);
            }
            else
            {
                TempData["message"] = "Nu aveti drepturi de editare asupra acestui continut!";
                return RedirectToAction("Show", "Subforums", new { id = post.SubforumId });
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Post requestPost)
        {
            var sanitizer = new HtmlSanitizer();

            Post post = db.Posts.Find(id);
            Subforum s = db.Subforums.Find(post.SubforumId);
            Forum f = db.Forums.Find(s.ForumId);
            Section sec = db.Sections.Find(f.SectionId);
            if (s == null || f == null || sec == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Editor") || User.IsInRole("Admin"))
                {
                    

                    post.PostTitle = sanitizer.Sanitize(requestPost.PostTitle);
                    post.PostContent = sanitizer.Sanitize(requestPost.PostContent);
                    db.SaveChanges();
                    return RedirectToAction("Show", "Subforums", new { id = post.SubforumId });
                    
                }
                else
                {
                    TempData["message"] = "Nu aveti drepturi de editare asupra acestui continut!";
                    return RedirectToAction("Show", "Subforums", new { id = post.SubforumId });
                }
            }
            else
            {
                ViewBag.subforumId = s.Id;
                ViewBag.subforumName = s.SubforumName;
                ViewBag.forumId = f.Id;
                ViewBag.forumName = f.ForumName;
                ViewBag.sectionId = sec.Id;
                ViewBag.sectionName = sec.SectionName;
                return View(requestPost);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                db.Posts.Remove(post);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "Nu aveti drepturi de stergere asupra acestui continut!";
            }
            return RedirectToAction("Show", "Subforums", new { id = post.SubforumId });
        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }
    }
}
