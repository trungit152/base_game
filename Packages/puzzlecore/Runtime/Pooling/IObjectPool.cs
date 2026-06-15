namespace trungnhd.puzzlecore.Pooling
{
    public interface IObjectPool<T> where T : class
    {
        /// <summary>Số phần tử đang nằm trong pool (sẵn sàng cho Get)</summary>
        int CountInactive { get; }

        /// <summary>Số phần tử đang được lấy ra ngoài (đã Get mà chưa Release)</summary>
        int CountActive { get; }

        /// <summary>Tổng số phần tử pool đang quản lý (active + inactive)</summary>
        int CountAll { get; }

        /// <summary>Lấy một phần tử (tái dùng nếu có, ngược lại tạo mới qua factory)</summary>
        T Get();

        /// <summary>Trả phần tử về pool để tái dùng</summary>
        void Release(T item);

        /// <summary>Xoá toàn bộ phần tử đang nằm trong pool (gọi onDestroy nếu có)</summary>
        void Clear();
    }
}
