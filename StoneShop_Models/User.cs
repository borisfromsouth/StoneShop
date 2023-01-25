using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoneShop_Models
{
    public class User : IdentityUser
    {
        // поскольку наследуемся от IdentityUser который уже имеет свои столбцы, то при миграции свойства модели добавятся к таблице 
        [Required]
        public string FullName { get; set; }

        [NotMapped]
        [Required]
        public string Street { get; set; }

        [Required]
        [NotMapped]
        public string City { get; set; }

        [NotMapped]
        public string State { get; set; }

        [Required]
        [NotMapped]
        public string PostCode { get; set; }
    }
}
