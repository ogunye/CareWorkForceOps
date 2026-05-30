using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.UserManagement.GetUserById;

public sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserManagementService _userManagementService;

    public GetUserByIdQueryHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManagementService.GetUserByIdAsync(
            request.TenantId,
            request.UserId,
            cancellationToken);

        return user is null
            ? Result<UserDto>.Failure(Error.Failure("User was not found."))
            : Result<UserDto>.Success(user);
    }
}