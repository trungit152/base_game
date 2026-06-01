using System;

namespace trungnhd.puzzlecore.Flow
{
    /// <summary>
    /// Một node trong state machine luồng game. Phần cài đặt nhận mọi thứ cần thiết qua
    /// <see cref="GameStateContext"/> — không tự khởi tạo service, nhờ vậy test được headless. Thêm
    /// một flow state mới bằng cách viết một <see cref="IGameState"/> mới; machine không phải sửa (Open/Closed).
    /// <para>
    /// Quy ước: yêu cầu chuyển state trong <see cref="Tick"/>, không phải trong <see cref="Enter"/>, để
    /// tránh transition đệ quy. <see cref="Enter"/> chỉ dùng để khởi tạo.
    /// </para>
    /// </summary>
    public interface IGameState
    {
        void Enter(GameStateContext context);
        void Exit(GameStateContext context);
        void Tick(GameStateContext context, double deltaTime);

        /// <summary>Guard: machine có được phép chuyển từ state này sang <paramref name="nextStateType"/> không?</summary>
        bool CanTransitionTo(Type nextStateType);
    }
}
