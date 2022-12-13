using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;
        public PostsController(ApplicationDbContext context)
        {
            db = context;
        }
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
            //TODO: User
            return View(post);
        }

        [HttpPost]
        public IActionResult New(int id, Post post)
        {

            Subforum s = db.Subforums.Find(id);
            Forum f = db.Forums.Find(s.ForumId);
            Section sec = db.Sections.Find(f.SectionId);
            if (s == null || f == null || sec == null)
            {
                return HttpNotFound();
            }
            post.SubforumId = id;
            post.PostDate = DateTime.Now;
            post.Id = 0;
            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
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
            ViewBag.subforumId = s.Id;
            ViewBag.subforumName = s.SubforumName;
            ViewBag.forumId = f.Id;
            ViewBag.forumName = f.ForumName;
            ViewBag.sectionId = sec.Id;
            ViewBag.sectionName = sec.SectionName;
            return View(post);
        }

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
            ViewBag.subforumId = s.Id;
            ViewBag.subforumName = s.SubforumName;
            ViewBag.forumId = f.Id;
            ViewBag.forumName = f.ForumName;
            ViewBag.sectionId = sec.Id;
            ViewBag.sectionName = sec.SectionName;
            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(int id, Post requestPost)
        {
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
                post.PostTitle = requestPost.PostTitle;
                post.PostContent = requestPost.PostContent;
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
                return View(requestPost);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Show", "Subforums", new { id = post.SubforumId });
        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }
    }
}
