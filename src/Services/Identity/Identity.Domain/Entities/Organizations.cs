using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sattec.Identity.Domain.Entities
{
    public class Organizations
    {
        /// <summary>
        /// آیدی
        /// </summary>
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        /// <summary>
        /// شماره بیمه
        /// </summary>
        [Key]
        public string? InsuranceNumber { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string? Name { get; set; }
    }
}
