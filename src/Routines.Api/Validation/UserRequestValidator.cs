using FluentValidation;
using Routines.Api.Contracts.Requests.Users;

namespace Routines.Api.Validation;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.DateOfBirth).NotEmpty();
    }
}
