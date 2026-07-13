using AIPM.Domain.Requirements;
using AIPM.SharedKernel.Errors;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Requirements;

public sealed class RequirementDomainTests
{
    [Fact]
    public void Intake_CreatesDraftWithParsedTrue_AndRaisesIntakenEvent()
    {
        var requirement = RequirementAggregate.Intake(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Login",
            "Users can authenticate with email.");

        requirement.Status.Should().Be(RequirementStatus.Draft);
        requirement.Parsed.Should().BeTrue();
        requirement.DomainEvents.Should().ContainSingle(x => x is RequirementIntakenDomainEvent);
        var domainEvent = requirement.DomainEvents.OfType<RequirementIntakenDomainEvent>().Single();
        domainEvent.Status.Should().Be(nameof(RequirementStatus.Draft));
        domainEvent.Parsed.Should().BeTrue();
    }

    [Fact]
    public void Intake_DoesNotSetUnderReview_WhenParsed()
    {
        var requirement = RequirementAggregate.Intake(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Title",
            "Statement body");

        requirement.Parsed.Should().BeTrue();
        requirement.Status.Should().Be(RequirementStatus.Draft);
        requirement.Status.Should().NotBe(RequirementStatus.UnderReview);
    }

    [Fact]
    public void Intake_AcceptsOptionalAcceptanceCriteriaAndDocumentMetadata()
    {
        var metadata = DocumentMetadata.CreateOptional("Spec.pdf", "application/pdf", "docs/spec.pdf");
        var requirement = RequirementAggregate.Intake(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Payments",
            "Support card payments.",
            ["Given a card, when charged, then receipt", "Refund within 30 days"],
            metadata);

        requirement.AcceptanceCriteria.Should().HaveCount(2);
        requirement.AcceptanceCriteria[0].SortOrder.Should().Be(0);
        requirement.AcceptanceCriteria[1].Statement.Should().Contain("Refund");
        requirement.DocumentMetadata.Should().NotBeNull();
        requirement.DocumentMetadata!.Title.Should().Be("Spec.pdf");
        requirement.DocumentMetadata.ContentType.Should().Be("application/pdf");
        requirement.DocumentMetadata.UriOrName.Should().Be("docs/spec.pdf");
    }

    [Fact]
    public void Intake_RequiresTenantProjectTitleAndStatement()
    {
        var act = () => RequirementAggregate.Intake(Guid.Empty, Guid.NewGuid(), "T", "S");
        act.Should().Throw<ValidationError>().WithMessage("*TenantId*");

        act = () => RequirementAggregate.Intake(Guid.NewGuid(), Guid.Empty, "T", "S");
        act.Should().Throw<ValidationError>().WithMessage("*ProjectId*");

        act = () => RequirementAggregate.Intake(Guid.NewGuid(), Guid.NewGuid(), " ", "S");
        act.Should().Throw<ValidationError>().WithMessage("*title*");

        act = () => RequirementAggregate.Intake(Guid.NewGuid(), Guid.NewGuid(), "T", " ");
        act.Should().Throw<ValidationError>().WithMessage("*statement*");
    }

    [Fact]
    public void DocumentMetadata_CreateOptional_ReturnsNull_WhenEmpty()
    {
        DocumentMetadata.CreateOptional(null, " ", null).Should().BeNull();
    }
}
