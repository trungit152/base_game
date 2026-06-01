namespace trungnhd.puzzlecore.Common
{
    /// <summary>
    /// Kết quả của một thao tác có thể thành công hoặc thất bại mà <i>không</i> trả về giá trị.
    /// Dùng thay cho exception trong các luồng điều khiển <i>được dự kiến</i> (guard từ chối,
    /// thiếu booster trong kho, action không hợp lệ) để chỗ gọi luôn tường minh và tất định.
    /// </summary>
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
