using System.ComponentModel.DataAnnotations;

namespace ForumApp.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu!")]
        [MinLength(5, ErrorMessage = "Lungimea minima trebuie sa fie de 5 caractere")]
        [StringLength(100, ErrorMessage = "Lungimea maxima trebuie sa fie de 100 de caractere")]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "Continutul este obligatoriu!")]
        [MinLength(1, ErrorMessage = "Lungimea minima trebuie sa fie de 1 caracter")]
        [StringLength(10000, ErrorMessage = "Lungimea maxima trebuie sa fie de 10000 de caractere")]
        public string PostContent { get; set; }

        //[Required(ErrorMessage = "Id-ul subforumului trebuie sa fie specificat!")]
        public int SubforumId { get; set; }

        [Required(ErrorMessage = "Data postarii trebuie sa fie specificata!")]
        public DateTime PostDate { get; set; }
        public string? UserId { get; set; }

        // dc e nullable?
        // practica buna, deoarece postarea nu o sa aibe proprietatea asta instant
        // mai intai se adauga cheia externa userId si dupa aceea se populeaza aceasta propriete
        // => va fi un interval cand e null
        public virtual ApplicationUser? User { get; set; }           // postarea apartine unui singur utilizator


        // [Required(ErrorMessage = "Userul care a creat postarea trebuie sa fie specificat!")]
        // TODO: De pus userul obligatoriu de updatat automat
        /*        public int? UserId { get; set; }*/

        public virtual Subforum? Subforum { get; set; }

        public string? UserName { get; set; }




    }
}