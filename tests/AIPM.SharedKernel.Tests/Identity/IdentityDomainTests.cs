using AIPM.Domain.Identity;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Identity;

public sealed class IdentityDomainTests
{
    [Fact]
    public void Tenant_Provision_RaisesProvisionedEvent()
    {
        var tenant = Tenant.Provision("Acme");

        tenant.Status.Should().Be(TenantStatus.Active);
        tenant.DomainEvents.Should().ContainSingle(x => x is TenantProvisionedDomainEvent);
    }

    [Fact]
    public void User_AssignRole_PreventsDuplicates()
    {
        var user = User.Create(Guid.NewGuid(), "admin@acme.test");
        var roleId = Guid.NewGuid();

        user.AssignRole(roleId);
        user.AssignRole(roleId);

        user.RoleAssignments.Should().HaveCount(1);
    }

    [Fact]
    public void Role_AssignPermission_PreventsDuplicates()
    {
        var role = Role.Create(Guid.NewGuid(), "Admin");

        role.AssignPermission(new Permission("identity.users.write"));
        role.AssignPermission(new Permission("identity.users.write"));

        role.PermissionAssignments.Should().HaveCount(1);
    }
}
