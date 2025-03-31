using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpoApp.Models.Entities
{
    // Models/IpoMaster.cs
    public class IpoMaster
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string OrgShortCode { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime ListingDate { get; set; }
        public string Registrar { get; set; }
        public string IPOType { get; set; }  // "SME" or "Mainline"
        public bool HisabDone { get; set; } = false;
        public string CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }

    // DTOs/Ipo/IpoCreateRequest.cs
    public class IpoCreateRequest
    {
        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required]
        public DateTime ClosingDate { get; set; }

        [Required]
        public DateTime ListingDate { get; set; }

        [StringLength(100)]
        public string Registrar { get; set; }

        [Required]
        [RegularExpression("^(SME|Mainline)$")]
        public string IPOType { get; set; }
    }

    // DTOs/Ipo/IpoResponse.cs
    public class IpoResponse
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string OrgShortCode { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime ListingDate { get; set; }
        public string Registrar { get; set; }
        public string IPOType { get; set; }
        public bool HisabDone { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
