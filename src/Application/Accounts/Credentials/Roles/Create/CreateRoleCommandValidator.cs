using FluentValidation;
using RoleValidation = DomainErrors.Role.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Create;

internal sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        // Validate Role Name
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(RoleValidation.InvalidName.Description)
            .WithErrorCode(RoleValidation.InvalidName.Code)
            .MinimumLength(3)
            .WithMessage(RoleValidation.NameTooShort.Description)
            .WithErrorCode(RoleValidation.NameTooShort.Code)
            .MaximumLength(50)
            .WithMessage(RoleValidation.NameTooLong.Description)
            .WithErrorCode(RoleValidation.NameTooLong.Code)
            .Matches(@"^[A-Za-z0-9_]+$") // letters, numbers, underscores only
            .WithMessage(RoleValidation.InvalidNameFormat.Description)
            .WithErrorCode(RoleValidation.InvalidNameFormat.Code);

        // Validate Permissions
        RuleFor(x => x.Permissions)
            .NotNull()
            .WithMessage(RoleValidation.InvalidPermissions.Description)
            .WithErrorCode(RoleValidation.InvalidPermissions.Code)
            .Must(p => p != null && p.All(permission => !string.IsNullOrWhiteSpace(permission)))
            .WithMessage(RoleValidation.InvalidPermissions.Description)
            .WithErrorCode(RoleValidation.InvalidPermissions.Code);
    }
}
