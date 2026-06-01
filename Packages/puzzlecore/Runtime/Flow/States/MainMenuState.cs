namespace trungnhd.puzzlecore.Flow.States
{
    /// <summary>
    /// Màn hình menu chờ. Không tự chuyển tiếp — game sẽ gọi <c>ChangeState&lt;LoadingState&gt;()</c> khi
    /// người chơi bấm "Play" (thường là một signal UI được xử lý ở tầng adapter).
    /// </summary>
    public class MainMenuState : GameStateBase
    {
    }
}
