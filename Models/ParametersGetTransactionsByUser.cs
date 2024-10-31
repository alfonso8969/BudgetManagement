namespace BudgetManagement.Models {
    public class ParametersGetTransactionsByUser {
        public int UserId { get; set; }
        public DateTime DateInit { get; set; }
        public DateTime DateEnd { get; set; }
            
    }
}
