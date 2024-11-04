using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class ForgetMyPasswordViewModel {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
