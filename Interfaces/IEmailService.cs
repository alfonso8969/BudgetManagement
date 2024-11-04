namespace BudgetManagement.Interfaces {
    public interface IEmailService {
        Task SendEmailChangePassword(string receptor, string link);
    }
}