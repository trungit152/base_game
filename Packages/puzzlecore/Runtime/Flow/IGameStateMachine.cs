using System;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Flow
{
    /// <summary>Điều phối trình tự các <see cref="IGameState"/> với transition có guard.</summary>
    public interface IGameStateMachine
    {
        IGameState Current { get; }

        /// <summary>Đăng ký (hoặc thay thế) một state, khoá theo kiểu cụ thể của nó.</summary>
        void RegisterState(IGameState state);

        Result ChangeState(Type stateType);
        Result ChangeState<TState>() where TState : IGameState;

        void Tick(double deltaTime);
    }
}
