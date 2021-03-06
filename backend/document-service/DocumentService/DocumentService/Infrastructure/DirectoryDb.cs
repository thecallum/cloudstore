using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentService.Infrastructure
{
    [Table("directory_table")]
    public class DirectoryDb
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("parent_directory_id")]
        public Guid? ParentDirectoryId { get; set; }

        [Column("parent_directory_ids")]
        public string ParentDirectoryIds { get; set; }
    }
}
