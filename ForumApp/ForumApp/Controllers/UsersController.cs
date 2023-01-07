using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;

            return View();
        }

        public async Task<ActionResult> Show(string Id)
        {
            ApplicationUser user = db.Users.Find(Id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(string Id)
        {
            var user = db.Users
                            .Include("Forums")
                            .Include("Subforums")
                            .Include("Posts")
                            .Where(u => u.Id == Id)
                            .First();

            // stergem forumurile create de user
            if (user.Forums.Count > 0)
            {
                
                foreach (var forum in user.Forums)
                {
                    //Subforum sf = db.Subforums.Find(u => u.Id == forum.Id);

                    db.Forums.Remove(forum);
                }
            }

            if (user.Subforums.Count > 0)
            {
                foreach (var subforum in user.Subforums)
                {
                    // decrementam numarul de subforumuri
                    Forum f = db.Forums.Find(subforum.ForumId);
                    f.CountOfSubforums--;
                    db.Subforums.Remove(subforum);
                }
            }

            if (user.Posts.Count > 0)
            {
                foreach (var post in user.Posts)
                {
                    // decrementam nr de mesaje din forum si subforum
                    Subforum sf = db.Subforums.Find(post.SubforumId);
                    Forum f = db.Forums.Find(sf.ForumId);
                    sf.MsgCount--;
                    f.MsgCount--;
                    db.Posts.Remove(post);
                }
            }

            db.ApplicationUsers.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
