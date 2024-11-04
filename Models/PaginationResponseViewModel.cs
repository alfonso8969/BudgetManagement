namespace BudgetManagement.Models {
    public class PaginationResponseViewModel {
        public int CurrentPage { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / RecordsPerPage);
        public string BaseURL { get; set; }
    }

    public class PaginationResponseViewModel<T>: PaginationResponseViewModel {
        public IEnumerable<T> Data { get; set; }
    }
}
