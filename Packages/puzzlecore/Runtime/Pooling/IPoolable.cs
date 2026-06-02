namespace trungnhd.puzzlecore.Pooling
{
    /// <summary>
    /// (Opt-in) Cho phép một đối tượng tự xử lý vòng đời pool của mình. Khi pool quản lý một đối tượng
    /// hiện thực interface này, nó tự gọi <see cref="OnGet"/>/<see cref="OnRelease"/> — không cần truyền
    /// callback. Vẫn dùng callback của pool song song được (interface chạy trước, callback chạy sau).
    /// <para>
    /// Đây là một <i>capability</i> tuỳ chọn: type không hiện thực interface vẫn pool được bình thường
    /// (qua callback). Pool KHÔNG ràng buộc <c>where T : IPoolable</c> để không ép mọi type phải poolable.
    /// </para>
    /// </summary>
    public interface IPoolable
    {
        /// <summary>Gọi ngay sau khi đối tượng được lấy ra khỏi pool (chuẩn bị / đặt lại trạng thái).</summary>
        void OnGet();

        /// <summary>Gọi ngay trước khi đối tượng được trả về pool (dọn dẹp trạng thái).</summary>
        void OnRelease();
    }
}
