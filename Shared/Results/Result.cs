using System.Diagnostics.CodeAnalysis;

namespace FriendStuff.Shared.Results;

public class Result
{
    [MemberNotNullWhen(true, nameof(SuccessMessage))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; init; }
    public string? SuccessMessage { get; init; }
    public Error? Error { get; init; }


    private Result(bool isSuccess, string? successMessage, Error? error)
    {
        IsSuccess = isSuccess;
        SuccessMessage = successMessage;
        Error = error;
    }

    public static Result Success(string? successMessage = null) => new(true, successMessage, null);
    public static Result Failure(Error error) => new(false, null, error);
}

public class Result<T>
{
    [MemberNotNullWhen(true, nameof(SuccessMessage))]
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; init; }
    public string? SuccessMessage { get; init; }
    public Error? Error { get; init; }
    public T? Value { get; set; }


    private Result(bool isSuccess, T? value, string? successMessage, Error? error)
    {
        IsSuccess = isSuccess;
        SuccessMessage = successMessage;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T? value = default, string? successMessage = null) => new(true, value, successMessage, null);
    public static Result<T> Failure(Error error) => new(false, default, null, error);
}