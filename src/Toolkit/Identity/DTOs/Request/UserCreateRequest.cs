using Toolkit;
using System.ComponentModel.DataAnnotations;

namespace Toolkit.Identity.DTOs.Request;

public class UserCreateRequest
{
    [Required(ErrorMessage = Constants.RequiredField)]
    [EmailAddress(ErrorMessage = Constants.InvalidField)]
    public string Email { get; set; }

    [Required(ErrorMessage = Constants.RequiredField)]
    [StringLength(50, ErrorMessage = Constants.InvalidStringLengthField, MinimumLength = Constants.MinSizePassword)]
    public string Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = Constants.NotMatchPasswordField)]
    public string ConfirmationPassword { get; set; }
}