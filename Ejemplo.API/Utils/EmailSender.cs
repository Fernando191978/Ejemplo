

namespace Ejemplo.API.Utils;


public class EmailSender
{
    private readonly ILogger<EmailSender> logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        this.logger = logger;
    }

    public Task SendAsync(string to, string subject, string body)
    {
        this.logger.LogInformation(
            "[EMAIL] Para: {To} | Asunto: {Subject}| Cuerpo: {Body}",
            to, subject, body);
        return Task.CompletedTask;
    }
}