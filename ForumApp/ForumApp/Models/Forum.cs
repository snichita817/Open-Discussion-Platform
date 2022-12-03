using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumApp.Models
{
    public class Forum
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Sectiunea este obligatorie!")]
        public int SectionId { get; set; }
        [MinLength(5, ErrorMessage = "Lungimea minima trebuie sa fie de 5 caractere")]
        [StringLength(100, ErrorMessage = "Lungimea maxima trebuie sa fie de 100 de caractere")]
        [Required(ErrorMessage = "Numele forumului este obligatoriu!")]
        public string ForumName { get; set; }
        public int ForumType { get; set; }
        public int CountOfSubforums { get; set; }
        public int MsgCount { get; set; }
        [MinLength(5, ErrorMessage = "Lungimea minima trebuie sa fie de 5 caractere")]
        [StringLength(100, ErrorMessage = "Lungimea maxima trebuie sa fie de 100 de caractere")]
        public string ForumDescription { get; set; }
        public virtual Section? Section { get; set; }
        public virtual ICollection<Subforum>? Subforums { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? ForumAccess { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Sect { get; set; }
    }
}
