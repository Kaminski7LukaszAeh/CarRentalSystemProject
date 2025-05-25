using CarRentalSystem.BusinessLogic.DataTransferObjects;
using Microsoft.AspNetCore.Identity;

namespace CarRentalSystem.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<SignInResult> LoginAsync(LoginDto dto);
        Task LogoutAsync();
        Task<bool> DeleteAccountAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
    }
}
