using BudgetManagement.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
  public class AccountType /*IValidatableObject*/ {
    public int Id { get; set; }
    [Required]
    [Display(Name = "Account type name")]
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters")]
    [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Name can only contain letters")]
    [FirstLetterInUppercase]
    [Remote(action: "CheckTypeAccountExits", controller: "AccountType")]
    public string Name { get; set; }
    public int UserId { get; set; }
    public int Order { get; set; }

    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {

    //    if (!string.IsNullOrEmpty(Name)) {
    //        var firstLetter = Name[0].ToString();
    //        if (firstLetter != firstLetter.ToUpper()) {
    //            yield return new ValidationResult("First letter must be uppercase",
    //                new[] { nameof(Name) });
    //        }
    //    }
    //}
  }
}
