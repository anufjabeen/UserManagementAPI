using UserManagementAPI.Common;
using UserManagementAPI.DTOs;

namespace UserManagementAPI.Services;

public interface IUserService
{
    IReadOnlyList<UserResponse> GetAll();
    UserResponse? GetById(int id);
    ServiceResult<UserResponse> Create(CreateUserRequest request);
    ServiceResult<UserResponse> Update(int id, UpdateUserRequest request);
    ServiceResult<bool> Delete(int id);
}
