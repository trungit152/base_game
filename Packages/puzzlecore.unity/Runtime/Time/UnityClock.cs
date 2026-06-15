using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Unity.Time
{
    public sealed class UnityClock : IClock
    {
        public double Now => UnityEngine.Time.timeAsDouble;
        public double DeltaTime => UnityEngine.Time.deltaTime;
    }
}
