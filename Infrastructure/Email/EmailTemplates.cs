using System.Net;

namespace Infrastructure.Email
{
    // Tekstovi i HTML e-mailova na jednom mestu. HTML je jednostavan (inline stil) da radi u svim klijentima.
    public static class EmailTemplates
    {
        private static string Wrap(string title, string bodyHtml) => $@"<div style=""font-family:Arial,Helvetica,sans-serif;max-width:480px;margin:0 auto;padding:24px;color:#1A1A2E"">
  <h2 style=""margin:0 0 16px 0;color:#1A1A2E"">ScoreHub</h2>
  <h3 style=""margin:0 0 12px 0"">{WebUtility.HtmlEncode(title)}</h3>
  {bodyHtml}
  <p style=""margin-top:24px;font-size:12px;color:#8A8A8A"">This is an automated message from ScoreHub. Please do not reply.</p>
</div>";

        public static (string Subject, string Html, string Text) VerificationCode(string code, int validMinutes, Domain.Entities.VerificationPurpose purpose = Domain.Entities.VerificationPurpose.EmailChange)
        {
            var reset = purpose == Domain.Entities.VerificationPurpose.PasswordReset;
            var subject = reset ? "Your ScoreHub password reset code" : "Your ScoreHub verification code";

            var html = Wrap(reset ? "Reset your password" : "Confirm your email address",
                $@"<p>{(reset ? "Enter this code in the ScoreHub app to reset your password:" : "Enter this code in the ScoreHub app to confirm your new email address:")}</p>
  <p style=""font-size:32px;font-weight:bold;letter-spacing:6px;margin:16px 0"">{code}</p>
  <p>The code is valid for {validMinutes} minutes. If you did not request this, you can safely ignore this email.</p>");

            var text = $"Your ScoreHub {(reset ? "password reset" : "verification")} code is {code}. It is valid for {validMinutes} minutes. If you did not request this, ignore this email.";

            return (subject, html, text);
        }

        public static (string Subject, string Html, string Text) Welcome(string firstName, string username)
        {
            var subject = "Welcome to ScoreHub!";
            var name = WebUtility.HtmlEncode(firstName);

            var html = Wrap($"Welcome, {firstName}!",
                $@"<p>Hi {name}, your ScoreHub account <b>{WebUtility.HtmlEncode(username)}</b> is ready.</p>
  <p>Here is what you can do in the app:</p>
  <ul style=""padding-left:20px;line-height:1.6"">
    <li>follow live scores, lineups and statistics for your favorite sports,</li>
    <li>add teams to your favorites and get notified about their matches,</li>
    <li>predict match scores and earn points,</li>
    <li>add friends and compete with them on the leaderboard.</li>
  </ul>
  <p>Enjoy the game!</p>");

            var text = $"Hi {firstName}, your ScoreHub account {username} is ready. Follow live scores, add favorite teams, predict match scores to earn points, and compete with your friends on the leaderboard. Enjoy the game!";

            return (subject, html, text);
        }

        public static (string Subject, string Html, string Text) PasswordChangedNotice(DateTime changedAtUtc)
        {
            var subject = "Your ScoreHub password was changed";
            var when = changedAtUtc.ToString("yyyy-MM-dd HH:mm") + " UTC";

            var html = Wrap("Password changed",
                $@"<p>The password on your ScoreHub account was changed on {when}. For your security, you were signed out on your other devices.</p>
  <p>If this was you, no action is needed. If it was not, please contact us right away.</p>");

            var text = $"The password on your ScoreHub account was changed on {when}. For your security, you were signed out on your other devices. If this was you, no action is needed. If it was not, please contact us right away.";

            return (subject, html, text);
        }

        public static (string Subject, string Html, string Text) AccountDeletedNotice(string username, DateTime deletedAtUtc, DateTime restoreUntilUtc)
        {
            var subject = "Your ScoreHub account was deleted";
            var when = deletedAtUtc.ToString("yyyy-MM-dd HH:mm") + " UTC";
            var until = restoreUntilUtc.ToString("yyyy-MM-dd");

            var html = Wrap("Account deleted",
                $@"<p>Your ScoreHub account <b>{WebUtility.HtmlEncode(username)}</b> was deleted on {when}.</p>
  <p>Changed your mind? Simply log in with your username and password before <b>{until}</b> and your account will be restored. After that date it is permanently removed.</p>
  <p>If you did not do this, change your password or contact us right away.</p>");

            var text = $"Your ScoreHub account {username} was deleted on {when}. Changed your mind? Simply log in with your username and password before {until} and your account will be restored. After that date it is permanently removed. If you did not do this, change your password or contact us right away.";

            return (subject, html, text);
        }

        public static (string Subject, string Html, string Text) AccountRestoredNotice(string username, DateTime restoredAtUtc)
        {
            var subject = "Welcome back to ScoreHub";
            var when = restoredAtUtc.ToString("yyyy-MM-dd HH:mm") + " UTC";

            var html = Wrap("Welcome back",
                $@"<p>Your ScoreHub account <b>{WebUtility.HtmlEncode(username)}</b> was restored on {when}. Your predictions and favorite teams are back.</p>
  <p>Friendships are not restored - you can add your friends again. If this was not you, change your password in the app immediately.</p>");

            var text = $"Your ScoreHub account {username} was restored on {when}. Your predictions and favorite teams are back. Friendships are not restored - you can add your friends again. If this was not you, change your password in the app immediately.";

            return (subject, html, text);
        }

        public static (string Subject, string Html, string Text) EmailChangedNotice(string maskedNewEmail, DateTime changedAtUtc)
        {
            var subject = "Your ScoreHub email address was changed";
            var when = changedAtUtc.ToString("yyyy-MM-dd HH:mm") + " UTC";

            var html = Wrap("Email address changed",
                $@"<p>The email address on your ScoreHub account was changed to <b>{WebUtility.HtmlEncode(maskedNewEmail)}</b> on {when}.</p>
  <p>If this was you, no action is needed. If it was not, change your password in the app immediately.</p>");

            var text = $"The email address on your ScoreHub account was changed to {maskedNewEmail} on {when}. If this was you, no action is needed. If it was not, change your password in the app immediately.";

            return (subject, html, text);
        }

        // "john.doe@gmail.com" -> "j*******@gmail.com"
        public static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1)
                return email;

            return email[0] + new string('*', at - 1) + email[at..];
        }
    }
}
