using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class RecoverPasswordViewModel {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} is not a valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
