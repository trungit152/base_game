using System;
using CatchHim.Gameplay.Actors;
using trungnhd.puzzlecore.input;
using VContainer.Unity;

namespace CatchHim
{
    public sealed class ActorInputController : IStartable, IDisposable
    {
        private readonly ISwipeInput _input;
        private readonly ActorManager _actors;

        public ActorInputController(ISwipeInput input, ActorManager actors)
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
            _actors.MoveAll(direction);
        }
    }
}
