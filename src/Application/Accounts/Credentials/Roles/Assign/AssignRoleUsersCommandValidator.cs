using FluentValidation;
using RoleValidation = DomainErrors.Role.Validation;

namespace RuanFa.FashionShop.Application.Accounts.Credentials.Roles.Assign;
internal class AssignRoleUsersCommandValidator : AbstractValidator<AssignRoleUsersCommand>
{
    public AssignRoleUsersCommandValidator()
    {
        // Validate UserIds
        RuleFor(x => x.UserIds)
            .NotNull()
            .WithMessage(RoleValidation.InvalidAssignedUsers.Description)
            .WithErrorCode(RoleValidation.InvalidAssignedUsers.Code)
            .Must(userIds => userIds != null && userIds.Count > 0)
            .WithMessage(RoleValidation.InvalidAssignedUsers.Description)
            .WithErrorCode(RoleValidation.InvalidAssignedUsers.Code);
    }
}
