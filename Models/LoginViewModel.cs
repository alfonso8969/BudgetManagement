using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class LoginViewModel {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} must be a valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}