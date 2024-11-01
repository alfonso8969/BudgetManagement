namespace BudgetManagement.Models {
    public class ResultReport {
        public decimal Amount { get; set; }
        public OperationType OperationTypeId { get; set; }
        public decimal Incomes { get; set; }
        public decimal Expenses { get; set; }
        public DateTime DateInit { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime ReferenceDate { get; set; }
    }
}
