using Microsoft.AspNetCore.Identity;

namespace ForumApp.Models
{
    // IdentityUser -> clasa care descrie userul in BD
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Forum>? Forums { get; set; }
        public virtual ICollection<Subforum>? Subforums { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
    }
}
