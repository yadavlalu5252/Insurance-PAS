namespace Authentication.Application.DTOs;

public class RoleResponseDto
{
    public long role_id { get; set; }

    public string role_name { get; set; } = string.Empty;

    public string? description { get; set; }
}