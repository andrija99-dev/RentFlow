using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Exceptions;

namespace RentFlow.Infrastructure.Identity;

/// <inheritdoc />
internal sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    /// <inheritdoc />
    public async Task<AuthenticatedUser> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        AccountRole role,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            throw new ConflictException($"An account with email '{email}' already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            ThrowForIdentityErrors(createResult);
        }

        var roleName = role.ToString();
        var roleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            ThrowForIdentityErrors(roleResult);
        }

        return new AuthenticatedUser(user.Id, email, [roleName]);
    }

    /// <inheritdoc />
    public async Task<AuthenticatedUser> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            throw new AuthenticationException("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new AuthenticatedUser(user.Id, user.Email!, [.. roles]);
    }

    private static void ThrowForIdentityErrors(IdentityResult result)
    {
        var duplicate = result.Errors.FirstOrDefault(e =>
            e.Code is "DuplicateUserName" or "DuplicateEmail");
        if (duplicate is not null)
        {
            throw new ConflictException(duplicate.Description);
        }

        var failures = result.Errors
            .Select(e => new ValidationFailure(nameof(IIdentityService), e.Description) { ErrorCode = e.Code })
            .ToList();
        throw new ValidationException(failures);
    }
}
