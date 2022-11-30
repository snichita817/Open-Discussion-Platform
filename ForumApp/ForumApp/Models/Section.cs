using System.ComponentModel.DataAnnotations;

namespace ForumApp.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele sectiunii este obligatoriu!")]
        public string SectionName { get; set; }

        public int CountOfForums { get; set; }
        public string SectionType { get; set; }

        public virtual ICollection<Forum> Forums { get; set; }

    }
}
