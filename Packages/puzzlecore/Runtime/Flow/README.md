# Flow

**State machine luồng game** — vòng đời cấp cao mà mọi game puzzle đều có
(Boot → MainMenu → Loading → Playing → Paused → Win/Lose). Phụ thuộc `Common`, `Signals`, `Puzzle`.

## Các type

| Type | Mục đích |
|------|----------|
| `IGameState` | Một node flow: `Enter`, `Exit`, `Tick`, và guard `CanTransitionTo(Type)`. |
| `GameStateBase` | Lớp cơ sở tiện dụng (vòng đời no-op, guard mở, helper `RequestState<T>()`). |
| `GameStateContext` | Service dùng chung trao cho state: `Signals`, `Clock`, `Machine`, `ActivePuzzle`, `Blackboard`. |
| `IGameStateMachine` / `GameStateMachine` | Đăng ký state và chạy transition có guard `Exit → Enter`; tick state hiện tại. |
| `States/…` | Các state chuẩn, override được: `BootState`, `MainMenuState`, `LoadingState`, `PlayingState`, `PausedState`, `WinState`, `LoseState`. |
| `Signals/StateEnteredSignal` | Phát sau khi một state trở thành state hiện tại. |
| `Signals/StateTransitionRejectedSignal` | Phát khi một guard chặn transition. |

## Transition hoạt động thế nào

`ChangeState<T>()` (hoặc `ChangeState(Type)`):
1. fail nếu type đích chưa được đăng ký;
2. hỏi `CanTransitionTo(target)` của state hiện tại — nếu từ chối, phát `StateTransitionRejectedSignal`
   và trả về `Result.Fail` (state hiện tại không đổi);
3. ngược lại chạy `current.Exit` → gán current → `next.Enter` → phát `StateEnteredSignal`.

Guard nằm **trong các state** (vd `PausedState` chỉ cho phép `PlayingState`/`MainMenuState`), nên bản thân
machine không bao giờ phình ra một bảng transition.

## Quy ước quan trọng: chuyển state từ `Tick`, không phải `Enter`

Yêu cầu state kế tiếp trong `Tick` (như `BootState`/`LoadingState`/`PlayingState` làm), không phải trong
`Enter`. `Enter` chỉ để khởi tạo — chuyển state bên trong nó sẽ gọi đệ quy vào machine. Các state chuẩn
tuân theo quy ước này.

## State không tự dựng service

Một state đọc mọi thứ nó cần từ `GameStateContext` (signal bus, clock, puzzle đang chạy, một `Blackboard`
dạng string→object cho dữ liệu riêng của game). Nhờ đó state nhẹ và test được headless — không cần DI container.

## Cách dùng

```csharp
var machine = new GameStateMachine(signalBus, clock);
machine.RegisterState(new BootState());
machine.RegisterState(new MainMenuState());
machine.RegisterState(new LoadingState());
machine.RegisterState(new PlayingState());
machine.RegisterState(new PausedState());
machine.RegisterState(new WinState());
machine.RegisterState(new LoseState());

machine.ChangeState<BootState>();
// mỗi frame:
machine.Tick(clock.DeltaTime);
```

## Mở rộng (Open/Closed)

- **State mới** → hiện thực `IGameState` (hoặc kế thừa `GameStateBase`) rồi `RegisterState(...)`. Machine
  không đổi.
- **Guard tuỳ biến** → override `CanTransitionTo`.
- **Cấp puzzle cho một màn** → hoặc gán `context.ActivePuzzle` trước khi vào Loading, hoặc kế thừa
  `LoadingState` và override `CreatePuzzle(context)` để dựng `PuzzleSession` của bạn.
