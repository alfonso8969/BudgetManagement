using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class Address {

        public int AddressId { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string StreetNumber { get; set; }
        public string Plant { get; set; }
        public string ApartmentNumber { get; set; }
        [Required]       
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        [StringLength(2, MinimumLength = 2)]
        [Display(Name = "A. community")]
        public string AutonomousCommunity { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Invalid Postal Code")]
        [StringLength(5, MinimumLength = 5)]
        public int PostalCode { get; set; }

        public override string ToString() {
            return $"{Street},{StreetNumber?? ""} {Plant ?? ""} {ApartmentNumber ?? ""}  {City}, {Country}, {Region}, {AutonomousCommunity} {PostalCode}";
        }
    }
}