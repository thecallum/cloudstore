using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentService.Infrastructure
{
    [Table("user_table")]
    public class UserDb
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        [Required]
        public string LastName { get; set; }

        [Column("email")]
        [Required]
        public string Email { get; set; }

        [Column("hash")]
        [Required]
        public string Hash { get; set; }
    }
}