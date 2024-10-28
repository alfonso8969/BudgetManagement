namespace BudgetManagement.Models {
    public class IndexAccountsViewModel {
        public string AccountType { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public decimal Balance => Accounts.Sum(x => x.Balance);
    }
}
