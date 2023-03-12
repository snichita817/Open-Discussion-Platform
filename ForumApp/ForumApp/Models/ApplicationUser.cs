using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumApp.Models
{
    // IdentityUser -> clasa care descrie userul in BD
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Forum>? Forums { get; set; }
        public virtual ICollection<Subforum>? Subforums { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
