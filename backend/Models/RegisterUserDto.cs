using backend.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(FileShareDbContext dbContext)
        {

            // Add rule for email to be unique
            RuleFor(x => x.Email).EmailAddress().WithMessage("Niepoprawny format adresu e-mail")
                    .NotEmpty().WithMessage("Nie podano żadnej wartości.").Custom((value, context) =>
                    {
                        bool emailInUse = dbContext.Users.Any(u => u.Email == value);

                        if (emailInUse)
                        {
                            context.AddFailure("Email", "Ten email istnieje w naszej bazie.");
                        }
                    });

            RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("Musisz podać imię.")
                    .MaximumLength(50).WithMessage("Podana wartość nie może być dłuższa niż 50 znaków.");

            RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Musisz podać imię.")
                    .MaximumLength(50).WithMessage("Podana wartość nie może być dłuższa niż 50 znaków.");

            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Musisz ustalić hasło.")
                    .MinimumLength(8).WithMessage("Twoje hasło musi mieć minimum 8 znaków.");

            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Hasła nie są identyczne.");
        }
    }
}
