namespace trungnhd.puzzlecore.input
{
    /// <summary>
    /// Một mẫu con trỏ tại một thời điểm: vị trí screen-space (pixel, y hướng LÊN), pha, mốc thời gian
    /// (giây) và id con trỏ. Thuần C# — không phụ thuộc engine; adapter Unity bơm dữ liệu vào dưới dạng
    /// này để mọi recognizer (swipe, tap, drag, hold) dùng chung một nguồn.
    /// </summary>
    public readonly struct PointerSample
    {
        public readonly float X;
        public readonly float Y;
        public readonly PointerPhase Phase;

        /// <summary>Mốc thời gian (giây) của mẫu — dùng cho tap (thời lượng tối đa) và hold (ngưỡng giữ).</summary>
        public readonly float Time;
        public readonly int PointerId;

        public PointerSample(float x, float y, PointerPhase phase, float time = 0f, int pointerId = 0)
        {
            X = x;
            Y = y;
            Phase = phase;
            Time = time;
            PointerId = pointerId;
        }
    }
}
