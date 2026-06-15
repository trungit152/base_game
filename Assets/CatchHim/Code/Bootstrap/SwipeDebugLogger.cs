using System;
using trungnhd.puzzlecore.input;
using UnityEngine;
using VContainer.Unity;

namespace CatchHim
{
    public sealed class SwipeDebugLogger : IStartable, IDisposable
    {
        private readonly ISwipeInput _input;

        public SwipeDebugLogger(ISwipeInput input)
        {
            _input = input;
        }

        public void Start()
        {
            _input.Swiped += OnSwiped;
        }

        private void OnSwiped(SwipeDirection direction)
        {
            Debug.Log($"[CatchHim] Swipe: {direction}");
        }

        public void Dispose()
        {
            _input.Swiped -= OnSwiped;
        }
    }
}
