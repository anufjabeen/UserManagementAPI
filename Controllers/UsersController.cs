using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Common;
using UserManagementAPI.DTOs;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

/// <summary>
/// HTTP layer only: [ApiController] validates the DTOs (data annotations) and returns 400 automatically.
/// </summary>
[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<UserResponse>> GetAll() => Ok(_service.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<UserResponse> GetById(int id)
    {
        var user = _service.GetById(id);
        return user is null
            ? NotFound(new { error = $"User with id {id} was not found." })
            : Ok(user);
    }

    [HttpPost]
    public ActionResult<UserResponse> Create(CreateUserRequest request)
    {
        var result = _service.Create(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : ToError(result);
    }

    [HttpPut("{id:int}")]
    public ActionResult<UserResponse> Update(int id, UpdateUserRequest request)
    {
        var result = _service.Update(id, request);
        return result.IsSuccess ? Ok(result.Value) : ToError(result);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var result = _service.Delete(id);
        return result.IsSuccess ? NoContent() : ToError(result);
    }

    private ActionResult ToError<T>(ServiceResult<T> result) => result.Status switch
    {
        ResultStatus.NotFound => NotFound(new { error = result.Error }),
        ResultStatus.Conflict => Conflict(new { error = result.Error }),
        _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." })
    };
}
