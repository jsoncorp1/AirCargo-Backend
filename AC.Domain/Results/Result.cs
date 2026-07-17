using System.Diagnostics.CodeAnalysis;

namespace AC.Domain.Results;


public class Result
{
    public bool IsSuccess { get; private set; }
    public string Error { get; private set; }
    public string ErrorKey { get; private set; }

    public bool Failure
    {
        get { return !IsSuccess; }
    }

    protected Result(bool success, string error, string errorKey = "")
    {
        Contracts.Require(success || !string.IsNullOrEmpty(error));
        Contracts.Require(!success || string.IsNullOrEmpty(error));

        IsSuccess = success;
        Error = error;
        ErrorKey = errorKey!;
    }

    public static Result Fail(string message, string errorKey = "")
    {
        return new Result(false, message, errorKey!);
    }

    public static Result<T> Fail<T>(string message, string errorKey = "")
    {
        return new Result<T>(default!, false, message, errorKey!);
    }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result Combine(params Result[] results)
    {
        foreach (Result result in results)
        {
            if (result.Failure)
                return result;
        }

        return Success();
    }
}


public class Result<T> : Result
{
    private T _value = default!;

    public T Value
    {
        get
        {
            Contracts.Require(IsSuccess);

            return _value;
        }
        [param: AllowNull]
        private set { _value = value; }
    }

    protected internal Result(T value, bool success, string error, string errorKey = "")
        : base(success, error, errorKey)
    {
        Contracts.Require(value != null || !success);

        Value = value!;
    }
}

public static class Contracts
{
    public static void Require(bool precondition)
    {
        if (!precondition)
            throw new Exception();
    }
}