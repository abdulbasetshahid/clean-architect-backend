using EShop.Application.Models.Mail;

namespace EShop.Application.Contracts.Infrastructure;

public interface IEmailService
{
    Task<bool> SendMail(Email email);
}
