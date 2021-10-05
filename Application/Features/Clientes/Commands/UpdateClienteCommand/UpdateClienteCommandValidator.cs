using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Commands.UpdateClienteCommand
{
    public class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
    {
        public UpdateClienteCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.");

            RuleFor(p => p.Nombre)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(80).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.Apellido)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(80).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.FechaNacimiento)
               .NotEmpty().WithMessage("BirthDate cannot be empty.");

            RuleFor(p => p.Telefono)
               .MaximumLength(9).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.Email)
               .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
               .EmailAddress().WithMessage("{PropertyName} should be an email.")
               .MaximumLength(100).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

            RuleFor(p => p.Direccion)
              .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
              .MaximumLength(120).WithMessage("{PropertyName} cannot have more lenght than {MaxLenght}.");

        }
    }
}
