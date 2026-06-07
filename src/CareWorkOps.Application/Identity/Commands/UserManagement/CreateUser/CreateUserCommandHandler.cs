using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.CreateUser;

public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserManagementService _userManagementService;

    public CreateUserCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _userManagementService.CreateUserAsync(
            request.TenantId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Roles,
            cancellationToken);
    }
}