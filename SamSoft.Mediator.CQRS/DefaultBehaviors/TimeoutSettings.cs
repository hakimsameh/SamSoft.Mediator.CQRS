namespace SamSoft.Mediator.CQRS.DefaultBehaviors;

/// <summary>
/// Settings for the TimeoutBehavior pipeline behavior.
/// </summary>
public class TimeoutSettings
{
    /// <summary>
    /// The timeout duration for requests. Default is 5 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
}