
namespace Authentication.Application.DTO;

public class UserResponseDto
{
    public long user_id { get; set; }
    public long employee_id { get; set; }
    public string username { get; set; } = string.Empty;
    public string? email { get; set; }
    public string? mobile { get; set; }
    public long role_id { get; set; }
    public string? role_name { get; set; }
    public long branch_id { get; set; }
    public string status { get; set; } = string.Empty;
    public bool two_factor_enabled { get; set; }
}
