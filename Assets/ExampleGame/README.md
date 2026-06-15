# ExampleGame — ví dụ tối giản ỨNG DỤNG PuzzleCore

Một mini-game **"Tap Target"** (chạm để đạt điểm mục tiêu) với mục đích DUY NHẤT là minh hoạ cách
**dùng framework `puzzlecore`**, không đầu tư gameplay. Nó áp dụng đủ 4 trụ của lõi:

| Trụ lõi | File trong ExampleGame | Vai trò |
|---------|------------------------|---------|
| **Puzzle** (`IPuzzleRules`) | `Puzzle/TapState`, `TapAction`, `TapRules`, `TapSession` | Transition system tất định: Tap → Score+1; đạt Target = Thắng, hết lượt = Thua. |
| **Booster** (`IBoosterEffect` + ISP) | `Puzzle/IScorable`, `Boosters/AddPointsEffect`, `ExampleBoosterDefinition` | Booster "cộng điểm" chạm puzzle qua capability hẹp `IScorable` — không coupling `TapState`. |
| **Flow** (`GameStateMachine`) | `Flow/ExampleLoadingState` | Override `CreatePuzzle` để cắm puzzle riêng vào luồng Boot→MainMenu→Loading→Playing→Win/Lose. |
| **Events** (`EventBus`) | `Flow/PuzzleReadyEvent`, `Bootstrap/ExampleGameController`, `View/ExampleGameDebugHud` | Tách "read side" (observer log) và "write side" (HUD ra lệnh) qua event bus. |

Lắp ghép ở `Bootstrap/ExampleGameLifetimeScope` (composition root VContainer — nơi DUY NHẤT lắp ghép,
giống `GameLifetimeScope` của package `puzzlecore.unity`, nhưng cắm thêm type của game). Đây chính là
bằng chứng **Open/Closed**: thêm puzzle/booster/state mới = thêm type, KHÔNG sửa framework.

## Vai trò các thành phần Bootstrap/View

- **`ExampleGameController`** (entry point) — *observer thuần*: chỉ subscribe event Flow/Puzzle/Booster
  rồi log ra Console. Không điều khiển gì.
- **`ExampleGameDebugHud`** (MonoBehaviour, IMGUI) — *bên điều khiển*: vẽ nút **Play / Tap +1 /
  Booster +4 / Về Menu** và một toggle *Tự động tap*. Nó resolve service lõi từ LifetimeScope rồi gửi
  lệnh qua `IGameStateMachine` / `IPuzzleSession` / `IBoosterService`. Là thành phần phụ trợ — bỏ ra
  game vẫn chạy.
- **`GameFlowDriver`** (từ package unity) — boot vào `BootState` và tick state machine mỗi frame
  (nhờ vậy `PlayingState` tự phát hiện Win/Lose).

## Cách chạy (khuyến nghị: dùng menu dựng scene)

1. Mở project trong Unity (Unity tự sinh `.meta` cho thư mục mới này; chờ biên dịch xong).
2. Menu **`Tools ▸ ExampleGame ▸ Tạo Scene Test`** — Unity sẽ tạo & lưu `Assets/ExampleGame/ExampleGame.unity`
   với sẵn 1 GameObject mang `ExampleGameLifetimeScope` + `ExampleGameDebugHud`.
   *(Dựng bằng code Editor cho chắc — không viết tay file `.unity`.)*
3. Bấm **Play**. HUD hiện ở góc trên-trái: bấm **Play → Tap +1 → Booster +4 …** để đạt 10 điểm
   trong 15 lượt (Win), hoặc hết lượt (Lose). Mọi tương tác in log `[ExampleGame] …` ở Console.

> Tự dựng tay cũng được: tạo Scene mới → 1 GameObject rỗng → Add Component `ExampleGameLifetimeScope`
> **và** `ExampleGameDebugHud`. Yêu cầu: project có **VContainer** (`jp.hadashikick.vcontainer`) — đã có
> sẵn vì `CatchHim` dùng.

## Các điểm "ứng dụng core" đáng chú ý

- `TapState` là `readonly struct`, `TapRules.Apply` trả state MỚI → giữ hợp đồng PURE của transition
  system (tiền đề cho undo/replay/solver sau này).
- `TapSession.AddPoints` (capability `IScorable`) **gọi lại `ApplyAction(Tap)` hợp lệ** thay vì sửa
  state thô → booster không bao giờ vượt mặt rules.
- HUD chỉ giữ `IPuzzleSession` (không generic) để ra lệnh, nhưng ép kiểu sang
  `IPuzzleSession<TapState, TapAction>` để đọc Score/Taps hiển thị — minh hoạ 2 góc nhìn session.
- `ExampleLoadingState` LUÔN dựng puzzle mới mỗi lần load → chơi lại được (chủ động né gap
  "replay dùng lại `ActivePuzzle` terminal" của lõi v1, xem memory `puzzlekit-architecture`).

## Lưu ý

- Code này là Unity (dùng `UnityEngine`/VContainer) nên **không** kiểm chứng được bằng headless harness;
  đã đối chiếu từng chữ ký API với lõi, nhưng lỗi (nếu có) chỉ lộ khi Unity biên dịch.
- HUD dùng **IMGUI** (`OnGUI`) cho gọn — không cần Canvas/EventSystem. Muốn UI thật (uGUI/TMP) thì thay
  HUD bằng một presenter + Button, vẫn gọi đúng các API lõi như trên.
