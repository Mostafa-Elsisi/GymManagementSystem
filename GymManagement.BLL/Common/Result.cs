namespace GymManagement.BLL.Common
{
    public sealed record Result(bool success, string? error = null, ResultKind Kind = ResultKind.OK)
    {
        public static Result OK() => new(true);

        public static Result Fail(string error, ResultKind Kind = ResultKind.Conflict) => new(false, error, Kind);

        public static Result NotFound(string error = "Not Found") => new(false, error, ResultKind.NotFound);

        public static Result Validation(string error) => new(false, error, ResultKind.ValidationFailed);
    }


    public sealed record Result<T>(bool success, T? value, string? error = null, ResultKind Kind = ResultKind.OK)
    {
        public static Result<T> OK(T value) => new(true, value);

        public static Result<T> Fail(string error, ResultKind Kind = ResultKind.Conflict) => new(false, default, error, Kind);

        public static Result<T> NotFound(string error = "Not Found") => new(false, default, error, ResultKind.NotFound);

        public static Result<T> Validation(string error) => new(false,default,error, ResultKind.ValidationFailed);
    }


}
