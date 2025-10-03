using FluentValidation;
using TOB.Identity.Domain.Requests;
using TOB.Identity.Infrastructure.Repositories;

namespace TOB.Identity.Infrastructure.Validation;

public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;

    public CreateTenantRequestValidator(ITenantRepository tenantRepository, IUserRepository userRepository)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;

        RuleFor(Tenant => Tenant.TenantName)
                .NotEmpty()
                .WithMessage("TenantName name is required")
                .Must(TenantNameIsUnique)
                .WithMessage("TenantName already exists");

        RuleFor(Tenant => Tenant.TenantAddress1)
            .NotEmpty()
            .WithMessage("TenantAddress1 is required");

        RuleFor(Tenant => Tenant.TenantCity)
            .NotEmpty()
            .WithMessage("TenantCity is required");

        RuleFor(Tenant => Tenant.TenantZip)
            .NotEmpty()
            .WithMessage("TenantZip is required");

        RuleFor(Tenant => Tenant.TenantPhoneNumber)
            .NotEmpty()
            .WithMessage("TenantPhoneNumber Number is Required");

        RuleFor(Tenant => Tenant.ContactFirstName)
            .NotEmpty()
            .WithMessage("FirstName is required");

        RuleFor(Tenant => Tenant.ContactLastName)
            .NotEmpty()
            .WithMessage("LastName is required");

        RuleFor(Tenant => Tenant.ContactEmail)
            .NotEmpty()
            .WithMessage("ContactEmail is required")
            .EmailAddress()
            .WithMessage("ContactEmail is invalid Email")
            .Must(UserNameIsUnique)
            .WithMessage("A user with this email address already exists");

        RuleFor(Tenant => Tenant.ContactMobilePhone)
            .NotEmpty()
            .WithMessage("ContactMobilePhone Number is Required");

    }

    private bool TenantNameIsUnique(string TenantName)
    {
        var tenantExists = _tenantRepository.DoesTenantExistsAsync(TenantName);

        return false == tenantExists.Result;
    }

    private bool UserNameIsUnique(string userName)
    {
        var userExists = _userRepository.DoesUsernameExistsAsync(userName);

        return false == userExists.Result;
    }
}
