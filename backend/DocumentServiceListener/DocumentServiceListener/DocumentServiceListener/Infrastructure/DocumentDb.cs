using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DocumentServiceListener.Infrastructure
{
    [Table("document_table")]
    public class DocumentDb
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [ForeignKey("directory_table")]
        [Column("directory_id")]
        public Guid? DirectoryId { get; set; }
        public DirectoryDb Directory { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("s3_location")]
        public string S3Location { get; set; }

        [Column("file_size")]
        public long FileSize { get; set; }

        [Column("thumbnail")]
        public string Thumbnail { get; set; }

    }
}
