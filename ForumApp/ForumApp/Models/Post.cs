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

        [Required(ErrorMessage = "Id-ul subforumului trebuie sa fie specificat!")]
        public int SubforumId { get; set; }

        [Required(ErrorMessage = "Data postarii trebuie sa fie specificata!")]
        public DateTime PostDate { get; set; }

        [Required(ErrorMessage = "Userul care a creat postarea trebuie sa fie specificat!")]
        public int UserId { get; set; }




    }
}