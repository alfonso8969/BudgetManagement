using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models {
    public class Address {

        public int AddressId { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        [Display(Name = "Street Number")]
        public int StreetNumber { get; set; }
        public int Plant { get; set; }
        [Display(Name = "Apt. Number")]
        [Required]
        public int ApartmentNumber { get; set; }
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
            return $"{Street},{StreetNumber} {Plant} {ApartmentNumber}  {City}, {Country}, {Region}, {AutonomousCommunity} {PostalCode}";
        }
    }
}