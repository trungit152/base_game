namespace trungnhd.puzzlecore.Flow.States
{
    /// <summary>
    /// Khởi tạo một lần khi vào game. Chuyển sang <see cref="MainMenuState"/> ngay ở tick đầu tiên
    /// (chuyển state từ Tick thay vì Enter để machine tránh được đệ quy). Override
    /// <see cref="GameStateBase.Enter"/> ở lớp con để chạy việc khởi tạo thật.
    /// </summary>
    public class BootState : GameStateBase
    {
        public override void Tick(GameStateContext context, double deltaTime)
        {
            RequestState<MainMenuState>(context);
        }
    }
}
