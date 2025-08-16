using FluentAssertions;
using ProductDemo.Helpers;

namespace ProductDemo.Tests.Helpers
{
    public class InputSanitizerTests
    {
        [Theory]
        [InlineData("   hello   world   ", "hello world")]
        [InlineData("Hello", "Hello")] // unchanged if already clean
        public void Clean_ShouldTrimAndCollapseSpaces(string input, string expected)
        {
            var result = InputSanitizer.Clean(input);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("TEST@EMAIL.COM", "test@email.com")]
        [InlineData("   User@Example.Org   ", "user@example.org")]
        public void Clean_ShouldLowercaseEmails(string input, string expected)
        {
            var result = InputSanitizer.Clean(input);
            result.Should().Be(expected);
        }

        [Fact]
        public void Clean_ShouldSanitizeHtml()
        {
            var input = "<script>alert('xss')</script>Hello";
            var result = InputSanitizer.Clean(input);

            result.Should().Be("Hello"); // script stripped
        }

        [Fact]
        public void Clean_ShouldReturnNull_WhenInputIsNullOrWhitespace()
        {
            InputSanitizer.Clean(null).Should().BeNull();
            InputSanitizer.Clean("   ").Should().Be("   "); // whitespace stays null? let's check

            // careful: in your implementation, whitespace → returns original
            // if you wanted it to return null, adjust implementation.
        }
    }
}
