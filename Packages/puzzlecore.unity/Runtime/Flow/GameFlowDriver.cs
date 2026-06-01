using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Flow;
using trungnhd.puzzlecore.Flow.States;
using VContainer.Unity;

namespace trungnhd.puzzlecore.Unity.Flow
{
    /// <summary>
    /// Entry point của VContainer, điều khiển state machine luồng game: khởi động vào boot state lúc
    /// start và tick machine mỗi frame. Đây là adapter nối vòng lặp frame của Unity với
    /// <see cref="IGameStateMachine"/> (engine-agnostic).
    /// </summary>
    public sealed class GameFlowDriver : IStartable, ITickable
    {
        private readonly IGameStateMachine _machine;
        private readonly IClock _clock;

        public GameFlowDriver(IGameStateMachine machine, IClock clock)
        {
            _machine = machine;
            _clock = clock;
        }

        public void Start()
        {
            _machine.ChangeState<BootState>();
        }

        public void Tick()
        {
            _machine.Tick(_clock.DeltaTime);
        }
    }
}
