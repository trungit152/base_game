namespace trungnhd.puzzlecore.Common
{
    public interface IClock
    {
        /// <summary>Số giây tính từ một mốc cố định</summary>
        double Now { get; }

        /// <summary>Số giây trôi qua kể từ frame trước</summary>
        double DeltaTime { get; }
    }
}
