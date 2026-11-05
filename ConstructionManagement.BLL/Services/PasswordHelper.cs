using Microsoft.AspNetCore.Identity;

namespace ConstructionManagement.BLL.Services;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        var hasher = new PasswordHasher<object>();
        return hasher.HashPassword(new object(), password);
    }
}