using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }

        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        [DataType(DataType.Html)]
        public string? Body { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [EmailAddress]
        public string? ContributorUsername { get; set; }
    }
}
