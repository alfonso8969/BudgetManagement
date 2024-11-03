using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class User: Person {
        public int Id {get; set;}

        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        [DataType(DataType.Text)]
        [Display(Name = "User Name")]
        [RegularExpression(@"^[a-zA-Z0-9'-''_']{1,40}$", ErrorMessage = "Name must contain only letters and numbers")]
        public string UserName { get; set; }

        public string NormalizedEmail {get; set;}

       
        public string PasswordHash { get; set; }
    }
}
