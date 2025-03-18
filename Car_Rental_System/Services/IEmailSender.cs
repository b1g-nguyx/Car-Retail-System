namespace Car_Rental_System.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);

}