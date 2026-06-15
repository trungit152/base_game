using System;
using CatchHim.Gameplay.Actors;
using trungnhd.puzzlecore.input;
using VContainer.Unity;

namespace CatchHim
{
    /// <summary>
    /// Translates swipe input into thief movement. The actor layer owns the rules (bounds,
    /// walkable terrain); this just maps a swipe to a move request on the thief.
    /// </summary>
    public sealed class ThiefInputController : IStartable, IDisposable
    {
        private readonly ISwipeInput _input;
        private readonly ActorManager _actors;

        public ThiefInputController(ISwipeInput input, ActorManager actors)
        {
            _input = input;
            _actors = actors;
        }

        public void Start()
        {
            _input.Swiped += OnSwiped;
        }

        public void Dispose()
        {
            _input.Swiped -= OnSwiped;
        }

        private void OnSwiped(SwipeDirection direction)
        {
            var thief = _actors.GetActor(ActorType.Thief);
            if (thief != null)
                _actors.TryMove(thief, direction);
        }
    }
}
