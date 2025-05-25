namespace CarRentalSystem.BusinessLogic.Common
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string Error { get; protected set; }
        public string Message { get; protected set; }

        public static Result Success(string message = "") =>
            new Result { IsSuccess = true, Message = message };

        public static Result Failure(string error) =>
            new Result { IsSuccess = false, Error = error };
    }
    public class Result<T> : Result
    {
        public T Value { get; private set; }

        public static Result<T> Success(T value, string message = "") =>
            new Result<T> { IsSuccess = true, Value = value, Message = message };

        public static Result<T> Failure(string error) =>
            new Result<T> { IsSuccess = false, Error = error };
    }

}
