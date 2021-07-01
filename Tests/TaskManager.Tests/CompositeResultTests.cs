using FluentAssertions;
using TaskManager.Contracts.Result;
using Xunit;

namespace TaskManager.Tests
{
    public class CompositeResultTests
    {
        [Fact]
        public void Result_AllFailed()
        {
            var result = Result<string>.Failure("data", ErrorType.Failed);
            var result2 = Result<string>.Failure("another data", ErrorType.Failed);
            var sut = new CompositeResult<string>(new[] { result, result2 });

            sut.IsSuccess.Should().BeFalse();
            sut.IsPartialFailure.Should().BeFalse();
        }

        [Fact]
        public void Result_AllSuccess()
        {
            var result = Result<string>.Success("data");
            var result2 = Result<string>.Success("another data");
            var sut = new CompositeResult<string>(new[] { result, result2 });

            sut.IsSuccess.Should().BeTrue();
            sut.IsPartialFailure.Should().BeFalse();
        }

        [Fact]
        public void Result_PartiallyFailed()
        {
            var result = Result<string>.Failure("data", ErrorType.NotFound);
            var result2 = Result<string>.Success("another data");
            var sut = new CompositeResult<string>(new[] { result, result2 });

            sut.IsSuccess.Should().BeFalse();
            sut.IsPartialFailure.Should().BeTrue();
        }
    }
}
