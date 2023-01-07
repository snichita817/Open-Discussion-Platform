using ForumApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>              // avem grija sa fie clasa userului pe care am creat-o
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Subforum> Subforums { get; set; }
    }

}