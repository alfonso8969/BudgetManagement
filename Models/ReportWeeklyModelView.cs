namespace BudgetManagement.Models {
    public class ReportWeeklyModelView {
        public decimal Incomes => ResultTransactionsReportWeeklies.Sum(x => x.Incomes);
        public decimal Expenses => ResultTransactionsReportWeeklies.Sum(x => x.Expenses);
        public decimal Total  => Incomes - Expenses;
        public DateTime ReferenceDate { get; set; }
        public IEnumerable<ResultReportWeekly> ResultTransactionsReportWeeklies { get; set; }
    }
}
