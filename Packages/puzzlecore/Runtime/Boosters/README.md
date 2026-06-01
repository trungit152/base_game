# Boosters

Định nghĩa, sở hữu và kích hoạt **booster** tác động lên puzzle đang chạy. Tổng quát trên mọi puzzle —
chỉ biết `IBoosterEffect` và `IPuzzleSession` (không generic), không bao giờ biết state cụ thể. Phụ thuộc
`Common`, `Signals`, `Puzzle`.

## Các type

| Type | Mục đích |
|------|----------|
| `IBoosterDefinition` | Metadata tĩnh: `Id`, `DisplayName`, `Effect`. (Một `ScriptableObject` trong Unity, một class thuần khi headless.) |
| `IBoosterEffect` | **Seam mở rộng.** `Result Apply(BoosterActivationContext)` — hành vi áp dụng lên puzzle. |
| `BoosterActivationContext` | Thứ effect nhận: `Puzzle`, `Signals`, `Clock`, `Args`. |
| `IBooster` / `Booster` | Một definition đi kèm `Count` sở hữu. |
| `IBoosterInventory` / `BoosterInventory` | Sở hữu số lượng; nơi **duy nhất** stock được đổi (`Add`, `TryConsume`, `GetCount`, `Has`). |
| `IBoosterService` / `BoosterService` | Facade game/UI gọi: `CanActivate`, `Activate`. |
| `BoosterActivationResult` | `Activated`, `BoosterId`, `RemainingCount`, `Error`. |
| `Signals/BoosterActivatedSignal`, `BoosterActivationFailedSignal` | Sự kiện kết quả. |

## Luồng kích hoạt (`BoosterService.Activate`)

1. Id không tồn tại → fail. Puzzle null hoặc đã kết thúc → fail.
2. `inventory.TryConsume(id)` → nếu không đủ, phát `BoosterActivationFailedSignal`, fail.
3. Dựng `BoosterActivationContext` và gọi `definition.Effect.Apply(ctx)`.
4. Nếu effect fail (hoặc ném exception) → **hoàn lại** booster, phát signal fail, fail.
5. Thành công → phát `BoosterActivatedSignal`, trả về `Success` kèm số lượng còn lại.

Trừ-rồi-hoàn-lại làm cho việc kích hoạt mang tính nguyên tử với bên gọi: một booster chỉ bị "tiêu" nếu
effect thực sự được áp dụng. (Việc dùng *thành công nhưng không thắng được* có nên hoàn lại không là quyết
định của từng game — override service hoặc logic của effect nếu bạn muốn ngữ nghĩa khác.)

## Cách một effect chạm vào puzzle mà không coupling

`BoosterService` truyền cho effect `IPuzzleSession` **không generic**. Effect khi đó hoặc:

- gọi `context.Puzzle.ApplyAction(someAction)` — rules vẫn là chân lý; hoặc
- ép kiểu xuống một **capability interface hẹp do chính nó khai báo**, mà puzzle chủ động hiện thực:

```csharp
public interface IDecrementable { Result Decrement(int amount); }   // do tính năng của effect khai báo

public sealed class DecrementBoosterEffect : IBoosterEffect
{
    private readonly int _amount;
    public DecrementBoosterEffect(int amount) => _amount = amount;

    public Result Apply(BoosterActivationContext ctx)
        => ctx.Puzzle is IDecrementable d
            ? d.Decrement(_amount)
            : Result.Fail("puzzle does not support decrement");
}
```

Module booster không bao giờ tham chiếu state cụ thể của puzzle — sự coupling chỉ nằm bên trong đúng
effect cụ thể này cùng đúng puzzle đã chọn hiện thực `IDecrementable`. (Ví dụ hoàn chỉnh ở
`dev/Headless/Demo`.)

## Cách dùng

```csharp
IReadOnlyList<IBoosterDefinition> defs = /* từ asset hoặc code */;
var inventory = new BoosterInventory(defs);
var service   = new BoosterService(inventory, defs.ToDictionary(d => d.Id), signalBus, clock);

inventory.Add("decrement", 3);

if (service.CanActivate("decrement", session))
{
    BoosterActivationResult r = service.Activate("decrement", session);
    // r.Activated, r.RemainingCount, r.Error
}
```

## Mở rộng (Open/Closed)

Một booster mới = một `IBoosterEffect` mới + một `IBoosterDefinition` trả về nó. Không sửa
`BoosterService`, `BoosterInventory`, hay bất kỳ type hiện có nào.
