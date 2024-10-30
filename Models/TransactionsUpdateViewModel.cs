namespace BudgetManagement.Models {
    public class TransactionsUpdateViewModel: TransactionCreateViewModel {
        public int PreviousAccountId { get; set; }
        public decimal PreviousAmount { get; set; }
    }
}
