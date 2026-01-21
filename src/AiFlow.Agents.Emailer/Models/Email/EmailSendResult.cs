namespace AiFlow.Core;

/// <summary>
/// Represents the result of sending an email, including recipient, subject, status, and whether it was a dry run.
/// </summary>
public sealed record EmailSendResult(
    /// <summary>
    /// The recipient email address.
    /// </summary>
    string To,
    /// <summary>
    /// The subject of the email.
    /// </summary>
    string Subject,
    /// <summary>
    /// The status of the email send operation.
    /// </summary>
    string Status,
    /// <summary>
    /// Indicates whether the email send was a dry run (not actually sent).
    /// </summary>
    bool DryRun);
