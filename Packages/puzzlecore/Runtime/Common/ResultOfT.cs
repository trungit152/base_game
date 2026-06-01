namespace trungnhd.puzzlecore.Common
{
    /// <summary>
    /// Kết quả của một thao tác có trả về giá trị khi thành công. Tương tự <see cref="Result"/>.
    /// Có chuyển đổi ngầm từ <typeparamref name="T"/> để trả về thành công gọn gàng hơn.
    /// </summary>
    public readonly struct Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        private Result(bool isSuccess, T value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Ok(T value) => new Result<T>(true, value, null);
        public static Result<T> Fail(string error) => new Result<T>(false, default, error);

        public static implicit operator Result<T>(T value) => Ok(value);

        public override string ToString() => IsSuccess ? "Ok(" + Value + ")" : "Fail(" + Error + ")";
    }
}
