namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>
    /// Metadata tĩnh mô tả một booster (dữ liệu, không phải điều phối). Có thể được hiện thực bằng
    /// một ScriptableObject trong Unity hoặc một class thuần khi chạy headless.
    /// </summary>
    public interface IBoosterDefinition
    {
        string Id { get; }
        string DisplayName { get; }

        /// <summary>Hành vi được áp dụng khi booster này được kích hoạt.</summary>
        IBoosterEffect Effect { get; }
    }
}
