using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.Models {
    public class AccountCreateViewModel: Account {
        public IEnumerable<SelectListItem> AccountsTypes { get; set; }
    }
}
