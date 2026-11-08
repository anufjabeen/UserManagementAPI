using UserManagementAPI.Common;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Services;

/// <summary>Business rules (unique email, existence checks) and entity/DTO mapping live here.</summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<UserResponse> GetAll() =>
        _repository.GetAll().Select(ToResponse).ToList();

    public UserResponse? GetById(int id)
    {
        var user = _repository.GetById(id);
        return user is null ? null : ToResponse(user);
    }

    public ServiceResult<UserResponse> Create(CreateUserRequest request)
    {
        var email = request.Email!.Trim();

        if (_repository.EmailExists(email))
        {
            return ServiceResult<UserResponse>.Conflict("A user with this email already exists.");
        }

        var created = _repository.Add(new User
        {
            Name = request.Name!.Trim(),
            Email = email,
            Age = request.Age,
            Department = request.Department!.Trim()
        });

        return ServiceResult<UserResponse>.Ok(ToResponse(created));
    }

    public ServiceResult<UserResponse> Update(int id, UpdateUserRequest request)
    {
        var existing = _repository.GetById(id);
        if (existing is null)
        {
            return ServiceResult<UserResponse>.NotFound($"User with id {id} was not found.");
        }

        var email = request.Email!.Trim();
        if (_repository.EmailExists(email, excludeId: id))
        {
            return ServiceResult<UserResponse>.Conflict("Another user with this email already exists.");
        }

        existing.Name = request.Name!.Trim();
        existing.Email = email;
        existing.Age = request.Age;
        existing.Department = request.Department!.Trim();
        _repository.Update(existing);

        return ServiceResult<UserResponse>.Ok(ToResponse(existing));
    }

    public ServiceResult<bool> Delete(int id) =>
        _repository.Delete(id)
            ? ServiceResult<bool>.Ok(true)
            : ServiceResult<bool>.NotFound($"User with id {id} was not found.");

    private static UserResponse ToResponse(User u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Age = u.Age,
        Department = u.Department
    };
}
