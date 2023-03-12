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
    [Authorize]
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

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;

            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            SetAccessRights();
            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user);     // listam numele rolurilor

            // cautam id ul rolului in baza de date
            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First();     // k selectam un singur rol

            ViewBag.UserRole = currentUserRole;

            // verificam daca userul care acceseaza viewul editului este admin
            // sau este userul insusi
            if(user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(user);
            }
            else
            {
                TempData["message"] = "Access denied!";
                return Redirect("/Sections/Index");
            }

        }

        private void SetAccessRights()
        {
            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public async Task<ActionResult> Edit(string id, ApplicationUser newUser, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();
            if(ModelState.IsValid)
            {

                // verificam sa nu faca cineva pe hackerul
                if (user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    user.UserName = newUser.UserName;
                    user.Email = newUser.Email;
                    user.FirstName = newUser.FirstName;
                    user.LastName = newUser.LastName;
                    user.PhoneNumber = newUser.PhoneNumber;

                    // cautam rolurile in baza de date
                    var roles = db.Roles.ToList();
                    foreach (var role in roles)
                    {
                        // scoatem userul din rolul precedent
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    // il unim cu rolul selectat
                    var roleName = await _roleManager.FindByIdAsync(newRole);
                    await _userManager.AddToRoleAsync(user, roleName.ToString());

                    db.SaveChanges();
                }
            }
            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Changes applied successfully!";
                return Redirect("/Sections/Index");
            }
            
        }

        public async Task<ActionResult> Show(string Id)
        {
            ApplicationUser user = db.Users.Find(Id);
            var roles = await _userManager.GetRolesAsync(user);

            SetAccessRights();

            ViewBag.Roles = roles;
            return View(user);
            
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(string Id)
        {
            var user = db.Users
                            .Include("Forums")
                            .Include("Subforums")
                            .Include("Posts")
                            .Where(u => u.Id == Id)
                            .First();

            // daca e user simplu verific sa fie idul lui
            // daca e admin verific sa nu-si dea singur delete
            if ((user.Id == _userManager.GetUserId(User) && User.IsInRole("Admin") == false) || (user.Id != _userManager.GetUserId(User) && User.IsInRole("Admin"))) 
            {
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
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
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
