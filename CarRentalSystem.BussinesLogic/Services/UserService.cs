using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.BusinessLogic.Mapping;
using CarRentalSystem.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace CarRentalSystem.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var user = DtoToEntityMapper.Map(dto);

            if (dto.Password != dto.ConfirmPassword)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
            }

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return result;
            }

            var roleAssignResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleAssignResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return IdentityResult.Failed(new IdentityError { Description = "Failed to assign role to user." });
            }

            return IdentityResult.Success;
        }

        public async Task<SignInResult> LoginAsync(LoginDto dto)
        {
            return await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            {
                return result.Succeeded;
            }  
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            {
                return user != null && await _userManager.IsInRoleAsync(user, role);
            }
        }
    }
}
