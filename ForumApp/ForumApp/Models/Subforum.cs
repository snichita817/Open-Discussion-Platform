using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumApp.Models
{
    public class Subforum
    {
        [Key]
        public int Id { get; set; }

        public int? ForumId { get; set; }

        [Required(ErrorMessage = "Tipul subforumului este obligatoriu!")]
        public int SubforumType { get; set; }

        [Required(ErrorMessage = "Numele subforumului este obligatoriu!")]
        [MinLength(2, ErrorMessage = "Lungimea minima trebuie sa fie de 2 caractere")]
        [StringLength(100, ErrorMessage = "Lungimea maxima trebuie sa fie de 100 de caractere")]
        public string SubforumName { get; set; }

        public int MsgCount { get; set; }
        public int ViewCount { get; set; }

        public string? SubforumDesc { get; set; }

        [Required(ErrorMessage = "Data crearii subforumului este obligatorie!")]
        public DateTime CreationDate { get; set; }
        public virtual Forum? Forum { get; set; }
        public string? UserId { get; set; }

        // dc e nullable?
        // practica buna, deoarece subforumul nu o sa aibe proprietatea asta instant
        // mai intai se adauga cheia externa userId si dupa aceea se populeaza aceasta propriete
        // => va fi un interval cand e null
        public virtual ApplicationUser? User { get; set; }           // subforumul apartine unui singur utilizator

        public virtual ICollection<Post>? Posts { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AccessLevel { get; set; }

    }
}
