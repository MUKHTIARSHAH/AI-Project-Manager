namespace AIPM.Application.Requirements;

/// <summary>
/// Publishes BC-02 Requirements integration events.
/// Trace: EVT-030, FR-010, ADR-SAD-004.
/// </summary>
public interface IRequirementsEventPublisher
{
    /// <summary>Publishes a strongly typed integration event message.</summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
