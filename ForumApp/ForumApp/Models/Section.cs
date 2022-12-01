using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumApp.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele sectiunii este obligatoriu!")]
        public string SectionName { get; set; }

        public int CountOfForums { get; set; }

        [Required(ErrorMessage = "Tipul de acces este obligatoriu!")]
        public int SectionType { get; set; }

        public virtual ICollection<Forum>? Forums { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Sect { get; set; }
    }
}
