using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sattec.Identity.Domain.Entities
{
    public class Brand
    {
        [ForeignKey("ApplicationUser")]
        /// <summary>
        /// آیدی
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// شماره ثبت
        /// </summary>
        [Key]
        public string? RegistrationNumber { get; set; }
        /// <summary>
        /// نام برند
        /// </summary>
        public string? BrandName { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// لوگو
        /// </summary>
        [NotMapped]
        public IFormFile? Logo { get; set; }
        /// <summary>
        /// صاحب برند
        /// </summary>
        public string? BrandOwner { get; set; }
    }
}
