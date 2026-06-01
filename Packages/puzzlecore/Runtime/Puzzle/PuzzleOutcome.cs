namespace trungnhd.puzzlecore.Puzzle
{
    /// <summary>Trạng thái kết thúc của một state puzzle. v1 giữ ở dạng enum đơn giản; có thể nâng
    /// thành struct giàu thông tin hơn (điểm, sao, lý do thua) ở phase sau.</summary>
    public enum PuzzleOutcome
    {
        Undecided = 0,
        Won = 1,
        Lost = 2
    }
}
