namespace CareWorkOps.Api.IntegrationTests.Infrastructure;

public sealed class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public sealed class RoleDto
{
    public Guid Id { get; set; }    
    public string Name { get; set; } = string.Empty;
}