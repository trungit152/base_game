# Puzzle

**Trái tim** của framework: một puzzle được mô hình hoá như một *transition system* tất định
(`state → action hợp lệ → state kế tiếp → kết quả`). Mọi thứ khác (flow, booster) điều khiển hoặc quan
sát một puzzle thông qua seam này. Phụ thuộc `Common` và `Signals`.

## Các type

| Type | Mục đích |
|------|----------|
| `IPuzzleRules<TState,TAction>` | Rules thuần: `InitialState`, `GetLegalActions`, `Apply`, `GetOutcome`. |
| `PuzzleOutcome` | `Undecided` / `Won` / `Lost`. |
| `IPuzzleSession` | Góc nhìn **không generic**: `Outcome`, `IsTerminal`, `ApplyAction(object)`, `Reset`. |
| `IPuzzleSession<TState,TAction>` | Góc nhìn có kiểu: `State`, `LegalActions`, `Rules`, `ApplyAction(TAction)`. |
| `PuzzleSession<TState,TAction>` | Session cụ thể duy nhất. Giữ state, kiểm tra & áp dụng action, tính lại outcome, phát signal. |
| `Signals/PuzzleActionAppliedSignal` | Phát sau khi apply thành công. |
| `Signals/PuzzleOutcomeChangedSignal` | Phát khi outcome chuyển trạng thái (vd Undecided → Won). |

## Hợp đồng thuần khiết (purity)

Phần cài đặt `IPuzzleRules` phải **thuần khiết**: không có field biến đổi, không mutate đầu vào, và `Apply`
trả về một state *mới*. Tính thuần khiết là thứ giúp về sau viết undo/redo, replay, solver và validator
một lần và tổng quát. Dùng `TState` bất biến (một `readonly struct` hoặc `record`).

## Board ≠ nền tảng

`TState` có thể là bất cứ thứ gì — một grid, một tuple Tháp Hà Nội, một bộ bài, một từ. Board/Grid chỉ là
*một* module representation tương lai, **ngang hàng** với core này, không phải nền móng của nó.

## Cách dùng

```csharp
public readonly struct MyState { /* bất biến */ }
public enum MyAction { /* ... */ }

public sealed class MyRules : IPuzzleRules<MyState, MyAction>
{
    public MyState InitialState => /* ... */;
    public IReadOnlyList<MyAction> GetLegalActions(MyState s) => /* ... */;
    public MyState Apply(MyState s, MyAction a) => /* state mới */;
    public PuzzleOutcome GetOutcome(MyState s) => /* Undecided/Won/Lost */;
}

var session = new PuzzleSession<MyState, MyAction>(new MyRules(), signalBus);
Result r = session.ApplyAction(someAction);   // fail nếu terminal / không hợp lệ / sai kiểu
if (session.Outcome == PuzzleOutcome.Won) { /* ... */ }
```

`PuzzleSession` áp đặt rules: apply khi `IsTerminal`, hoặc một action không nằm trong
`GetLegalActions(State)`, sẽ trả về `Result.Fail` và không đổi gì cả.

## Cách các module khác giữ được sự tách rời

Booster và flow state `Playing` chỉ giữ `IPuzzleSession` **không generic**, nên chúng làm việc với *bất
kỳ* puzzle nào mà không cần biết `TState`/`TAction` của nó. Một effect cần làm nhiều hơn việc apply action
hợp lệ thì tự khai báo một **capability interface hẹp của riêng mình** rồi ép kiểu xuống (xem
`Boosters/README.md` và ví dụ `IDecrementable` trong `dev/Headless/Demo`).
