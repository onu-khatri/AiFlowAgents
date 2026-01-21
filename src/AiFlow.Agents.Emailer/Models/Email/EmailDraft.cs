namespace AiFlow.Core;

/// <summary>
/// Represents a draft of an email, including the subject and HTML body.
/// </summary>
public sealed record EmailDraft(
    /// <summary>
    /// The subject of the email draft.
    /// </summary>
    string Subject,
    /// <summary>
    /// The HTML body content of the email draft.
    /// </summary>
    string HtmlBody);
