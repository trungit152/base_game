namespace trungnhd.puzzlecore.Signals
{
    /// <summary>
    /// Interface đánh dấu (marker) cho mọi message phát trên <see cref="ISignalBus"/>.
    /// Một loại sự kiện mới chỉ là một <see cref="ISignal"/> mới — bus không bao giờ phải sửa (Open/Closed).
    /// </summary>
    public interface ISignal
    {
    }
}
