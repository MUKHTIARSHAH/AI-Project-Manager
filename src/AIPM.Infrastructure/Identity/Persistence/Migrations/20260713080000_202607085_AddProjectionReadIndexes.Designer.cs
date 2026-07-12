using AIPM.Infrastructure.Identity.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

[DbContext(typeof(IdentityDbContext))]
[Migration("20260713080000_202607085_AddProjectionReadIndexes")]
partial class _202607085_AddProjectionReadIndexes
{
}
