using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BudgetManagement.Models {
    public class Transaction {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "You should select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "You should select an account")]
        [Display(Name = "Account")]
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Operation Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Parse(DateTime.Today.ToString(), new CultureInfo("es-ES"));
        [StringLength(1000, ErrorMessage = "The note should not pass of {1} characters")]
        public string Note { get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; }

        public string Account { get; set; }
        public string Category { get; set; }
    }
}
