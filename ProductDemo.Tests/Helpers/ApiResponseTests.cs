using FluentAssertions;
using ProductDemo.Helpers;

namespace ProductDemo.Tests.Helpers
{
    public class ApiResponseTests
    {
        [Fact]
        public void SuccessResponse_ShouldSetSuccessTrue_AndData()
        {
            var response = ApiResponse<string>.SuccessResponse("Hello", "Done");

            response.Success.Should().BeTrue();
            response.Data.Should().Be("Hello");
            response.Message.Should().Be("Done");
            response.Errors.Should().BeNull();
        }

        [Fact]
        public void FailResponse_ShouldSetSuccessFalse_AndSingleError()
        {
            var response = ApiResponse<string>.FailResponse("Bad request");

            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle("Bad request");
        }

        [Fact]
        public void FailResponse_ShouldHandleMultipleErrors()
        {
            var errors = new List<string> { "Error1", "Error2" };
            var response = ApiResponse<string>.FailResponse(errors);

            response.Success.Should().BeFalse();
            response.Errors.Should().Contain(errors);
        }
    }
}
