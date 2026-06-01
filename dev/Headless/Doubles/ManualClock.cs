using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Headless.Doubles
{
    /// <summary><see cref="IClock"/> tất định cho test; thời gian chỉ nhích khi được yêu cầu.</summary>
    public sealed class ManualClock : IClock
    {
        public double Now { get; private set; }
        public double DeltaTime { get; private set; }

        public void Advance(double deltaTime)
        {
            DeltaTime = deltaTime;
            Now += deltaTime;
        }
    }
}
