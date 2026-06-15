using CatchHim.Gameplay.Actors;
using CatchHim.Gameplay.Grid;
using trungnhd.puzzlecore.input;
using VContainer;
using VContainer.Unity;

namespace CatchHim
{
    public class CatchHimLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GridManager>(Lifetime.Singleton);
            builder.Register<ActorManager>(Lifetime.Singleton);

            // Input chain: Unity pointer source -> swipe recognizer (ISwipeInput).
            builder.RegisterInstance(SwipeConfig.Default);
            builder.Register<IPointerSource, UnityPointerSource>(Lifetime.Singleton);
            builder.Register<ISwipeInput, SwipeRecognizer>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<GridViewManager>();
            builder.RegisterComponentInHierarchy<ActorViewManager>();

            builder.RegisterEntryPoint<GridController>();
            builder.RegisterEntryPoint<ThiefInputController>();
        }
    }
}
