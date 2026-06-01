using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Flow.Signals;
using trungnhd.puzzlecore.Puzzle;
using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Flow
{
    /// <summary>
    /// State machine mặc định: sở hữu các state đã đăng ký và <see cref="GameStateContext"/> dùng chung,
    /// chạy transition có guard theo trình tự Exit -&gt; Enter, và tick state hiện tại. Thêm state không
    /// bao giờ phải sửa class này — hãy gọi <see cref="RegisterState"/>.
    /// </summary>
    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
        private readonly ISignalBus _signals;
        private readonly GameStateContext _context;

        public IGameState Current { get; private set; }

        public GameStateMachine(ISignalBus signals, IClock clock, IPuzzleSession activePuzzle = null)
        {
            _signals = signals ?? throw new ArgumentNullException(nameof(signals));
            _context = new GameStateContext(signals, clock, this) { ActivePuzzle = activePuzzle };
        }

        public void RegisterState(IGameState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            _states[state.GetType()] = state;
        }

        public Result ChangeState<TState>() where TState : IGameState
            => ChangeState(typeof(TState));

        public Result ChangeState(Type stateType)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));

            if (!_states.TryGetValue(stateType, out var next))
            {
                return Result.Fail("state not registered: " + stateType.Name);
            }

            if (Current != null && !Current.CanTransitionTo(stateType))
            {
                _signals.Publish(new StateTransitionRejectedSignal(Current.GetType(), stateType, "rejected by guard"));
                return Result.Fail("transition rejected by guard");
            }

            Current?.Exit(_context);
            Current = next;
            Current.Enter(_context);
            _signals.Publish(new StateEnteredSignal(stateType));
            return Result.Ok();
        }

        public void Tick(double deltaTime)
        {
            Current?.Tick(_context, deltaTime);
        }
    }
}
