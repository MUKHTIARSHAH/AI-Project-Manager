namespace AIPM.Application.Portfolio;

/// <summary>
/// Publishes BC-01 integration events.
/// Trace: EVT-010, FR-122, ADR-SAD-004.
/// </summary>
public interface IPortfolioEventPublisher
{
    /// <summary>Publishes a strongly typed integration event message.</summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
