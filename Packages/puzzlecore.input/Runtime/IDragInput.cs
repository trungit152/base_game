using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>Giai đoạn của một thao tác kéo.</summary>
    public enum DragPhase
    {
        Begin,
        Move,
        End
    }

    /// <summary>Một bước trong thao tác kéo: pha, vị trí hiện tại, và độ dời so với mẫu trước.</summary>
    public readonly struct DragEvent
    {
        public readonly DragPhase Phase;
        public readonly Point2 Position;
        public readonly Point2 Delta;

        public DragEvent(DragPhase phase, Point2 position, Point2 delta)
        {
            Phase = phase;
            Position = position;
            Delta = delta;
        }
    }

    /// <summary>Phát chuỗi Begin → Move* → End khi con trỏ chạm-kéo-nhả.</summary>
    public interface IDragInput
    {
        event Action<DragEvent> Dragging;
    }
}
