using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ProductDemo.Helpers
{
    public static class ValidationHelper
    {
        public static async Task<IActionResult?> ValidateAndFormatAsync<T>(IValidator<T> validator, T dto)
        {
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<string>.FailResponse(errors, "Validation failed");
                return new BadRequestObjectResult(response);
            }

            return null;
        }
    }

}
