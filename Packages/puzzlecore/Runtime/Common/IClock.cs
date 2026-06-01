namespace trungnhd.puzzlecore.Common
{
    /// <summary>
    /// Trừu tượng hoá thời gian để core (engine-agnostic) không bao giờ tham chiếu UnityEngine.Time.
    /// Unity cung cấp clock dựa trên <c>UnityEngine.Time</c>; test headless cung cấp clock thủ công.
    /// </summary>
    public interface IClock
    {
        /// <summary>Số giây tính từ một mốc cố định (thời gian game, đơn điệu tăng).</summary>
        double Now { get; }

        /// <summary>Số giây trôi qua kể từ tick/frame trước.</summary>
        double DeltaTime { get; }
    }
}
