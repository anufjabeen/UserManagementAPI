using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

/// <summary>Data access contract. Swap the implementation (e.g. EF Core + SQL Server) without touching the rest of the app.</summary>
public interface IUserRepository
{
    IReadOnlyList<User> GetAll();
    User? GetById(int id);
    User Add(User user);
    bool Update(User user);
    bool Delete(int id);
    bool EmailExists(string email, int? excludeId = null);
}
