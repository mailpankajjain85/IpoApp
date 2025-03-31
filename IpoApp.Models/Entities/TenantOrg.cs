using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpoApp.Models.Entities
{
    public class TenantOrg
    {
        public Guid OrgID { get; set; } = Guid.NewGuid();

        [Required, StringLength(255)]
        public string OrgName { get; set; }

        [Required, StringLength(50)]
        public string OrgShortCode { get; set; }

        [StringLength(20)]
        public string OrgPhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TenantOrgDto
    {
        [Required]
        [StringLength(255)]
        public string OrgName { get; set; }

        [Required]
        [StringLength(50)]
        public string OrgShortCode { get; set; }

        [Phone]
        public string? OrgPhoneNumber { get; set; }

        // Include other fields as needed
    }
}
