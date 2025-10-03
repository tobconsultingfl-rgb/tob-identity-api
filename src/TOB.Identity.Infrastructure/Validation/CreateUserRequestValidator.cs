using FluentValidation;
using TOB.Identity.Domain.Requests;
using TOB.Identity.Infrastructure.Repositories;

namespace TOB.Identity.Infrastructure.Validation;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly IUserRepository _userRepository;

    public CreateUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(user => user.TenantId)
                .NotEmpty()
                .WithMessage("TenantId is required");

        RuleFor(user => user.Roles)
            .NotEmpty()
            .WithMessage("At least one role must be added");

        RuleFor(user => user.FirstName)
                .NotEmpty()
                .WithMessage("FirstName is required");

        RuleFor(user => user.LastName)
                .NotEmpty()
                .WithMessage("LastName is required");

        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid Email")
            .Must(UserNameIsUnique);

        RuleFor(user => user.MobilePhone)
            .NotEmpty()
            .WithMessage("MobilePhone is required");
    }

    private bool UserNameIsUnique(string userName)
    {
        var userExists = _userRepository.DoesUsernameExistsAsync(userName);

        return false == userExists.Result;
    }
}
