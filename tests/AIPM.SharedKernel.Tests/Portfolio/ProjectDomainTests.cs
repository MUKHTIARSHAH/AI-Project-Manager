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
}
