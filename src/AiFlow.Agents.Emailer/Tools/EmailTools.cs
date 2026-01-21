using AiFlow.Core;
using AiFlow.Core.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.ComponentModel;

namespace AiFlow.Agents.Emailer.Tools;

/// <summary>
/// Provides tools for sending emails using SMTP settings from configuration.
/// </summary>
public sealed class EmailTools
{
    private readonly SmtpOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTools"/> class with the specified SMTP options.
    /// </summary>
    /// <param name="options">The SMTP options to use for email sending.</param>
    public EmailTools(IOptions<SmtpOptions> options) => _options = options.Value;

    /// <summary>
    /// Sends an HTML email using SMTP settings from configuration.
    /// </summary>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="htmlBody">The HTML body of the email.</param>
    /// <param name="to">Optional override recipient email address. If not specified, uses the default recipient from configuration.</param>
    /// <param name="dryRun">Optional override for dry-run mode. If true, the email will not be sent.</param>
    /// <returns>
    /// An <see cref="EmailSendResult"/> containing the delivery status, recipient, subject, and dry-run flag.
    /// </returns>
    [Description("Send an HTML email using SMTP settings from configuration. Returns a delivery status.")]
    public async Task<EmailSendResult> SendEmailAsync(
        [Description("Email subject")] string subject,
        [Description("HTML body")] string htmlBody,
        [Description("Optional override recipient")] string? to = null,
        [Description("Optional override dryRun")] bool? dryRun = null)
    {
        var targetTo = string.IsNullOrWhiteSpace(to) ? _options.To : to;
        var isDryRun = dryRun ?? _options.DryRun;

        if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.From) || string.IsNullOrWhiteSpace(targetTo))
            return new EmailSendResult(targetTo, subject, "SMTP not configured (Host/From/To missing)", isDryRun);

        if (isDryRun)
            return new EmailSendResult(targetTo, subject, "[DRY_RUN] Email not sent (dry-run enabled)", true);

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_options.From));
        message.To.Add(MailboxAddress.Parse(targetTo));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        return new EmailSendResult(targetTo, subject, "Sent", false);
    }
}
