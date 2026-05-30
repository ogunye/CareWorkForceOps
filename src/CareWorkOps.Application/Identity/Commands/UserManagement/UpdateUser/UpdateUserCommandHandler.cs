using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.UpdateUser;

public sealed class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUserManagementService _userManagementService;

    public UpdateUserCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManagementService.UpdateUserAsync(
            request.TenantId,
            request.UserId,
            request.FirstName,
            request.LastName,
            cancellationToken);

        return user is null
            ? Result<UserDto>.Failure(Error.Failure("Unable to update user."))
            : Result<UserDto>.Success(user);
    }
}