using FluentValidation.TestHelper;
using ProductDemo.DTOs.Auth;
using ProductDemo.Validators.Auth;

namespace ProductDemo.Tests.Validators.Auth
{
    public class RegisterDtoValidatorTests
    {
        private readonly RegisterDtoValidator _validator = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Email_Is_NullOrEmpty(string? email)
        {
            var dto = new RegisterDto { Email = email, Password = "ValidPass123!" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid_Format()
        {
            var dto = new RegisterDto { Email = "invalid-email", Password = "ValidPass123!" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Password_Is_NullOrEmpty(string? password)
        {
            var dto = new RegisterDto { Email = "test@email.com", Password = password };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Theory]
        [InlineData("short")]                     // too short
        [InlineData("nocaps123!")]                // no uppercase
        [InlineData("NOLOWERCASE123!")]           // no lowercase
        [InlineData("NoNumbers!")]                // no number
        [InlineData("NoSpecial123")]              // no special char
        public void Should_Have_Error_When_Password_Does_Not_Meet_Rules(string password)
        {
            var dto = new RegisterDto { Email = "test@email.com", Password = password };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var dto = new RegisterDto { Email = "test@email.com", Password = "StrongPass123!" };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
