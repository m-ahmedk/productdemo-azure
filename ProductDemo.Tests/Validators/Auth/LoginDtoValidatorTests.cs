using FluentValidation.TestHelper;
using ProductDemo.DTOs.Auth;
using ProductDemo.Validators.Auth;

namespace ProductDemo.Tests.Validators.Auth
{
    public class LoginDtoValidatorTests
    {
        private readonly LoginDtoValidator _validator = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Email_Is_NullOrEmpty(string? email)
        {
            var dto = new LoginDto { Email = email, Password = "ValidPass123!" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid_Format()
        {
            var dto = new LoginDto { Email = "not-an-email", Password = "ValidPass123!" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Password_Is_NullOrEmpty(string? password)
        {
            var dto = new LoginDto { Email = "test@email.com", Password = password };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var dto = new LoginDto { Email = "test@email.com", Password = "ValidPass123!" };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
