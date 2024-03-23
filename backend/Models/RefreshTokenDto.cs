using FluentValidation;

namespace backend.Models
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }

    }

    public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Musisz podać token odświeżający.");
        }
    }
}
