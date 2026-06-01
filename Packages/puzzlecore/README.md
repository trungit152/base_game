# PuzzleCore (`trungnhd.puzzlecore`)

Phần lõi engine-agnostic của framework game puzzle **PuzzleCore**. Thuần C# — **không `UnityEngine`,
không DI container** — nên unit-test được headless và tái sử dụng được trên nhiều engine. Unity chỉ là
một adapter (`com.trungnhd.puzzlecore.unity`).

## Triết lý

Một puzzle là một **transition system** tất định: `state → action hợp lệ → state kế tiếp → kết quả`.
Đó là bất biến ổn định, được nắm bởi `IPuzzleRules<TState, TAction>`. Board/grid chỉ là *một* cách biểu
diễn và cố tình **không** phải nền tảng — nhờ vậy các tính năng xuyên suốt (undo/redo, replay, solver,
validator) về sau viết được một lần, tổng quát, dựa trên transition system thay vì dựa vào board.

## Các module (`trungnhd.puzzlecore.*`)

| Namespace | Cung cấp |
|-----------|----------|
| `Common`  | `Result` / `Result<T>` (thành/bại tường minh thay cho exception), `IClock` (seam thời gian). |
| `Signals` | `ISignal`, `ISignalBus`, `SignalBus` — pub/sub đồng bộ, tách rời. |
| `Puzzle`  | `IPuzzleRules<,>` (trái tim), `IPuzzleSession`(+typed), `PuzzleSession<,>`, `PuzzleOutcome`. |
| `Flow`    | `IGameStateMachine` / `GameStateMachine`, `IGameState` / `GameStateBase`, `GameStateContext`, và các state chuẩn (Boot → MainMenu → Loading → Playing → Paused → Win/Lose). |
| `Boosters`| `IBoosterDefinition`, `IBoosterEffect`, `IBoosterInventory`, `IBoosterService` cùng phần cài đặt + context/result kích hoạt. |

## Nguyên tắc thiết kế (SOLID)

- **Open/Closed** — mở rộng bằng cách thêm type, không bao giờ sửa framework:
  - puzzle mới → hiện thực `IPuzzleRules<,>`
  - flow state mới → hiện thực `IGameState` rồi `RegisterState(...)`
  - booster mới → hiện thực `IBoosterEffect` (+ một `IBoosterDefinition`)
  - sự kiện mới → hiện thực `ISignal`
- **Dependency Inversion** — mọi thứ phụ thuộc interface; core không biết engine hay container nào.
  Việc lắp ghép xảy ra ở rìa (xem `com.trungnhd.puzzlecore.unity`, hoặc `new` trong `dev/Headless`).
- **Interface Segregation** — booster tác động lên `IPuzzleSession` (mỏng, không generic). Effect nào
  cần nhiều hơn thì tự khai báo một capability interface **hẹp của riêng nó** và ép kiểu xuống
  (vd `session is IDecrementable`), nên hệ thống booster không bao giờ coupling với state cụ thể.

## Booster chạm vào puzzle mà không coupling

`BoosterService` chỉ biết `IBoosterEffect` và `IPuzzleSession`. Một effect sẽ hoặc:
1. gọi `session.ApplyAction(object)` với một action hợp lệ (rules vẫn là chân lý), hoặc
2. ép kiểu xuống một capability interface do chính nó khai báo (vd `IDecrementable`) — *puzzle* chủ
   động "đăng ký" bằng cách hiện thực interface đó.

Xem `dev/Headless/Demo` để có ví dụ hoàn chỉnh (`CountdownSession : PuzzleSession<,>, IDecrementable`
và `DecrementBoosterEffect`).

## Kiểm chứng headless

Core được chứng minh chạy không cần Unity bằng một console harness thuần:

```powershell
dotnet build dev/Headless/Headless.csproj -c Release
dotnet dev/Headless/bin/Release/net9.0/Headless.dll
```

Kỳ vọng: `RESULT: N passed, 0 failed`, exit code 0. (Chạy **`.dll` qua muxer `dotnet`**, KHÔNG chạy
apphost `.exe` / test host — chúng crash trên bản Windows này với lỗi CET/shadow-stack.)

## Roadmap

- **Phase 1 (bản này):** seam core + state machine luồng game + hệ thống booster.
- **Về sau:** undo/redo & replay (tổng quát trên transition system), module *representation* Grid/Board
  (ngang hàng với core, không phải nền tảng), level/progression, save/load, economy, AI solver.
