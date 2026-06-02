using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>Tham số nhận diện drag. Ngưỡng theo pixel.</summary>
    public readonly struct DragConfig
    {
        /// <summary>Quãng đường tối thiểu từ điểm chạm để bắt đầu coi là kéo.</summary>
        public readonly float Threshold;

        public DragConfig(float threshold)
        {
            Threshold = threshold;
        }

        public static DragConfig Default => new DragConfig(10f);
    }

    /// <summary>
    /// Nhận diện drag thuần: sau khi con trỏ dịch quá <see cref="DragConfig.Threshold"/> kể từ điểm
    /// chạm, phát <see cref="DragPhase.Begin"/>, các <see cref="DragPhase.Move"/> kèm độ dời, rồi
    /// <see cref="DragPhase.End"/> khi nhả. Tiêu thụ một dòng <see cref="IPointerSource"/>.
    /// </summary>
    public sealed class DragRecognizer : IDragInput, IDisposable
    {
        private readonly IPointerSource _source;
        private readonly float _threshold;

        private bool _tracking;
        private bool _dragging;
        private int _activeId;
        private float _startX;
        private float _startY;
        private float _lastX;
        private float _lastY;

        public event Action<DragEvent> Dragging;

        public DragRecognizer(IPointerSource source, DragConfig config)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _threshold = config.Threshold;
            _source.Pointer += OnPointer;
        }

        private void OnPointer(PointerSample s)
        {
            switch (s.Phase)
            {
                case PointerPhase.Began:
                    _tracking = true;
                    _dragging = false;
                    _activeId = s.PointerId;
                    _startX = _lastX = s.X;
                    _startY = _lastY = s.Y;
                    break;

                case PointerPhase.Moved:
                    if (!_tracking || s.PointerId != _activeId)
                    {
                        break;
                    }
                    if (!_dragging)
                    {
                        float dx = s.X - _startX;
                        float dy = s.Y - _startY;
                        if (dx * dx + dy * dy > _threshold * _threshold)
                        {
                            _dragging = true;
                            Emit(DragPhase.Begin, s);
                            _lastX = s.X;
                            _lastY = s.Y;
                        }
                    }
                    else
                    {
                        Emit(DragPhase.Move, s);
                        _lastX = s.X;
                        _lastY = s.Y;
                    }
                    break;

                case PointerPhase.Ended:
                case PointerPhase.Canceled:
                    if (_tracking && s.PointerId == _activeId)
                    {
                        _tracking = false;
                        if (_dragging)
                        {
                            _dragging = false;
                            Emit(DragPhase.End, s);
                        }
                    }
                    break;
            }
        }

        private void Emit(DragPhase phase, PointerSample s)
        {
            var position = new Point2(s.X, s.Y);
            var delta = new Point2(s.X - _lastX, s.Y - _lastY);
            Dragging?.Invoke(new DragEvent(phase, position, delta));
        }

        public void Dispose() => _source.Pointer -= OnPointer;
    }
}
