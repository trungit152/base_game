using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>
    /// Bộ nhận diện swipe thuần: theo dõi một dòng <see cref="IPointerSource"/>, ghi điểm bắt đầu khi
    /// con trỏ chạm, và khi nhả thì tính độ dời để suy ra hướng. Tất định và engine-agnostic, nên
    /// kiểm chứng được headless bằng một nguồn con trỏ giả.
    /// </summary>
    public sealed class SwipeRecognizer : ISwipeInput, IDisposable
    {
        private readonly IPointerSource _source;
        private readonly float _minDistance;

        private bool _tracking;
        private int _activeId;
        private float _startX;
        private float _startY;

        public event Action<SwipeDirection> Swiped;

        public SwipeRecognizer(IPointerSource source, SwipeConfig config)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _minDistance = config.MinSwipeDistance;
            _source.Pointer += OnPointer;
        }

        private void OnPointer(PointerSample sample)
        {
            switch (sample.Phase)
            {
                case PointerPhase.Began:
                    _tracking = true;
                    _activeId = sample.PointerId;
                    _startX = sample.X;
                    _startY = sample.Y;
                    break;

                case PointerPhase.Ended:
                    if (_tracking && sample.PointerId == _activeId)
                    {
                        _tracking = false;
                        var direction = Resolve(sample.X - _startX, sample.Y - _startY, _minDistance);
                        if (direction.HasValue)
                        {
                            Swiped?.Invoke(direction.Value);
                        }
                    }
                    break;

                case PointerPhase.Canceled:
                    if (sample.PointerId == _activeId)
                    {
                        _tracking = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// Phần toán thuần: từ độ dời (dx, dy) screen-space (y hướng LÊN) suy ra hướng vuốt, hoặc null
        /// nếu dưới ngưỡng. Trục có trị tuyệt đối lớn hơn thắng; hòa thì ưu tiên trục ngang (tất định).
        /// </summary>
        public static SwipeDirection? Resolve(float dx, float dy, float minDistance)
        {
            if (dx * dx + dy * dy < minDistance * minDistance)
            {
                return null;
            }

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                return dx >= 0f ? SwipeDirection.Right : SwipeDirection.Left;
            }

            return dy >= 0f ? SwipeDirection.Up : SwipeDirection.Down;
        }

        public void Dispose()
        {
            _source.Pointer -= OnPointer;
        }
    }
}
