namespace BudgetManagement.Models {
    public class Users {
        public int Id {get; set;}
        public string Email {get; set;}
        public string NormalizedEmail {get; set;}
        public string PasswordHash { get; set; }
    }
}
