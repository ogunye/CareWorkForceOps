using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.UserManagement.GetUsers;

public sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, Result<IReadOnlyCollection<UserDto>>>
{
    private readonly IUserManagementService _userManagementService;

    public GetUsersQueryHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result<IReadOnlyCollection<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userManagementService.GetUsersAsync(
            request.TenantId,
            cancellationToken);

        return Result<IReadOnlyCollection<UserDto>>.Success(users);
    }
}