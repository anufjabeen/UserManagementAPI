namespace UserManagementAPI.Common;

public enum ResultStatus
{
    Success,
    NotFound,
    Conflict
}

/// <summary>
/// Lets the service layer report outcomes without knowing anything about HTTP.
/// The controller translates the status into a status code.
/// </summary>
public class ServiceResult<T>
{
    public ResultStatus Status { get; }
    public T? Value { get; }
    public string? Error { get; }

    private ServiceResult(ResultStatus status, T? value, string? error)
    {
        Status = status;
        Value = value;
        Error = error;
    }

    public bool IsSuccess => Status == ResultStatus.Success;

    public static ServiceResult<T> Ok(T value) => new(ResultStatus.Success, value, null);
    public static ServiceResult<T> NotFound(string error) => new(ResultStatus.NotFound, default, error);
    public static ServiceResult<T> Conflict(string error) => new(ResultStatus.Conflict, default, error);
}
