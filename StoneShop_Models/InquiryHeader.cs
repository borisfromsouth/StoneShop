using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoneShop_Models
{
    public class InquiryHeader
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }  // значение string потому что ключ - случайно генерируемая строка Guid

        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime InquiryDate { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
