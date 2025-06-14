using FluentValidation;
using RoleValidation = DomainErrors.Role.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Users.Assign;
internal class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
    {
        // Validate RoleIds
        RuleFor(x => x.RoleIds)
            .NotNull()
            .WithMessage(RoleValidation.InvalidAssignedRoles.Description)
            .WithErrorCode(RoleValidation.InvalidAssignedRoles.Code)
            .Must(roleIds => roleIds != null && roleIds.Count > 0)
            .WithMessage(RoleValidation.InvalidAssignedRoles.Description)
            .WithErrorCode(RoleValidation.InvalidAssignedRoles.Code);
    }
}
