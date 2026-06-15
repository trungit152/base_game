namespace trungnhd.puzzlecore.Common
{
    public readonly struct Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        private Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Ok() => new Result(true, null);
        public static Result Fail(string error) => new Result(false, error);

        public override string ToString() => IsSuccess ? "Ok" : "Fail(" + Error + ")";
    }
}
