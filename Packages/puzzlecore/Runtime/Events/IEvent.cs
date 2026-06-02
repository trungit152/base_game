namespace trungnhd.puzzlecore.Events
{
    /// <summary>
    /// Marker (interface đánh dấu) cho mọi message phát trên <see cref="IEventBus"/>.
    /// Một loại sự kiện mới chỉ là một <see cref="IEvent"/> mới — bus không bao giờ phải sửa (Open/Closed).
    /// </summary>
    public interface IEvent
    {
    }
}
