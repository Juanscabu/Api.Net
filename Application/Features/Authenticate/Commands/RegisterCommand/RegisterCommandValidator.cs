using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Authenticate.Commands.RegisterCommand
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(p => p.Nombre)
               .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
               .MaximumLength(80).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.Apellido)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(80).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.Email)
              .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
              .EmailAddress().WithMessage("{PropertyName} should be an email.")
              .MaximumLength(100).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.UserName)
             .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
             .MaximumLength(10).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.Password)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
            .MaximumLength(15).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
            .MaximumLength(15).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.")
            .Equal(p => p.Password).WithMessage("{PropertyName} should be equal as password."); 

        }
    }
}
