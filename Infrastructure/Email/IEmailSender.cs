namespace Infrastructure.Email
{
    public interface IEmailSender
    {
        // Baca izuzetak ako slanje ne uspe (pozivalac odlucuje sta dalje - npr. korisnik moze da klikne "Resend").
        Task SendAsync(string toAddress, string subject, string htmlBody, string textBody, CancellationToken ct = default);
    }
}
