using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Mobile { get; set; }

        [Required]
        [MaxLength(50)]
        public string Address { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        public int StateId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Hobbies { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual State State { get; set; }
    }
}
