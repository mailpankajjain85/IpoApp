using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IpoApp.Models.Entities
{
    // Models/TransactionMaster.cs
    public class TransactionMaster
    {
        public Guid TransactionID { get; set; } = Guid.NewGuid();
        public string OrgShortCode { get; set; }
        public string ClientShortCode { get; set; }
        public Guid IPOId { get; set; }
        public SaudaType SaudaType { get; set; } // "BUY"/"SELL"
        public TransactionType TransactionType { get; set; }
        public AppType AppType { get; set; } // "RETAIL"/"SHNI"/"BHNI"  
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; }
    }

    // DTOs/Transaction/CreateTransactionRequest.cs
    public class CreateTransactionRequest
    {
        [Required]
        public string ClientShortCode { get; set; }

        [Required]
        public Guid IPOId { get; set; }

        [Required]
        [RegularExpression("^(App|Shares|SubjectTo)$")]
        public string SaudaType { get; set; }
        [Required]
        [RegularExpression("^(BUY|SELL)$")]
        public string TransactionType { get; set; }
        [Required]
        [RegularExpression("^(RETAIL|SHNI|BHNI)$")]
        public string AppType { get; set; } // "RETAIL"/"SHNI"/"BHNI"  

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
    public class UpdateTransactionRequest
    {
        public Guid TransactionID { get; set; }
        [Required]
        public string ClientShortCode { get; set; }

        [Required]
        public Guid IPOId { get; set; }

        [Required]
        [RegularExpression("^(App|Shares|SubjectTo)$")]
        public string SaudaType { get; set; }
        [Required]
        [RegularExpression("^(BUY|SELL)$")]
        public string TransactionType { get; set; }
        [Required]
        [RegularExpression("^(RETAIL|SHNI|BHNI)$")]
        public string AppType { get; set; } // "RETAIL"/"SHNI"/"BHNI"  

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
    // DTOs/Transaction/TransactionResponse.cs
    public class TransactionResponse
    {
        public Guid TransactionID { get; set; }
        public string ClientShortCode { get; set; }
        public string IPOId { get; set; }
        public SaudaType SaudaType { get; set; }
        public AppType AppType { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
