using FluentValidation;

namespace HRSupport.Application.Features.Employees.Commans
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required").Matches(@"^\d{10}$").WithMessage("Phone must be 10 digits");
            RuleFor(x => x.CardID).GreaterThan(0).WithMessage("CardID must be greater than 0");
            RuleFor(x => x.BirthDate).LessThan(DateTime.Now).WithMessage("BirthDate must be in the past");
            RuleFor(x => x.StartDate).LessThanOrEqualTo(DateTime.Now).WithMessage("StartDate cannot be in the future");
            RuleFor(x => x.Department).IsInEnum().WithMessage("Invalid department");
            RuleFor(x => x.Roles).IsInEnum().WithMessage("Invalid role");
        }
    }
}
