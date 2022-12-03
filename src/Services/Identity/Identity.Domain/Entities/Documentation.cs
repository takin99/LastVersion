using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sattec.Identity.Domain.Entities
{
    public class Documentation
    {
        [ForeignKey("ApplicationUser")]
        /// <summary>
        /// آیدی
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// آیدی مدارک 
        /// </summary>
        [Key]
        public Guid? Id { get; set; }
        /// <summary>
        /// توضیح
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// فایل
        /// </summary>
        [NotMapped]
        public IFormFile? File { get; set; }


    }
}
