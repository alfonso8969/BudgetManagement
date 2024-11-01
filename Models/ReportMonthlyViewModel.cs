namespace BudgetManagement.Models {
    public class ReportMonthlyViewModel {
        public decimal Incomes => ResultTransactionsReportMonthlies.Sum(x => x.Incomes);
        public decimal Expenses => ResultTransactionsReportMonthlies.Sum(x => x.Expenses);
        public decimal Total => Incomes - Expenses;
        public int Year { get; set; }
        public IEnumerable<ResultReportMonthly> ResultTransactionsReportMonthlies { get; set; }
    }
}
