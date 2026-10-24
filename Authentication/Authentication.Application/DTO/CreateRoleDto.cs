using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.DTOs;

public class CreateRoleDto
{
    [Required]
    [StringLength(50)]
    public string role_name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? description { get; set; }
}