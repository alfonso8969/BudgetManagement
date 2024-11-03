using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class Person {
        public int PersonId {get; set;}

        
        public string Name {get; set;}

     
        public string LastName {get; set;}

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string PhoneNumber  {get; set;}
        public Address Address {get; set;}

    }
}
