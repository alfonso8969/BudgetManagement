namespace BudgetManagement.Models {
    public class DetailTransactionReportViewModel {

        public DateTime DateInit { get; set; }
        public DateTime DateEnd { get; set; }

        public IEnumerable<TransactionByDate> TransactionsGrouped { get; set; }

        public decimal BalanceIncomes => TransactionsGrouped.Sum(
            t => t.BalanceIncomes
        );
        public decimal BalanceExpenses => TransactionsGrouped.Sum(
            t => t.BalanceExpenses
        );

        public decimal Total => BalanceIncomes - BalanceExpenses;
        
        
        public class TransactionByDate {
            public DateTime DateTransaction { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; }


            public decimal BalanceIncomes => Transactions.Where(
                t => t.OperationTypeId == OperationType.Income
                ).Sum(
                    t => t.Amount
                );

            public decimal BalanceExpenses => Transactions.Where(
                t => t.OperationTypeId == OperationType.Spent
                ).Sum(
                    t => t.Amount
                );
        }

    }
}
