using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>
    /// Hành vi mà một booster áp dụng lên puzzle đang chạy. Đây là SEAM mở rộng chính: thêm một
    /// booster mới bằng cách viết một effect mới — không sửa code framework (Open/Closed).
    /// <para>
    /// Một effect tác động lên puzzle thông qua <see cref="Puzzle.IPuzzleSession"/> (không generic) ở
    /// trong context, hoặc bằng cách ép kiểu xuống một capability interface hẹp do chính nó khai báo.
    /// Effect không bao giờ được tham chiếu kiểu state cụ thể của puzzle.
    /// </para>
    /// </summary>
    public interface IBoosterEffect
    {
        Result Apply(BoosterActivationContext context);
    }
}
