using System.ComponentModel.DataAnnotations;

namespace ElectronicWallet.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Неверный логин или пароль!")]
    public string Identifier { get; set; }
    [Required(ErrorMessage = "Требуется пароль!")]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}