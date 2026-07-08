using System.ComponentModel.DataAnnotations;
using AIPM.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AIPM.SharedKernel.Tests.Configuration;

/// <summary>
/// Deployment profile configuration tests.
/// </summary>
public sealed class DeploymentProfileTests
{
    /// <summary>Each ADR-SAD-005 profile loads expected capability flags.</summary>
    [Theory]
    [InlineData("saas", true, true, true, false)]
    [InlineData("dedicated", true, false, true, true)]
    [InlineData("airgapped", false, false, false, true)]
    public void Profile_LoadsCapabilities(
        string profile,
        bool publicEgress,
        bool sharedPools,
        bool cloudLlm,
        bool dedicatedCompute)
    {
        var services = BuildServices(profile);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<DeploymentOptions>>().Value;

        options.Profile.Should().Be(profile);
        options.Capabilities.PublicEgress.Should().Be(publicEgress);
        options.Capabilities.SharedAgentPools.Should().Be(sharedPools);
        options.Capabilities.CloudLlmRoutes.Should().Be(cloudLlm);
        options.Capabilities.DedicatedCompute.Should().Be(dedicatedCompute);
    }

    /// <summary>Invalid profile name fails data annotation validation.</summary>
    [Fact]
    public void InvalidProfile_FailsValidation()
    {
        var options = new DeploymentOptions { Profile = "invalid-profile" };
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(options, new ValidationContext(options), results, validateAllProperties: true);

        valid.Should().BeFalse();
        results.Should().NotBeEmpty();
    }

    private static ServiceCollection BuildServices(string profile)
    {
        var contentRoot = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "..", "src", "AIPM.Host"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(contentRoot)
            .AddJsonFile("appsettings.json", optional: false)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Deployment:Profile"] = profile
            })
            .AddDeploymentProfile(new HostEnvironmentStub(), contentRoot)
            .Build();

        var services = new ServiceCollection();
        services.AddPlatformConfiguration(configuration);
        return services;
    }

    private sealed class HostEnvironmentStub : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Testing";
        public string ApplicationName { get; set; } = "AIPM.Host";
        public string ContentRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
