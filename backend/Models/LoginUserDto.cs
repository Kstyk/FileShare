using FluentValidation;

namespace backend.Models
{
    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Musisz podać adres e-mail.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Musisz podać hasło.");
        }
    }
}
