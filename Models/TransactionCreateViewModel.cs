using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class TransactionCreateViewModel: Transaction {
        public IEnumerable<SelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; }
    }
}
