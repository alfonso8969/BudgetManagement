namespace BudgetManagement.Models {
    public class PaginationViewModel {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int maxCountRecordsByPageBy = 50;
        public int TotalRecords { get; set; }

        public int RecordsPerPage {
            get => recordsPerPage;
            set {
                recordsPerPage = value > maxCountRecordsByPageBy ? maxCountRecordsByPageBy : value;
            }
        }

        public int RecordsToSkip =>  RecordsPerPage * (Page - 1);
    }
}
