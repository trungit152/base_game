using System;
using ExampleGame.Bootstrap;
using ExampleGame.Flow;
using ExampleGame.Puzzle;
using trungnhd.puzzlecore.Boosters;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Flow;
using trungnhd.puzzlecore.Flow.Events;
using trungnhd.puzzlecore.Flow.States;
using trungnhd.puzzlecore.Puzzle;
using UnityEngine;
using VContainer;

namespace ExampleGame.View
{
    /// <summary>
    /// HUD debug bằng IMGUI để TEST ExampleGame bằng nút bấm (không cần uGUI/Canvas/EventSystem).
    /// Nó KHÔNG chứa logic game: chỉ resolve service lõi từ <see cref="ExampleGameLifetimeScope"/> rồi
    /// gửi lệnh qua API lõi (<see cref="IGameStateMachine"/> / <see cref="IPuzzleSession"/> /
    /// <see cref="IBoosterService"/>). Là thành phần phụ trợ — bỏ ra game vẫn chạy.
    /// </summary>
    public sealed class ExampleGameDebugHud : MonoBehaviour
    {
        private const string BoosterId = "add_points";
        private const float AutoStep = 0.5f;

        private IGameStateMachine _machine;
        private IBoosterService _boosters;
        private IBoosterInventory _inventory;
        private IEventBus _events;

        private IPuzzleSession _session;
        private Type _state;
        private bool _auto;
        private float _autoTimer;   
        private GUIStyle _title;

        private void Start()
        {
            var scope = FindObjectOfType<ExampleGameLifetimeScope>();
            if (scope == null || scope.Container == null)
            {
                Debug.LogError("[ExampleGame] Không tìm thấy ExampleGameLifetimeScope đã build. " +
                               "Đặt HUD CHUNG scene với scope.");
                enabled = false;
                return;
            }

            _machine = scope.Container.Resolve<IGameStateMachine>();
            _boosters = scope.Container.Resolve<IBoosterService>();
            _inventory = scope.Container.Resolve<IBoosterInventory>();
            _events = scope.Container.Resolve<IEventBus>();

            // Lắng nghe để cập nhật trạng thái hiển thị (read side, tách rời).
            _events.Subscribe<PuzzleReadyEvent>(OnPuzzleReady);
            _events.Subscribe<StateEnteredEvent>(OnStateEntered);
        }

        private void OnDestroy()
        {
            if (_events == null) return;
            _events.Unsubscribe<PuzzleReadyEvent>(OnPuzzleReady);
            _events.Unsubscribe<StateEnteredEvent>(OnStateEntered);
        }

        private void OnPuzzleReady(PuzzleReadyEvent e) => _session = e.Puzzle;
        private void OnStateEntered(StateEnteredEvent e) => _state = e.StateType;

        private void Update()
        {
            if (!_auto || _session == null) return;
            if (_state != typeof(PlayingState) || _session.IsTerminal) return;

            _autoTimer += Time.deltaTime;
            if (_autoTimer < AutoStep) return;
            _autoTimer = 0f;
            _session.ApplyAction(TapAction.Tap);
        }

        private void OnGUI()
        {
            _title ??= new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 };

            GUILayout.BeginArea(new Rect(12, 12, 330, 330), GUI.skin.box);

            GUILayout.Label("<b>ExampleGame — HUD test (ứng dụng core)</b>", _title);
            GUILayout.Space(4);
            GUILayout.Label("State: " + (_state != null ? _state.Name : "(chưa vào)"));

            if (_session is IPuzzleSession<TapState, TapAction> typed)
            {
                var s = typed.State;
                GUILayout.Label("Score: " + s.Score + " / " + s.Target +
                                "    Taps: " + s.TapsUsed + " / " + s.TapBudget);
            }
            else
            {
                GUILayout.Label("Score: -");
            }

            GUILayout.Label("Outcome: " + (_session != null ? _session.Outcome.ToString() : "-"));
            GUILayout.Label("Booster '" + BoosterId + "': còn " +
                            (_inventory != null ? _inventory.GetCount(BoosterId) : 0));

            GUILayout.Space(8);

            // PLAY — chỉ ở MainMenu.
            if (_state == typeof(MainMenuState))
            {
                if (GUILayout.Button("▶  Play")) _machine.ChangeState<ExampleLoadingState>();
            }

            // TAP / BOOSTER — chỉ khi đang chơi và chưa kết thúc.
            bool canPlay = _session != null && !_session.IsTerminal && _state == typeof(PlayingState);
            using (new GUILayout.HorizontalScope())
            {
                GUI.enabled = canPlay;
                if (GUILayout.Button("Tap +1")) _session.ApplyAction(TapAction.Tap);
                if (GUILayout.Button("Booster +4")) _boosters.Activate(BoosterId, _session);
                GUI.enabled = true;
            }

            GUI.enabled = canPlay;
            _auto = GUILayout.Toggle(_auto, "  Tự động tap mỗi 0.5s");
            GUI.enabled = true;

            // CHƠI LẠI — ở Win/Lose, quay về Menu (Loading sẽ dựng puzzle mới).
            if (_state == typeof(WinState) || _state == typeof(LoseState))
            {
                if (GUILayout.Button("⟲  Về Menu (chơi lại)")) _machine.ChangeState<MainMenuState>();
            }

            GUILayout.EndArea();
        }
    }
}
