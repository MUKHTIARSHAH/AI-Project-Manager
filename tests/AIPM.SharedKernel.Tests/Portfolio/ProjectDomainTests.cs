using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class ProjectDomainTests
{
    [Fact]
    public void Create_RaisesProjectCreatedDomainEvent_AndStartsAsDraft()
    {
        var project = ProjectAggregate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Mobile App v2");

        project.Status.Should().Be(ProjectStatus.Draft);
        project.IsArchived.Should().BeFalse();
        project.DomainEvents.Should().ContainSingle(x => x is ProjectCreatedDomainEvent);
    }

    [Fact]
    public void Create_RequiresOwnerProgramWorkspaceAndTenant()
    {
        var act = () => ProjectAggregate.Create(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "X");
        act.Should().Throw<ValidationError>().WithMessage("*TenantId*");

        act = () => ProjectAggregate.Create(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "X");
        act.Should().Throw<ValidationError>().WithMessage("*ProgramId*");

        act = () => ProjectAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "X");
        act.Should().Throw<ValidationError>().WithMessage("*WorkspaceId*");

        act = () => ProjectAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "X");
        act.Should().Throw<ValidationError>().WithMessage("*OwnerUserId*");

        act = () => ProjectAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), " ");
        act.Should().Throw<ValidationError>().WithMessage("*name*");
    }

    [Fact]
    public void Archive_RaisesProjectArchivedDomainEvent_AndMakesReadOnly()
    {
        var project = ProjectAggregate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Archive Me");
        project.ClearDomainEvents();

        project.Archive();

        project.Status.Should().Be(ProjectStatus.Archived);
        project.ArchivedAt.Should().NotBeNull();
        project.DomainEvents.Should().ContainSingle(x => x is ProjectArchivedDomainEvent);

        var update = () => project.Update("New Name", Guid.NewGuid(), Guid.NewGuid());
        update.Should().Throw<ValidationError>().WithMessage("*read-only*");
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_Throws()
    {
        var project = ProjectAggregate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Twice");
        project.Archive();

        var act = () => project.Archive();
        act.Should().Throw<ValidationError>().WithMessage("*already archived*");
    }

    [Fact]
    public void Update_ChangesMutableFields()
    {
        var project = ProjectAggregate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Original");
        var owner = Guid.NewGuid();
        var workspace = Guid.NewGuid();

        project.Update("Renamed", owner, workspace);

        project.Name.Should().Be("Renamed");
        project.OwnerUserId.Should().Be(owner);
        project.WorkspaceId.Should().Be(workspace);
    }

    [Fact]
    public void RecordScopeChange_OnActiveProject_AddsProposedAndRaisesEvent()
    {
        var project = ProjectAggregate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Scoped");
        project.ClearDomainEvents();

        var scopeChange = project.RecordScopeChange(
            "Add OAuth2",
            "Extend login scope",
            "REQ-1, REQ-2");

        scopeChange.Status.Should().Be(ScopeChangeStatus.Proposed);
        project.ScopeChanges.Should().ContainSingle(x => x.Id == scopeChange.Id);
        project.DomainEvents.Should().ContainSingle(x => x is ScopeChangeRecordedDomainEvent);
        scopeChange.AffectedRequirementCitation.Should().Be("REQ-1, REQ-2");
    }

    [Fact]
    public void RecordScopeChange_OnArchivedProject_Throws()
    {
        var project = ProjectAggregate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Archived Scope");
        project.Archive();

        var act = () => project.RecordScopeChange("Title", "Description");
        act.Should().Throw<ValidationError>().WithMessage("*read-only*");
    }

    [Fact]
    public void ApproveRejectImplement_FollowLifecycle_AndInvalidTransitionsThrow()
    {
        var project = ProjectAggregate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Lifecycle");
        var proposed = project.RecordScopeChange("Title", "Description");
        project.ClearDomainEvents();

        var approved = project.ApproveScopeChange(proposed.Id);
        approved.Status.Should().Be(ScopeChangeStatus.Approved);
        approved.DecidedAt.Should().NotBeNull();
        project.DomainEvents.Should().ContainSingle(x => x is ScopeChangeApprovedDomainEvent);

        var implement = project.MarkScopeChangeImplemented(proposed.Id);
        implement.Status.Should().Be(ScopeChangeStatus.Implemented);
        project.DomainEvents.Should().Contain(x => x is ScopeChangeImplementedDomainEvent);

        var rejectAfterImplemented = () => project.RejectScopeChange(proposed.Id);
        rejectAfterImplemented.Should().Throw<ValidationError>();

        var other = project.RecordScopeChange("Other", "Desc");
        project.RejectScopeChange(other.Id).Status.Should().Be(ScopeChangeStatus.Rejected);
        var approveRejected = () => project.ApproveScopeChange(other.Id);
        approveRejected.Should().Throw<ValidationError>();

        var approveMissing = () => project.ApproveScopeChange(Guid.NewGuid());
        approveMissing.Should().Throw<NotFoundError>();
    }
}
