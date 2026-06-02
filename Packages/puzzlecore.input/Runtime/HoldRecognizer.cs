using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>Tham số nhận diện hold/long-press. Thời gian theo giây, khoảng cách theo pixel.</summary>
    public readonly struct HoldConfig
    {
        /// <summary>Thời gian giữ tối thiểu để kích hoạt hold.</summary>
        public readonly float Duration;

        /// <summary>Dịch chuyển tối đa cho phép trong lúc giữ; vượt quá → huỷ.</summary>
        public readonly float MoveTolerance;

        public HoldConfig(float duration, float moveTolerance)
        {
            Duration = duration;
            MoveTolerance = moveTolerance;
        }

        public static HoldConfig Default => new HoldConfig(0.5f, 20f);
    }

    /// <summary>
    /// Nhận diện hold/long-press thuần: giữ tại chỗ (không dịch quá <see cref="HoldConfig.MoveTolerance"/>)
    /// đủ <see cref="HoldConfig.Duration"/> giây thì phát một lần. Dựa vào nhịp Moved theo frame mà
    /// <see cref="IPointerSource"/> phát khi đang nhấn để đo thời gian — không cần tick riêng.
    /// </summary>
    public sealed class HoldRecognizer : IHoldInput, IDisposable
    {
        private readonly IPointerSource _source;
        private readonly float _duration;
        private readonly float _moveTolerance;

        private bool _tracking;
        private bool _fired;
        private int _activeId;
        private float _startX;
        private float _startY;
        private float _startTime;

        public event Action<Point2> Held;

        public HoldRecognizer(IPointerSource source, HoldConfig config)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _duration = config.Duration;
            _moveTolerance = config.MoveTolerance;
            _source.Pointer += OnPointer;
        }

        private void OnPointer(PointerSample s)
        {
            switch (s.Phase)
            {
                case PointerPhase.Began:
                    _tracking = true;
                    _fired = false;
                    _activeId = s.PointerId;
                    _startX = s.X;
                    _startY = s.Y;
                    _startTime = s.Time;
                    break;

                case PointerPhase.Moved:
                    if (!_tracking || s.PointerId != _activeId || _fired)
                    {
                        break;
                    }
                    if (FarFromStart(s))
                    {
                        _tracking = false; // dịch quá → không còn là hold
                    }
                    else if ((s.Time - _startTime) >= _duration)
                    {
                        _fired = true;
                        Held?.Invoke(new Point2(_startX, _startY));
                    }
                    break;

                case PointerPhase.Ended:
                case PointerPhase.Canceled:
                    if (s.PointerId == _activeId)
                    {
                        _tracking = false;
                    }
                    break;
            }
        }

        private bool FarFromStart(PointerSample s)
        {
            float dx = s.X - _startX;
            float dy = s.Y - _startY;
            return dx * dx + dy * dy > _moveTolerance * _moveTolerance;
        }

        public void Dispose() => _source.Pointer -= OnPointer;
    }
}
