using System.ComponentModel.DataAnnotations;

namespace Toolkit.Identity.DTOs.Request;

public class UserLoginRequest
{
    [Required(ErrorMessage = "The {0} field is required")]
    [EmailAddress(ErrorMessage = "The {0} field is invalid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The {0} field is required")]
    public string Senha { get; set; }
}