namespace BudgetManagement.Models {
    public class GetTransactionByAccountViewModel {

        public int UserId { get; set; }
        public int AccountId { get; set; }
        public DateTime DateInit { get; set; }
        public DateTime DateEnd { get; set; }

    }
}
