using System;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Flow
{
    /// <summary>
    /// Lớp cơ sở tiện dụng cho state: các method vòng đời mặc định không làm gì và guard mở, để lớp
    /// con chỉ override những gì cần. Cung cấp helper chuyển state có kiểu.
    /// </summary>
    public abstract class GameStateBase : IGameState
    {
        public virtual void Enter(GameStateContext context) { }
        public virtual void Exit(GameStateContext context) { }
        public virtual void Tick(GameStateContext context, double deltaTime) { }
        public virtual bool CanTransitionTo(Type nextStateType) => true;

        /// <summary>Yêu cầu chuyển sang <typeparamref name="TState"/> qua machine trong context.</summary>
        protected static Result RequestState<TState>(GameStateContext context) where TState : IGameState
            => context.Machine.ChangeState<TState>();
    }
}
