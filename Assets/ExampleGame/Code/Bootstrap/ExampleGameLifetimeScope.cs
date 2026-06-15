using System.Collections.Generic;
using System.Linq;
using ExampleGame.Boosters;
using ExampleGame.Flow;
using ExampleGame.Puzzle;
using trungnhd.puzzlecore.Boosters;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Flow;
using trungnhd.puzzlecore.Flow.States;
using trungnhd.puzzlecore.Unity.Flow;
using trungnhd.puzzlecore.Unity.Time;
using VContainer;
using VContainer.Unity;

namespace ExampleGame.Bootstrap
{
    /// <summary>
    /// Composition root của ExampleGame — nơi DUY NHẤT "lắp ghép". Tự wiring lõi (giống
    /// <c>GameLifetimeScope</c> của package unity) NHƯNG cắm thêm các type riêng của game: <c>TapRules</c>,
    /// <c>ExampleLoadingState</c>, booster cộng điểm. Không sửa một dòng nào của framework (Open/Closed).
    /// </summary>
    public sealed class ExampleGameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // --- Hạ tầng lõi ---
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<IClock, UnityClock>(Lifetime.Singleton);

            // --- Luật puzzle riêng của game (được inject vào ExampleLoadingState) ---
            builder.RegisterInstance(new TapRules(target: 10, tapBudget: 15));

            // --- Booster (definition dạng class thuần, không cần .asset) ---
            var definitions = new List<IBoosterDefinition>
            {
                new ExampleBoosterDefinition("add_points", "Cộng điểm", new AddPointsEffect(amount: 4)),
            };
            builder.RegisterInstance<IReadOnlyList<IBoosterDefinition>>(definitions);
            builder.Register<IBoosterInventory>(_ =>
            {
                var inventory = new BoosterInventory(definitions);
                inventory.Add("add_points", 1); // cho sẵn 1 cái để demo
                return inventory;
            }, Lifetime.Singleton);
            builder.Register<IBoosterService>(resolver => new BoosterService(
                    resolver.Resolve<IBoosterInventory>(),
                    definitions.ToDictionary(d => d.Id),
                    resolver.Resolve<IEventBus>(),
                    resolver.Resolve<IClock>()),
                Lifetime.Singleton);

            // --- Flow state (dùng ExampleLoadingState thay cho LoadingState mặc định) ---
            builder.Register<IGameState, BootState>(Lifetime.Singleton);
            builder.Register<IGameState, MainMenuState>(Lifetime.Singleton);
            builder.Register<IGameState, ExampleLoadingState>(Lifetime.Singleton);
            builder.Register<IGameState, PlayingState>(Lifetime.Singleton);
            builder.Register<IGameState, PausedState>(Lifetime.Singleton);
            builder.Register<IGameState, WinState>(Lifetime.Singleton);
            builder.Register<IGameState, LoseState>(Lifetime.Singleton);

            builder.Register<IGameStateMachine>(resolver =>
            {
                var machine = new GameStateMachine(resolver.Resolve<IEventBus>(), resolver.Resolve<IClock>());
                foreach (var state in resolver.Resolve<IReadOnlyList<IGameState>>())
                {
                    machine.RegisterState(state);
                }
                return machine;
            }, Lifetime.Singleton);

            // --- Entry points ---
            builder.RegisterEntryPoint<GameFlowDriver>();        // boot vào BootState + tick machine mỗi frame
            builder.RegisterEntryPoint<ExampleGameController>();  // observer: log event ra Console (HUD lo điều khiển)
        }
    }
}
