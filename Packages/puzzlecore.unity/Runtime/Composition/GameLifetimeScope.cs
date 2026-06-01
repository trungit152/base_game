using System.Collections.Generic;
using System.Linq;
using trungnhd.puzzlecore.Boosters;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Flow;
using trungnhd.puzzlecore.Flow.States;
using trungnhd.puzzlecore.Signals;
using trungnhd.puzzlecore.Unity.Boosters;
using trungnhd.puzzlecore.Unity.Flow;
using trungnhd.puzzlecore.Unity.Time;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace trungnhd.puzzlecore.Unity.Composition
{
    /// <summary>
    /// Composition root — type DUY NHẤT trong framework tham chiếu VContainer. Nó wiring core (thuần
    /// C#): interface -&gt; cài đặt. Vì core chỉ phụ thuộc interface và constructor thuần, cùng object
    /// graph đó có thể dựng được headless bằng `new` (xem dev/Headless) — đó chính là bằng chứng core
    /// không phụ thuộc container.
    /// <para>
    /// Để mở rộng cho một game: thêm các asset booster definition vào <see cref="_boosterDefinitions"/>,
    /// và đăng ký bất kỳ <see cref="IGameState"/> mới nào ở dưới — không cần sửa code framework.
    /// </para>
    /// </summary>
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private List<BoosterDefinitionAsset> _boosterDefinitions = new List<BoosterDefinitionAsset>();

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Hạ tầng core ---
            builder.Register<ISignalBus, SignalBus>(Lifetime.Singleton);
            builder.Register<IClock, UnityClock>(Lifetime.Singleton);

            // --- Booster ---
            var definitions = _boosterDefinitions.Cast<IBoosterDefinition>().ToList();
            builder.RegisterInstance<IReadOnlyList<IBoosterDefinition>>(definitions);
            builder.Register<IBoosterInventory>(_ => new BoosterInventory(definitions), Lifetime.Singleton);
            builder.Register<IBoosterService>(resolver => new BoosterService(
                    resolver.Resolve<IBoosterInventory>(),
                    definitions.ToDictionary(d => d.Id),
                    resolver.Resolve<ISignalBus>(),
                    resolver.Resolve<IClock>()),
                Lifetime.Singleton);

            // --- Flow state (Open/Closed: thêm state -> đăng ký tại đây) ---
            builder.Register<IGameState, BootState>(Lifetime.Singleton);
            builder.Register<IGameState, MainMenuState>(Lifetime.Singleton);
            builder.Register<IGameState, LoadingState>(Lifetime.Singleton);
            builder.Register<IGameState, PlayingState>(Lifetime.Singleton);
            builder.Register<IGameState, PausedState>(Lifetime.Singleton);
            builder.Register<IGameState, WinState>(Lifetime.Singleton);
            builder.Register<IGameState, LoseState>(Lifetime.Singleton);

            builder.Register<IGameStateMachine>(resolver =>
            {
                var machine = new GameStateMachine(resolver.Resolve<ISignalBus>(), resolver.Resolve<IClock>());
                foreach (var state in resolver.Resolve<IReadOnlyList<IGameState>>())
                {
                    machine.RegisterState(state);
                }
                return machine;
            }, Lifetime.Singleton);

            // --- Entry point: boot luồng game và tick mỗi frame ---
            builder.RegisterEntryPoint<GameFlowDriver>();
        }
    }
}
