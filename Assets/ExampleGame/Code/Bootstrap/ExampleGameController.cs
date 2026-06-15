using System;
using ExampleGame.Flow;
using trungnhd.puzzlecore.Boosters.Events;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Flow.Events;
using trungnhd.puzzlecore.Puzzle.Events;
using UnityEngine;
using VContainer.Unity;

namespace ExampleGame.Bootstrap
{
    /// <summary>
    /// Observer THUẦN: lắng nghe event của Flow/Puzzle/Booster rồi log ra Console. KHÔNG điều khiển
    /// game — việc bấm Play/Tap/Booster do HUD (hoặc input) đảm nhận. Minh hoạ "read side" tách rời
    /// hoàn toàn qua <see cref="IEventBus"/>: chỉ phụ thuộc duy nhất event bus, không biết ai phát.
    /// </summary>
    public sealed class ExampleGameController : IStartable, IDisposable
    {
        private readonly IEventBus _events;

        public ExampleGameController(IEventBus events)
        {
            _events = events;
        }

        public void Start()
        {
            _events.Subscribe<StateEnteredEvent>(OnStateEntered);
            _events.Subscribe<StateTransitionRejectedEvent>(OnTransitionRejected);
            _events.Subscribe<PuzzleReadyEvent>(OnPuzzleReady);
            _events.Subscribe<PuzzleOutcomeChangedEvent>(OnOutcomeChanged);
            _events.Subscribe<BoosterActivatedEvent>(OnBoosterActivated);
            _events.Subscribe<BoosterActivationFailedEvent>(OnBoosterFailed);
            Debug.Log("[ExampleGame] Observer sẵn sàng — dùng HUD (nút bấm) để chơi.");
        }

        private void OnStateEntered(StateEnteredEvent e)
            => Debug.Log("[ExampleGame][Flow] Vào state: " + e.StateType.Name);

        private void OnTransitionRejected(StateTransitionRejectedEvent e)
            => Debug.LogWarning("[ExampleGame][Flow] Bị chặn: " + e.FromState.Name + " -> " + e.ToState.Name + " (" + e.Reason + ")");

        private void OnPuzzleReady(PuzzleReadyEvent e)
            => Debug.Log("[ExampleGame] Puzzle sẵn sàng.");

        private void OnOutcomeChanged(PuzzleOutcomeChangedEvent e)
            => Debug.Log("[ExampleGame][Puzzle] Outcome: " + e.Previous + " -> " + e.Current);

        private void OnBoosterActivated(BoosterActivatedEvent e)
            => Debug.Log("[ExampleGame][Booster] Kích hoạt '" + e.BoosterId + "', còn lại " + e.RemainingCount);

        private void OnBoosterFailed(BoosterActivationFailedEvent e)
            => Debug.LogWarning("[ExampleGame][Booster] Thất bại '" + e.BoosterId + "': " + e.Reason);

        public void Dispose()
        {
            _events.Unsubscribe<StateEnteredEvent>(OnStateEntered);
            _events.Unsubscribe<StateTransitionRejectedEvent>(OnTransitionRejected);
            _events.Unsubscribe<PuzzleReadyEvent>(OnPuzzleReady);
            _events.Unsubscribe<PuzzleOutcomeChangedEvent>(OnOutcomeChanged);
            _events.Unsubscribe<BoosterActivatedEvent>(OnBoosterActivated);
            _events.Unsubscribe<BoosterActivationFailedEvent>(OnBoosterFailed);
        }
    }
}
