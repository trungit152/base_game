using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>Tham số nhận diện tap/click. Khoảng cách theo pixel, thời lượng theo giây.</summary>
    public readonly struct TapConfig
    {
        /// <summary>Độ dịch chuyển tối đa vẫn coi là tap; vượt quá → không phải tap.</summary>
        public readonly float MaxDistance;

        /// <summary>Thời lượng tối đa từ chạm tới nhả vẫn coi là tap.</summary>
        public readonly float MaxDuration;

        public TapConfig(float maxDistance, float maxDuration)
        {
            MaxDistance = maxDistance;
            MaxDuration = maxDuration;
        }

        public static TapConfig Default => new TapConfig(20f, 0.3f);
    }

    /// <summary>
    /// Nhận diện tap/click thuần: chạm rồi nhả mà không dịch quá <see cref="TapConfig.MaxDistance"/> và
    /// trong vòng <see cref="TapConfig.MaxDuration"/>. Tiêu thụ một dòng <see cref="IPointerSource"/>.
    /// </summary>
    public sealed class TapRecognizer : ITapInput, IDisposable
    {
        private readonly IPointerSource _source;
        private readonly float _maxDistance;
        private readonly float _maxDuration;

        private bool _tracking;
        private bool _movedTooFar;
        private int _activeId;
        private float _startX;
        private float _startY;
        private float _startTime;

        public event Action<Point2> Tapped;

        public TapRecognizer(IPointerSource source, TapConfig config)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _maxDistance = config.MaxDistance;
            _maxDuration = config.MaxDuration;
            _source.Pointer += OnPointer;
        }

        private void OnPointer(PointerSample s)
        {
            switch (s.Phase)
            {
                case PointerPhase.Began:
                    _tracking = true;
                    _movedTooFar = false;
                    _activeId = s.PointerId;
                    _startX = s.X;
                    _startY = s.Y;
                    _startTime = s.Time;
                    break;

                case PointerPhase.Moved:
                    if (_tracking && s.PointerId == _activeId && !_movedTooFar && FarFromStart(s))
                    {
                        _movedTooFar = true;
                    }
                    break;

                case PointerPhase.Ended:
                    if (_tracking && s.PointerId == _activeId)
                    {
                        _tracking = false;
                        bool inTime = (s.Time - _startTime) <= _maxDuration;
                        if (!_movedTooFar && !FarFromStart(s) && inTime)
                        {
                            Tapped?.Invoke(new Point2(s.X, s.Y));
                        }
                    }
                    break;

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
            return dx * dx + dy * dy > _maxDistance * _maxDistance;
        }

        public void Dispose() => _source.Pointer -= OnPointer;
    }
}
