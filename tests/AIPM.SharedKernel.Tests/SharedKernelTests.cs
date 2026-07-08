using AIPM.SharedKernel.Ids;
using AIPM.SharedKernel.Results;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests;

/// <summary>
/// Tests for shared kernel primitives.
/// </summary>
public sealed class SharedKernelTests
{
    /// <summary>TenantId generates unique values.</summary>
    [Fact]
    public void TenantId_New_GeneratesUniqueValues()
    {
        var a = TenantId.New();
        var b = TenantId.New();
        a.Should().NotBe(b);
    }

    /// <summary>Result success has no error.</summary>
    [Fact]
    public void Result_Success_HasNoError()
    {
        var result = Result.Success();
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    /// <summary>Result failure contains error message.</summary>
    [Fact]
    public void Result_Failure_ContainsError()
    {
        var result = Result.Failure("failed");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("failed");
    }

    /// <summary>Result with value returns payload.</summary>
    [Fact]
    public void ResultT_Success_ReturnsValue()
    {
        var result = Result<int>.Success(42);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }
}
