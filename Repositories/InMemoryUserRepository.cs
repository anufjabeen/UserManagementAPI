using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

/// <summary>Thread-safe in-memory store (registered as a singleton). Data resets when the app restarts.</summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _users = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public InMemoryUserRepository()
    {
        // Seed data so GET /users returns something right away.
        Add(new User { Name = "Alice Johnson", Email = "alice@example.com", Age = 30, Department = "Engineering" });
        Add(new User { Name = "Bob Smith", Email = "bob@example.com", Age = 42, Department = "HR" });
    }

    public IReadOnlyList<User> GetAll()
    {
        lock (_lock)
        {
            // Return copies so callers cannot mutate the stored entities.
            return _users.Values.OrderBy(u => u.Id).Select(Clone).ToList();
        }
    }

    public User? GetById(int id)
    {
        lock (_lock)
        {
            return _users.TryGetValue(id, out var user) ? Clone(user) : null;
        }
    }

    public User Add(User user)
    {
        lock (_lock)
        {
            user.Id = _nextId++;
            _users[user.Id] = Clone(user);
            return Clone(user);
        }
    }

    public bool Update(User user)
    {
        lock (_lock)
        {
            if (!_users.ContainsKey(user.Id))
            {
                return false;
            }

            _users[user.Id] = Clone(user);
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            return _users.Remove(id);
        }
    }

    public bool EmailExists(string email, int? excludeId = null)
    {
        lock (_lock)
        {
            return _users.Values.Any(u =>
                u.Id != excludeId &&
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }
    }

    private static User Clone(User u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Age = u.Age,
        Department = u.Department
    };
}
