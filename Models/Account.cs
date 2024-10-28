using BudgetManagement.Validations;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class Account {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "Name is too long")]
        [FirstLetterInUppercase]
        public string Name { get; set; }
        [Display(Name = "Account Type")]
        //[AccountTypeValidation]
        public int AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage = "Description is too long")]
        public string Description { get; set; }
        public string AccountType { get; set; }
    }
}
