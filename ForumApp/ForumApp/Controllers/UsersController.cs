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

        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user);         // lista nume de roluri

            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First();                     // selectam un singur rol
            ViewBag.UserRole = currentUserRole;

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();


            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.FirstName = newData.FirstName;
                user.LastName = newData.LastName;
                user.PhoneNumber = newData.PhoneNumber;


                // Cautam toate rolurile din baza de date
                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    // Scoatem userul din rolurile anterioare
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }

                // Adaugam noul rol selectat
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();

            }
            return RedirectToAction("Index");
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

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach(var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
